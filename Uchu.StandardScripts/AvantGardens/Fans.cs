using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using DestructibleComponent = Uchu.World.DestructibleComponent;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class Fans : NativeScript
    {
        public override Task LoadAsync()
        {
            var ids = new[]
            {
                (19, 1),
                (19, 2),
                (20, 1)
            };
            
            foreach (var trigger in GetTriggers(ids))
            {
                var gameObject = trigger.GameObject;
                
                if (!gameObject.TryGetComponent<SwitchComponent>(out var switchComponent)) continue;
                
                gameObject.TryGetComponent<QuickBuildComponent>(out var buttonQuickBuild);

                var fanId = trigger.Trigger.Events[0].Commands[0].TargetName;
                
                var fanObject = GetFan(fanId);
                
                fanObject.TryGetComponent<DestructibleComponent>(out var fanDestructibleComponent);
                
                if (fanDestructibleComponent == default) continue;
                
                Listen(switchComponent.OnActivated, player =>
                {
                    UpdateFanState(fanObject,switchComponent,fanDestructibleComponent);
                });
                
                Listen(switchComponent.OnDeactivated, () =>
                {
                    UpdateFanState(fanObject,switchComponent,fanDestructibleComponent);
                });
                
                Listen(fanDestructibleComponent.OnSmashed, (_,__) =>
                {
                    UpdateFanState(fanObject,switchComponent,fanDestructibleComponent);
                });
                
                Listen(gameObject.OnInteract, player =>
                {
                    if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return;
                    
                    switchComponent.Activate(player);
                    
                    Serialize(gameObject);
                });
                
                Listen(Zone.OnTick, () =>
                {
                    if (switchComponent.State) return;
                    
                    foreach (var player in Zone.Players)
                    {
                        if (player?.Transform == default) return;
                        
                        if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return;
                        
                        if (!(Vector3.Distance(player.Transform.Position, gameObject.Transform.Position) < 2)) continue;

                        switchComponent.Activate(player);

                        Serialize(gameObject);
                    }
                });
                
                DeactivateFx(fanObject);

                Listen(Zone.OnObject, (newObject) =>
                {
                    if (!(newObject is GameObject newFanObject)) return;

                    if (!IsCorrectFan(newFanObject,fanId)) return;

                    fanObject = newFanObject;
                    
                    fanObject.TryGetComponent<DestructibleComponent>(out fanDestructibleComponent);
                
                    if (fanDestructibleComponent == default) return;
                    
                    Listen(fanDestructibleComponent.OnSmashed, (_,__) =>
                    {
                        UpdateFanState(fanObject,switchComponent,fanDestructibleComponent);
                    });
                    
                    UpdateFanState(fanObject,switchComponent,fanDestructibleComponent);
                });

                Listen(Zone.OnPlayerLoad, player =>
                {
                    Listen(player.OnReadyForUpdatesEvent, message =>
                    {
                        if (message.GameObject != fanObject) return;
                        
                        if (GetFanState(switchComponent,fanDestructibleComponent))
                        {
                            player.Message(new PlayFXEffectMessage
                            {
                                Associate = fanObject,
                                EffectId = 495,
                                EffectType = "fanOn",
                                Name = "fanOn",
                                Priority = 1,
                                Scale = 1,
                                Serialize = true,
                            });
                            
                            player.Message(new PlayAnimationMessage
                            {
                                Associate = fanObject,
                                AnimationId = "fan-on",
                                PlayImmediate = true,
                                Priority = 0.4f,
                                Scale = 1,
                            });
                        }
                        else
                        {
                            player.Message(new PlayAnimationMessage
                            {
                                Associate = fanObject,
                                AnimationId = "fan-off",
                                PlayImmediate = true,
                                Priority = 0.4f,
                                Scale = 1,
                            });
                        }
                    });
                });
            }
            
            return Task.CompletedTask;
        }

        private GameObject GetFan(string fanId)
        {
            foreach (var zoneGameObject in Zone.GameObjects)
            {
                if (!IsCorrectFan(zoneGameObject, fanId)) continue;

                return zoneGameObject;
            }

            return null;
        }

        private bool IsCorrectFan(GameObject gameObject,string fanId)
        {
            if (!gameObject.Settings.TryGetValue("groupID", out var groupObj)) return false;

            var groupId = (string) groupObj;

            return groupId == $"{fanId};" && gameObject.ToString().Contains("Fan");
        }

        private bool GetFanState(SwitchComponent switchComponent, DestructibleComponent fanDestructibleComponent)
        {
            return !switchComponent.State && fanDestructibleComponent.Alive;
        }
        
        private void UpdateFanState(GameObject fanObject, SwitchComponent switchComponent, DestructibleComponent fanDestructibleComponent)
        {
            if (!GetFanState(switchComponent,fanDestructibleComponent))
            {
                ActivateFx(fanObject);
            }
            else
            {
                DeactivateFx(fanObject);
            }
        }

        private void ActivateFx(GameObject gameObject)
        {
            var group = (gameObject.Settings["groupID"] as string)?.Split(';')[0];
            
            if (group == default) return;

            gameObject.Animate("fan-off", true);

            gameObject.StopFX("fanOn");

            foreach (var fanObject in GetGroup(group))
            {
                // Nothing is found here
                if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                    
                physicsComponent.IsEffectActive = false;
                
                Serialize(fanObject);
            }

            GetGroup($"{group}fx")[0].Animate("trigger", true);
        }

        private void DeactivateFx(GameObject gameObject)
        {
            var group = (gameObject.Settings["groupID"] as string)?.Split(';')[0];
            
            if (group == default) return;

            gameObject.Animate("fan-on", true);

            gameObject.PlayFX("fanOn", "fanOn", 495);

            foreach (var fanObject in GetGroup(group))
            {
                // Nothing is found here
                if (!fanObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) continue;
                
                physicsComponent.IsEffectActive = true;

                Serialize(fanObject);
            }

            GetGroup($"{group}fx")[0].Animate("idle", true);
        }
    }
}