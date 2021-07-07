using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Hosting.Shell;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Systems.Behaviors
{
    /// <summary>
    /// Links a behavior to execute with the parameters to execute it with
    /// </summary>
    public class BehaviorExecution
    {
        /// <summary>
        /// The behavior to execute
        /// </summary>
        public BehaviorBase BehaviorBase { get; set; }
        
        /// <summary>
        /// The parameters to pass to the behavior for (de)serialization and execution
        /// </summary>
        public BehaviorExecutionParameters BehaviorExecutionParameters { get; set; }
    }
    
    public class BehaviorTree
    {
        /// <summary>
        /// Creates a behavior tree
        /// </summary>
        private BehaviorTree()
        {
        }
        
        /// <summary>
        /// Contains all the abstract information about the database behaviors in this tree
        /// </summary>
        public BehaviorInfo[] BehaviorIds { get; private set; }

        private bool _deserialized;
        
        /// <summary>
        /// Whether this tree is deserialized
        /// </summary>
        /// <exception cref="InvalidOperationException">If you try to deserialize the tree when it's already serialized</exception>
        private bool Deserialized
        {
            get => _deserialized;
            set
            {
                if (Serialized && value)
                    throw new InvalidOperationException("Can't deserialize tree, tree is already serialized.");
                _deserialized = value;
            }
        }

        private bool _serialized = false;

        /// <summary>
        /// Whether this tree is serialized
        /// </summary>
        /// <exception cref="InvalidOptionException">If you try to serialize the tree when it's already deserialized</exception>
        private bool Serialized
        {
            get => _serialized;
            set
            {
                if (Deserialized && value)
                    throw new InvalidOperationException("Can't serialize tree, tree is already deserialized.");
                _serialized = value;
            }
        }

        /// <summary>
        /// All the implemented behaviors in this tree, stored by ID
        /// </summary>
        public Dictionary<int, BehaviorBase> SkillRoots { get; set; } = new Dictionary<int, BehaviorBase>();

        /// <summary>
        /// All the behaviors in this tree, grouped by SkillCastType
        /// </summary>
        private Dictionary<SkillCastType, List<BehaviorExecution>> RootBehaviors { get; } =
            new Dictionary<SkillCastType, List<BehaviorExecution>>();

        private static Dictionary<BehaviorTemplateId, Type> _behaviors;

        private SkillCastType CastType;
        
        /// <summary>
        /// Map of all implemented behaviors, ordered by behavior template id
        /// </summary>
        public static Dictionary<BehaviorTemplateId, Type> Behaviors
        {
            get
            {
                if (_behaviors != default) return _behaviors;
                _behaviors = new Dictionary<BehaviorTemplateId, Type>();

                // Get all implemented behaviors for the assembly
                var behaviors = typeof(BehaviorBase).Assembly.GetTypes().Where(t =>
                    t.BaseType != null
                    && t.BaseType.IsGenericType
                    && t.BaseType.GetGenericTypeDefinition() == typeof(BehaviorBase<>)
                    || t.BaseType != null
                    && t.BaseType == typeof(BehaviorBase)
                    && t != typeof(BehaviorBase)
                    && !t.IsGenericType).ToArray();

                // Store all found behaviors by their skill ID
                foreach (var behavior in behaviors)
                {
                    var behaviorInstance = Activator.CreateInstance(behavior);
                    if (behaviorInstance == default)
                    {
                        Logger.Error($"Could not create instance of behavior of type {behavior}.");
                        continue;
                    }
                    _behaviors.Add(((BehaviorBase) behaviorInstance).Id, behavior);
                }

                return _behaviors;
            }
        }

        /// <summary>
        /// Builds a behavior tree from a skill set by finding and initializing all the skills
        /// that belong to that skill set
        /// </summary>
        /// <param name="skillSetId">The ID of the skill set</param>
        /// <returns>The behavior tree</returns>
        public static async Task<BehaviorTree> FromSkillSetAsync(int skillSetId)
        {
            var skillSetSkills = await ClientCache.FindAllAsync<ItemSetSkills>(skillSetId);

            // Create the behavior tree from all the skills in the skill set
            var tree = new BehaviorTree
            {
                BehaviorIds = await Task.WhenAll(skillSetSkills.Select(async skillSetSkill =>
                {
                    var behavior = await ClientCache.FindAsync<SkillBehavior>(skillSetSkill.SkillID);
                    return new BehaviorInfo
                    {
                        BaseBehavior = behavior?.BehaviorID ?? 0,
                        CastType = skillSetSkill.SkillCastType.HasValue
                            ? (SkillCastType) skillSetSkill.SkillCastType
                            : SkillCastType.OnEquip,
                        SkillId = skillSetSkill.SkillID ?? 0
                    };
                }))
            };

            await tree.BuildAsync();
            return tree;
        }

        public static async Task<BehaviorTree> FromLotAsync(Lot lot)
        {
            var tree = new BehaviorTree();

            var objectSkills = await ClientCache.FindAllAsync<ObjectSkills>(lot);
            tree.BehaviorIds = new BehaviorInfo[objectSkills.Length];

            for (var index = 0; index < objectSkills.Length; index++)
            {
                var objectSkill = objectSkills[index];
                var behavior = await ClientCache.FindAsync<SkillBehavior>(objectSkill.SkillID);

                if (behavior == default)
                {
                    Logger.Error($"Could not find behavior for skill: {objectSkill.SkillID}");
                    continue;
                }

                Logger.Information($"[{lot}] SKILL: {objectSkill.SkillID} -> {behavior.BehaviorID}");

                tree.BehaviorIds[index] = new BehaviorInfo
                {
                    BaseBehavior = behavior.BehaviorID ?? 0,
                    CastType = objectSkill.CastOnType.HasValue ? (SkillCastType) objectSkill.CastOnType : SkillCastType.OnUse,
                    SkillId = objectSkill.SkillID ?? 0,
                    ImaginationCost = behavior.Imaginationcost ?? 0
                };
            }
            
            await tree.BuildAsync();
            return tree;
        }

        /// <summary>
        /// Takes a skill ID and creates a simple skill tree with database information about that behavior skill
        /// </summary>
        /// <param name="skillId">The skill ID to use</param>
        /// <returns>A behavior tree that contains database information about the skill belonging to the skill ID</returns>
        public static async Task<BehaviorTree> FromSkillAsync(int skillId)
        {
            var tree = new BehaviorTree();
            
            // Try to find the base behavior linked to this skill
            var behavior = await BaseBehaviorForSkill(skillId);
            if (behavior?.BehaviorID == default)
                return tree;

            tree.BehaviorIds = new[]
            {
                new BehaviorInfo
                {
                    SkillId = skillId,
                    CastType =  SkillCastType.Default,
                    BaseBehavior = behavior.BehaviorID.Value,
                    ImaginationCost = behavior.Imaginationcost ?? 0
                }
            };

            await tree.BuildAsync();
            return tree;
        }

        /// <summary>
        /// Finds the root behavior associated with a given skill by looking in the SkillBehavior table and returns
        /// database information regarding that root behavior.
        /// </summary>
        /// <param name="skillId">The skill ID to find a behavior for</param>
        /// <returns>The skill behavior if it existed, <c>default</c> otherwise</returns>
        private static async Task<SkillBehavior> BaseBehaviorForSkill(int skillId)
        {
            var skillBehavior = await ClientCache.FindAsync<SkillBehavior>(skillId);
            if (skillBehavior == default)
            {
                Logger.Error($"Could not find behavior for skill: {skillId}.");
            }
            else if (skillBehavior.BehaviorID == default)
            {
                Logger.Error($"{skillId} has an invalid behavior id.");
            }
            
            return skillBehavior;
        }

        /// <summary>
        /// Builds the skill tree by taking all database information about the behaviors in the tree and turning them
        /// into implemented classes for each respective database behavior.
        /// </summary>
        private async Task BuildAsync()
        {
            // Build the base behavior for each requested skill
            foreach (var skill in BehaviorIds)
            {
                var root = BehaviorBase.Cache.ToArray().FirstOrDefault(
                    b => b.BehaviorId == skill.BaseBehavior);

                // If the behavior can't be found in the cache, build it from scratch using its template
                if (root == default)
                {
                    root = await BehaviorFromInfo(skill);
                    if (root == null)
                    {
                        continue;
                    }
                }

                // For each skill, store the fresh behavior root
                SkillRoots[skill.SkillId] = root;

                var executionPreparation = new BehaviorExecution()
                {
                    BehaviorBase = root
                };
                
                // Also store each behavior by CastType
                if (RootBehaviors.TryGetValue(skill.CastType, out var list))
                {
                    list.Add(executionPreparation);
                }
                else
                {
                    RootBehaviors[skill.CastType] = new List<BehaviorExecution>
                        { executionPreparation };
                }
            }
        }

        /// <summary>
        /// Finds and builds a behavior implementation using behavior database information.
        /// </summary>
        /// <remarks>
        /// Also adds this behavior implementation to the cache
        /// </remarks>
        /// <param name="context">Reusable context to query from</param>
        /// <param name="info">The behavior info to get the skill from</param>
        /// <returns>The instantiated behavior base if succeeded, <c>null</c> otherwise</returns>
        private static async Task<BehaviorBase> BehaviorFromInfo(BehaviorInfo info)
        {
            var behavior = (await ClientCache.FindAsync<BehaviorTemplate>(info.BaseBehavior));
            if (behavior?.TemplateID == null)
                return null;
            
            var behaviorTypeId = (BehaviorTemplateId) behavior.TemplateID;

            if (!Behaviors.TryGetValue(behaviorTypeId, out var behaviorType))
            {
                Logger.Error($"No behavior type of \"{behaviorTypeId}\" found.");
                return null;
            }

            return await BehaviorBase.BuildBranch(info.BaseBehavior);
        }

        /// <summary>
        /// Serializes the tree by preparing all the behaviors given the context
        /// </summary>
        /// <param name="associate">The owner of the behaviors</param>
        /// <param name="writer">Writer for bitstream</param>
        /// <param name="skillId">The skill to serialize</param>
        /// <param name="syncId">SyncID to use for subsequent sync skills</param>
        /// <param name="calculatingPosition">Position to use as base for position-based behaviors</param>
        /// <param name="target">Optional target for this behavior</param>
        /// <returns>Serialized behavior given the context</returns>
        public NpcExecutionContext Serialize(GameObject associate, BitWriter writer, int skillId,
            uint syncId, Vector3 calculatingPosition, GameObject target = default)
        {
            Serialized = true;
            CastType = SkillCastType.Default;
            target ??= associate;

            var context = new NpcExecutionContext(associate, skillId, syncId, calculatingPosition);
            if (!SkillRoots.TryGetValue(skillId, out var root))
            {
                Logger.Debug($"Failed to find skill: {skillId}");
                return context;
            }
            
            context.Root = root;
            var parameters = context.Root.SerializeStart(writer, context, new ExecutionBranchContext { Target = target });

            // Setup the behavior for execution
            RootBehaviors[CastType] = new List<BehaviorExecution>()
            {
                new BehaviorExecution
                {
                    BehaviorBase = context.Root,
                    BehaviorExecutionParameters = parameters
                }
            };
            
            return context;
        }

        /// <summary>
        /// Deserializes the entire skill tree by taking a reader and parsing that into behaviors
        /// </summary>
        /// <param name="associate">The caster of the skill</param>
        /// <param name="reader">The reader to parse the behaviors from</param>
        /// <param name="castType">The specific cast type of this skill</param>
        /// <param name="target">An explicit initial target for each root skill</param>
        /// <returns>The shared execution context that was created for all behaviors</returns>
        public ExecutionContext Deserialize(GameObject associate, BitReader reader,
            SkillCastType castType = SkillCastType.OnEquip, GameObject target = default)
        {
            Deserialized = true;
            CastType = castType;
            
            var context = new ExecutionContext(associate);

            DeserializeRootBehaviorsForSkillType(reader, SkillCastType.Default, context, target);
            DeserializeRootBehaviorsForSkillType(reader, castType, context, target);

            return context;
        }

        /// <summary>
        /// Deserializes a this behavior tree for a certain skill type by taking a reader and parsing it into
        /// behaviors
        /// </summary>
        /// <param name="skillType">The skill type to deserialize for</param>
        /// <param name="context">The execution context to pass to all behaviors</param>
        /// <param name="target">An optional explicit target to set as initial target</param>
        private void DeserializeRootBehaviorsForSkillType(BitReader reader, SkillCastType skillType,
            ExecutionContext context, GameObject target = default)
        {
            if (RootBehaviors.TryGetValue(skillType, out var rootBehaviorList))
            {
                foreach (var executionPreparation in rootBehaviorList)
                {
                    // For each behavior specify behavior specific context (for example duration and explicit target)
                    context.Root = executionPreparation.BehaviorBase;

                    // Prepare the behavior for execution by populating it with context
                    executionPreparation.BehaviorExecutionParameters = 
                        executionPreparation.BehaviorBase.DeserializeStart(reader, context,
                            new ExecutionBranchContext
                        {
                            Target = target,
                        });
                }
            }
        }

        /// <summary>
        /// Executes all the behaviors in this tree that correspond to a certain root skill type
        /// </summary>
        /// <param name="skillType">The skill type to execute behaviors for</param>
        private void ExecuteRootBehaviorsForSkillType(SkillCastType skillType)
        {
            if (RootBehaviors.TryGetValue(skillType, out var rootBehaviorList))
            {
                foreach (var executionPreparation in rootBehaviorList)
                {
                    executionPreparation.BehaviorBase.ExecuteStart(executionPreparation.BehaviorExecutionParameters);
                    
                    // If a behavior is serialized (server side) we have to manually sync it
                    if (Serialized)
                    {
                        using var stream = new MemoryStream();
                        var writer = new BitWriter(stream);
                        
                        executionPreparation.BehaviorBase.SerializeSync(writer, executionPreparation.BehaviorExecutionParameters);
                        executionPreparation.BehaviorBase.ExecuteSync(executionPreparation.BehaviorExecutionParameters);
                    }
                }
            }
        }
        
        /// <summary>
        /// Executes the tree by executing all skills of the default and casttype skill type
        /// </summary>
        /// <exception cref="InvalidOperationException">If the tree isn't (de)serialized</exception>
        public void Execute()
        {
            if (!(Serialized || Deserialized))
                throw new InvalidOperationException("Can't execute tree: it's neither serialized nor deserialized.");

            ExecuteRootBehaviorsForSkillType(SkillCastType.Default);
            
            if (CastType != SkillCastType.Default)
                ExecuteRootBehaviorsForSkillType(CastType);
        }

        /// <summary>
        /// Uses the skill tree by starting all the skills of the OnUse skill type
        /// </summary>
        /// <exception cref="InvalidOperationException">If the tree isn't deserialized</exception>
        public void Use()
        {
            if (!Deserialized)
                throw new InvalidOperationException("Can't use behavior: tree is not deserialized.");
            if (!RootBehaviors.TryGetValue(SkillCastType.OnUse, out var list))
                return;
         
            foreach (var behaviorExecution in list)
            {
                behaviorExecution.BehaviorBase.ExecuteStart(behaviorExecution.BehaviorExecutionParameters);
            }
        }

        /// <summary>
        /// Mounts the skill tree by starting all the skills of the equip skill type
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the tree isn't deserialized</exception>
        public void Mount()
        {
            if (!Deserialized)
                throw new InvalidOperationException("Can't mount tree: tree is not deserialized.");
            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list))
                return;
            
            foreach (var executionPreparation in list)
            {
                executionPreparation.BehaviorBase.ExecuteStart(executionPreparation
                    .BehaviorExecutionParameters);
            }
        }

        /// <summary>
        /// Dismantles the skill tree by calling dismantle on all equip skills.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the tree isn't deserialized</exception>
        public void Dismantle()
        {
            if (!Deserialized)
                throw new InvalidOperationException("Can't dismantle tree: tree is not deserialized.");
            if (!RootBehaviors.TryGetValue(SkillCastType.OnEquip, out var list)) 
                return;

            foreach (var executionPreparation in list)
            {
                executionPreparation.BehaviorBase.Dismantle(executionPreparation
                    .BehaviorExecutionParameters);
            }
        }
    }
}