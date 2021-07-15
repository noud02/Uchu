using System;
using System.Numerics;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    public static class InstancingUtilities
    {
        private static readonly Random Random = new Random();

        public static GameObject Spawner(LevelObjectTemplate levelObject, Object parent)
        {
            if (!levelObject.LegoInfo.TryGetValue("spawntemplate", out var spawnTemplate))
            {
                Logger.Error("Instantiating a spawner without a \"spawntemplate\" is not allowed.");
                return null;
            }

            if (spawnTemplate is string s)
            {
                spawnTemplate = int.Parse(s);
            }

            var instance = GameObject.Instantiate(
                parent,
                position: levelObject.Position,
                rotation: levelObject.Rotation,
                scale: -1,
                objectId: (ObjectId) levelObject.ObjectId,
                lot: levelObject.Lot
            );

            instance.Settings = levelObject.LegoInfo;

            if (levelObject.LegoInfo.TryGetValue("trigger_id", out var trigger))
            {
                Logger.Debug($"SPAWN TRIGGER: {trigger}");
            }

            var spawnerComponent = instance.AddComponent<SpawnerComponent>();

            spawnerComponent.SpawnTemplate = new Lot((int) spawnTemplate);
            spawnerComponent.SpawnScale = levelObject.Scale;

            if (levelObject.LegoInfo.TryGetValue("respawn", out var respawnTime))
                spawnerComponent.RespawnTime = Convert.ToInt32(Convert.ToSingle(respawnTime) * 1000);

            return instance;
        }

        public static SpawnerNetwork SpawnerNetwork(LuzSpawnerPath spawnerPath, Zone zone)
        {
            var network = Object.Instantiate<SpawnerNetwork>(zone);

            network.ActivateOnLoad = spawnerPath.ActivateSpawnerNetworkOnLoad;
            network.MaxToSpawn = spawnerPath.MaxSpawnCount;
            network.RespawnTime = spawnerPath.RespawnTime * 1000;
            network.SpawnsToMaintain = spawnerPath.NumberToMaintain;
            network.Name = spawnerPath.PathName;

            foreach (var pathWaypoint in spawnerPath.Waypoints)
            {
                var spawnerWaypoint = (LuzSpawnerWaypoint) pathWaypoint;

                var spawnerSettings = spawnerWaypoint.Configs;

                var spawnerNode = Spawner(new LevelObjectTemplate
                {
                    LegoInfo = spawnerSettings,
                    Position = spawnerWaypoint.Position,
                    Rotation = spawnerWaypoint.Rotation,
                    Scale = -1,
                    Lot = Lot.Spawner,
                    ObjectId = (ObjectId) spawnerPath.SpawnerObjectId,
                }, zone);

                var spawnerComponent = spawnerNode.GetComponent<SpawnerComponent>();
                spawnerComponent.SpawnerNodeId = Convert.ToUInt32((int) spawnerSettings["spawner_node_id"]);
                spawnerComponent.Network = network;

                network.AddSpawnerNode(spawnerComponent);
            }

            return network;
        }

        public static GameObject InstantiateLoot(Lot lot, Player owner, GameObject source, Vector3 spawn)
        {
            if (owner is null) return default;

            try
            {
                var drop = GameObject.Instantiate(
                    owner.Zone,
                    lot,
                    spawn
                );

                drop.Layer = StandardLayer.Hidden;

                var finalPosition = new Vector3
                {
                    X = spawn.X + ((float) Random.NextDouble() % 1f - 0.5f) * 20f,
                    Y = spawn.Y,
                    Z = spawn.Z + ((float) Random.NextDouble() % 1f - 0.5f) * 20f
                };

                owner.Message(new DropClientLootMessage
                {
                    Associate = owner,
                    Currency = 0,
                    Lot = drop.Lot,
                    Loot = drop,
                    Owner = owner,
                    Source = source,
                    SpawnPosition = drop.Transform.Position + Vector3.UnitY,
                    FinalPosition = finalPosition
                });

                return drop;
            }
            catch (Exception e)
            {
                Logger.Error(e);

                return default;
            }
        }
        
        public static void InstantiateCurrency(int currency, Player owner, GameObject source, Vector3 spawn)
        {
            if (owner is null) return;
            if (currency <= 0) return;
            
            try
            {
                var finalPosition = new Vector3
                {
                    X = spawn.X + ((float) Random.NextDouble() % 1f - 0.5f) * 20f,
                    Y = spawn.Y,
                    Z = spawn.Z + ((float) Random.NextDouble() % 1f - 0.5f) * 20f
                };

                owner.Message(new DropClientLootMessage
                {
                    Associate = owner,
                    Currency = currency,
                    Owner = owner,
                    Source = source,
                    SpawnPosition = spawn + Vector3.UnitY,
                    FinalPosition = finalPosition
                });

                var character = owner.GetComponent<CharacterComponent>();
                character.EntitledCurrency += currency;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
