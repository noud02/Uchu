using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Physics;
using Uchu.World.Client;

namespace Uchu.World
{
    public class TriggerComponent : StructReplicaComponent<TriggerSerialize>
    {
        public Trigger Trigger { get; private set; }
        
        public override ComponentId Id => ComponentId.TriggerComponent;

        protected TriggerComponent()
        {
            Listen(OnStart, () =>
            {
                if (!GameObject.Settings.TryGetValue("trigger_id", out var triggerIds)) return;

                Logger.Information($"{GameObject} Attempting trigger: {triggerIds}");
                
                var str = (string) triggerIds;
                var split = str.Split(':');

                if (split.Length != 2)
                {
                    Logger.Error($"{GameObject} Failed to parse trigger: {triggerIds}");
                    
                    return;
                }
                
                var fileId = int.Parse(split[0]);
                var triggerId = int.Parse(split[1]);

                LoadTrigger(fileId, triggerId);
            });
        }

        public void LoadTrigger(int fileId, int triggerId)
        {
            var trigger = Zone.ZoneInfo.TriggerDictionary[fileId, triggerId];
                
            if (trigger == default)
            {
                trigger = Zone.ZoneInfo.TriggerDictionary[triggerId, fileId];

                if (trigger == default)
                {
                    Logger.Error($"{GameObject} Failed to find trigger: {triggerId}:{fileId}");

                    return;
                }
            }
                
            if (trigger.Enabled == 0) return;

            LoadTrigger(trigger);
        }

        public void LoadTrigger(Trigger trigger)
        {
            Trigger = trigger;
            
            Logger.Information($"{GameObject} Trigger: {Trigger.FileId}:{Trigger.Id}");

            foreach (var @event in trigger.Events)
            {
                PhysicsComponent physics;
                switch (@event.Id)
                {
                    case "OnCreate":
                        ExecuteEvent(@event);
                        break;
                    case "OnDestroy":
                        Listen(OnDestroyed, () =>
                        {
                            ExecuteEvent(@event);
                        });
                        break;
                    case "OnEnter":
                    {
                        // Find physics asset path from cdclient
                        var phantomPhysicsComponentId =
                            GameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                        var cdcComponent = ClientCache.GetTable<Core.Client.PhysicsComponent>()
                            .FirstOrDefault(r => r.Id == phantomPhysicsComponentId);
                        var assetPath = cdcComponent?.Physicsasset;

                        // Give physics object correct dimensions
                        physics = GameObject.AddComponent<PhysicsComponent>();
                        physics.SetPhysicsByPath(assetPath);

                        Listen(physics.OnEnter, other => { ExecuteEvent(@event, other.GameObject); });
                        break;
                    }
                    case "OnExit":
                    {
                        // Find physics asset path from cdclient
                        var phantomPhysicsComponentId =
                            GameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                        var cdcComponent = ClientCache.GetTable<Core.Client.PhysicsComponent>()
                            .FirstOrDefault(r => r.Id == phantomPhysicsComponentId);
                        var assetPath = cdcComponent?.Physicsasset;

                        // Give physics object correct dimensions
                        physics = GameObject.AddComponent<PhysicsComponent>();
                        physics.SetPhysicsByPath(assetPath);

                        Listen(physics.OnLeave, other => { ExecuteEvent(@event, other.GameObject); });
                        break;
                    }
                    default:
                        Logger.Error($"Unsupported event type: {@event.Id}!");
                        break;
                }
            }
        }

        private void ExecuteEvent(TriggerEvent @event, params object[] arguments)
        {
            foreach (var command in @event.Commands)
            {
                ExecuteTriggerCommand(command, arguments);
            }
        }

        private void ExecuteTriggerCommand(TriggerCommand command, params object[] arguments)
        {
            Logger.Information($"TRIGGER: {command.Id} -> {string.Join(", ", arguments)}");
            
            switch (command.Id)
            {
                case "SetPhysicsVolumeEffect":
                    SetPhysicsVolumeEffect(command);
                    break;
                case "CastSkill":
                    CastSkill(command, arguments);
                    break;
                case "pushObject":
                    PushObject(command, arguments);
                    break;
                case "repelObject":
                    RepealObject(command, arguments);
                    break;
                case "updateMission":
                    UpdateMission(command, arguments);
                    break;
            }

            GameObject.Serialize(GameObject);
        }

        private void UpdateMission(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;
            if (!target.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;

            var commandArgs = command.Arguments.Split(',');
            if (commandArgs.Length == 0) return;

            switch (commandArgs.First())
            {
                // ReSharper disable once StringLiteralTypo
                case "exploretask":
                    _ = missionInventoryComponent.DiscoverAsync(commandArgs.Last());
                    break;
            }
        }

        private void PushObject(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = new Vector3
                {
                    X = float.Parse(parameters[0]),
                    Y = float.Parse(parameters[1]),
                    Z = float.Parse(parameters[2])
                }
            });
        }

        private void RepealObject(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = forward * float.Parse(parameters[0])
            });
        }

        private void CastSkill(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;
            
            target.SendChatMessage($"Laser!");
            
            var parameters = command.Arguments.Split(',');

            var skill = int.Parse(parameters[0]);

            var skillComponent = GameObject.AddComponent<SkillComponent>();

            var _ = Task.Run(async () =>
            {
                try
                {
                    await skillComponent.CalculateSkillAsync(skill, target);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    throw;
                }
            });
        }

        private void SetPhysicsVolumeEffect(TriggerCommand command)
        {
            if (!GameObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) return;

            var arguments = command.Arguments.Split(',');
                    
            physicsComponent.IsEffectActive = true;

            var effectTypeInfo = typeof(PhantomPhysicsEffectType);

            var effectType = (PhantomPhysicsEffectType) Enum.Parse(effectTypeInfo, arguments[0]);

            physicsComponent.EffectType = effectType;

            var amount = float.Parse(arguments[1]);

            physicsComponent.EffectAmount = amount;

            if (arguments.Length > 2)
            {
                var direction = new Vector3
                {
                    X = float.Parse(arguments[2]),
                    Y = float.Parse(arguments[3]),
                    Z = float.Parse(arguments[4])
                };

                physicsComponent.EffectDirection = direction;
            }
        }
        
        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override TriggerSerialize GetPacket()
        {
            var packet = base.GetPacket();
            if (Trigger != default)
            {
                packet.TriggerId = Trigger.Id;
            }
            return packet;
        }
    }
}