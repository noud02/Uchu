using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.StandardScripts;
using System.IO;
using System;
using InfectedRose.Luz;
using System.Numerics;
using Uchu.World.Client;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BlockYardProperty : NativeScript
    {
        static private string GUIDMaelstrom { get; } = "{7881e0a1-ef6d-420c-8040-f59994aa3357}"; // ambient sounds for when the Maelstrom is on
        static private string GUIDPeaceful { get; } = "{c5725665-58d0-465f-9e11-aeb1d21842ba}"; // happy ambient sounds when no Maelstrom is present
        static bool HasOwner { get; set; } = true;
        static private string[] GlobalObjects { get; } = // Objects you always spawn
        {
            "Mailbox",
            "PropertyGuard",
            "Launcher"
        };
        static private string[] MaelstromObjects { get; } = // Objects needed for Maelstrom battle
        {
            "DestroyMaelstrom",
            "SpiderBoss",
            "SpiderEggs",
            "Rocks",
            "DesMaelstromInstance",
            "Spider_Scream",
            "ROF_Targets_00",
            "ROF_Targets_01",
            "ROF_Targets_02",
            "ROF_Targets_03",
            "ROF_Targets_04"
        };
        static private string[] PeacefulObjects { get; } = // Objects needed for the property
        {
            "SunBeam",
            "BankObj",
            "AGSmallProperty"
        };
        private GameObject[] SpiderEggs { get; set; } = { };
        private GameObject SpiderQueen { get; set; }
        private Player Player { get; set; }

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, (LocalPlayer) =>
            {
                Player = LocalPlayer;
                if (HasOwner) // If the world is peaceful
                {
                    Peaceful(Player);
                }
                else
                {
                    Maelstrom(Player);
                }
            });

            return Task.CompletedTask;
        }

        private void Peaceful(Player player)
        {
            Player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDPeaceful
            }); // Play peaceful sounds

            SpawnPaths(false);

            foreach (var item in Zone.GameObjects.Where(i => i.Lot == 9938))
            {
                Player.Message(new StopFXEffectMessage
                {
                    Name = "beam",
                    Associate = item,
                    KillImmediate = false
                });

                Player.Message(new DieMessage
                {
                    Associate = item,
                    ClientDeath = true,
                    SpawnLoot = true,
                    DeathType = "",
                    DirectionRelativeAngleXz = 0.00f,
                    DirectionRelativeAngleY = 0.00f,
                    DirectionRelativeForce = 0.00f,
                    KillType = 1,
                    Killer = default,
                    LootOwner = default
                });
            }
        }

        private void Maelstrom(Player player)
        {
            Player.Message(new PlayNDAudioEmitterMessage
            {
                Associate = player,
                NDAudioEventGUID = GUIDMaelstrom
            }); // Play eerie sounds

            SpawnPaths(true);

            foreach (var item in Zone.GameObjects)
            {
                if (item.Lot.Id == 14375)
                {
                    SpiderEggs.Append(item); // Spider Queen Eggs
                }

                if (item.Lot.Id == 9524) Destroy(item); // Build Border (We don't need it atm)

                if (item.Lot.Id == 14381) // Spider Queen
                {
                    item.RemoveComponent<BaseCombatAiComponent>(); // Once the Combat AI is fixed we can remove this
                    SpiderQueen = item;
                }

                if (item.Lot.Id == 9938) // FX Object
                {
                    // Start all the FX

                    Player.Message(new PlayFXEffectMessage
                    {
                        Name = "TornadoDebris",
                        EffectType = "debrisOn",
                        Associate = item
                    });
                    Player.Message(new PlayFXEffectMessage
                    {
                        Name = "TornadoVortex",
                        EffectType = "VortexOn",
                        Associate = item
                    });
                    Player.Message(new PlayFXEffectMessage
                    {
                        Name = "silhouette",
                        EffectType = "onSilhouette",
                        Associate = item
                    });
                }
            }

            Listen(Player.OnFireServerEvent, (name, message) =>
            {
                if (message.Arguments == "CleanupSpiders")
                {
                    CleanupSpiders();
                }
            });
        }

        private void CleanupSpiders()
        {
            foreach (var item in Zone.GameObjects.Where(i => i.Lot == 16197))
            {
                Destroy(item);
            } // Deletes any spiders in the scene
        }

        private void WithdrawSpider(Player Player, bool Withdraw) // This removes the spider boss from the arena
        {
            if (Withdraw)
            {
                Player.Message(new SetStunnedMessage
                {
                    Associate = SpiderQueen,
                    CantMove = true,
                    CantJump = true,
                    CantAttack = true,
                    CantTurn = true,
                    CantUseItem = true,
                    CantEquip = true,
                    CantInteract = true
                }); // The animation handles movement

                SpiderQueen.Transform.Rotate(new Quaternion { X = 0.0f, Y = -0.005077f, Z = 0.0f, W = 0.999f }); // Orientation for the animation to make sense

                SpiderQueen.Animate("withdraw");
                
                using var cdClient = new CdClientContext();
                var Animation = cdClient.AnimationsTable.FirstOrDefault(
                    a => a.Animationname == "withdraw"
                );

                float LengthMs = ((Animation.Animationlength ??= 2.5f) - 0.25f) * 1000;

                Player.Message(new SetStatusImmunityMessage
                {
                    Associate = SpiderQueen,
                    state = ImmunityState.Push,
                    bImmuneToSpeed = true,
                    bImmuneToBasicAttack = true,
                    bImmuneToDOT = true
                });
            }
        }

        private void SpawnPaths(bool IsMaelstrom)
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (IsMaelstrom ? MaelstromObjects.Contains(path.PathName) : PeacefulObjects.Contains(path.PathName) || GlobalObjects.Contains(path.PathName))
                    {

                        var obj = InstancingUtilities.Spawner(path, Zone);

                        if (obj == null) return;

                        obj.Layer = StandardLayer.Hidden;

                        var spawner = obj.GetComponent<SpawnerComponent>();

                        spawner.SpawnsToMaintain = (int)path.NumberToMaintain;

                        spawner.SpawnLocations = path.Waypoints.Select(w => new SpawnLocation
                        {
                            Position = w.Position,
                            Rotation = Quaternion.Identity
                        }).ToList();

                        Start(obj);

                        spawner.SpawnCluster();
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e);
                }
            }
        }

        private void DestroyMaelstrom()
        {
            foreach (var path in Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    if (MaelstromObjects.Contains(path.PathName))
                    {

                        var obj = InstancingUtilities.Spawner(path, Zone);

                        if (obj == null) return;

                        obj.Layer = StandardLayer.Hidden;

                        var spawner = obj.GetComponent<SpawnerComponent>();

                        spawner.SpawnsToMaintain = (int)path.NumberToMaintain;

                        spawner.SpawnLocations = path.Waypoints.Select(w => new SpawnLocation
                        {
                            Position = w.Position,
                            Rotation = Quaternion.Identity
                        }).ToList();

                        Start(obj);

                        spawner.SpawnCluster();
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e);
                }
            }

            foreach (var item in Zone.GameObjects.Where(i => i.Lot == 9938))
            {
                Player.Message(new StopFXEffectMessage
                {
                    Name = "TornadoVortex",
                    Associate = item,
                    KillImmediate = true
                });

                Player.Message(new StopFXEffectMessage
                {
                    Name = "TornadoDebris",
                    Associate = item,
                    KillImmediate = true
                });

                Player.Message(new StopFXEffectMessage
                {
                    Name = "silhouette",
                    Associate = item,
                    KillImmediate = true
                });

                Player.Message(new DieMessage
                {
                    Associate = item,
                    ClientDeath = true,
                    SpawnLoot = true,
                    DeathType = "",
                    DirectionRelativeAngleXz = 0.00f,
                    DirectionRelativeAngleY = 0.00f,
                    DirectionRelativeForce = 0.00f,
                    KillType = 1,
                    Killer = default,
                    LootOwner = default
                });
            }
        }
    }
}