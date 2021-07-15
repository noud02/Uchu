using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Systems.AI;
using Uchu.World.Systems.Behaviors;

namespace Uchu.World
{
    public class BaseCombatAiComponent : ReplicaComponent
    {
        public bool PerformingAction { get; set; }

        public CombatAiAction Action { get; set; }

        public GameObject Target { get; set; }

        public override ComponentId Id => ComponentId.BaseCombatAIComponent;

        /// <summary>
        /// Cooldown that's set after a skill is executed, equal to <c>Cooldown</c> but not decremented on tick
        /// </summary>
        private float FrozenCooldown { get; set; }
        
        /// <summary>
        /// Cooldown that's set after a skill is executed, this will be decremented on each tick
        /// </summary>
        private float Cooldown { get; set; }

        public bool AbilityDowntime { get; set; }

        public List<NpcSkillEntry> SkillEntries { get; set; }

        private SkillComponent SkillComponent { get; set; }

        private DestructibleComponent DestructibleComponent { get; set; }

        private QuickBuildComponent QuickBuildComponent { get; set; }

        private DestroyableComponent Stats { get; set; }

        public bool Enabled { get; set; } = true;

        public BaseCombatAiComponent()
        {
            Listen(OnStart, () =>
            {
                SkillEntries = new List<NpcSkillEntry>();

                Listen(GameObject.OnStart, async () =>
                {
                    SkillComponent = GameObject.GetComponent<SkillComponent>();
                    DestructibleComponent = GameObject.GetComponent<DestructibleComponent>();
                    QuickBuildComponent = GameObject.GetComponent<QuickBuildComponent>();
                    Stats = GameObject.GetComponent<DestroyableComponent>();

                    foreach (var skillEntry in SkillComponent.DefaultSkillSet)
                    {
                        var skillInfo = (await ClientCache.GetTableAsync<SkillBehavior>()).First(
                            s => s.SkillID == skillEntry.SkillId
                        );
                        
                        SkillEntries.Add(new NpcSkillEntry
                        {
                            SkillId = skillEntry.SkillId,
                            Cooldown = 0,
                            AbilityCooldown = (skillInfo.Cooldown ?? 1) * 1000,
                            Tree = await BehaviorTree.FromSkillAsync((int)skillEntry.SkillId)
                        });
                    }

                    Zone.Update(GameObject, delta => CalculateCombat(delta), 1);
                });
            });
        }

        /// <summary>
        /// Calculates the combat for an AI
        /// </summary>
        /// <param name="delta">Passed time in milliseconds since last tick</param>
        private Task CalculateCombat(float delta)
        {
            if (!Enabled 
                || !DestructibleComponent.Alive 
                || QuickBuildComponent != default && QuickBuildComponent.State != RebuildState.Completed)
                return Task.CompletedTask;

            if (Cooldown <= 0)
            {
                AbilityDowntime = false;
                var elapsedCooldown = FrozenCooldown;
                
                Cooldown = 1000f;
                FrozenCooldown = Cooldown;

                foreach (var entry in SkillEntries)
                {
                    entry.Cooldown -= elapsedCooldown;
                    if (entry.Cooldown > 0 || AbilityDowntime)
                        continue;

                    var time = SkillComponent.CalculateSkill(entry.Tree, (int) entry.SkillId);
                    if (time == 0)
                        continue;

                    AbilityDowntime = true;
                    entry.Cooldown = entry.AbilityCooldown + time;
                    
                    Cooldown += time;
                    FrozenCooldown = Cooldown;
                }
            }

            Cooldown -= delta;

            return Task.CompletedTask;
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(PerformingAction);

            if (!PerformingAction) return;

            writer.Write((uint) Action);
            writer.Write(Target);
        }

        public GameObject[] SeekValidTargets()
        {
            // TODO: Optimize

            if (Stats.Factions.Length == default)
                return new GameObject[0];

            var entries = Zone.Objects.OfType<DestroyableComponent>();
            var targets = new List<GameObject>();

            foreach (var entry in entries
                .Where(e => e.Factions.Length != default && e.Health > 0))
            {
                if (entry.GameObject.TryGetComponent<TriggerComponent>(out _))
                    continue;
                
                if (Stats.Enemies.Contains(entry.Factions.First()) && !Stats.Friends.Contains(entry.Factions.First()))
                {
                    targets.Add(entry.GameObject);
                }
            }

            return targets.ToArray();
        }
    }
}