using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Core;
using InfectedRose.Lvl;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Systems.Missions;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1587_script_name__removed")]
    public class SkeletonTower : ObjectScript
    {
        public SkeletonTower(GameObject towerObject) : base(towerObject)
        {
            var legs = new List<GameObject>();

            var radius = (float) towerObject.Settings["hort_offset"];

            for (var i = 0; i < 3; i++)
            {
                // Calculate leg position
                var origin = towerObject.Transform.Position;
                var towerRotation = towerObject.Transform.EulerAngles.Y / 360 * Math.Tau;
                const double rotationOffset = -1f / 4 * Math.Tau;
                var relativePosition = new Vector3(
                    radius * (float) Math.Cos((float) i / 4 * Math.Tau - towerRotation + rotationOffset),
                    0,
                    radius * (float) Math.Sin((float) i / 4 * Math.Tau - towerRotation + rotationOffset));
                var position = origin + relativePosition;

                var legSpawnerObject = GameObject.Instantiate(
                    Zone,
                    "SkeletonTowerLegSpawner",
                    position,
                    Quaternion.Identity,
                    -1,
                    ObjectId.Standalone,
                    Lot.Spawner
                );

                // Set leg orientation
                legSpawnerObject.Transform.LookAt(towerObject.Transform.Position);

                var legSpawnerComponent = legSpawnerObject.AddComponent<SpawnerComponent>();
                legSpawnerComponent.SpawnTemplate = new Lot((int) towerObject.Settings["legLOT"]);
                // this time is a wild guess
                legSpawnerComponent.RespawnTime = 50 * 1000;

                Start(legSpawnerObject);

                // Spawn leg
                legSpawnerComponent.Spawn();

                // Keep track of leg spawners
                // legSpawnerComponents.Add(legSpawnerComponent);
                legs.Add(legSpawnerObject);

                // Listen for legs being smashed
                Listen(legSpawnerComponent.OnRespawnInitiated, player =>
                {
                    // Return if any leg is alive
                    if (legs.Any(leg => leg.GetComponent<SpawnerComponent>().HasActiveSpawn))
                        return;

                    // Arriving here in the code means all legs are smashed;
                    // the tower should fall down and explode.

                    // Legs should not respawn legs until tower respawns.
                    // Destroy current leg spawners
                    foreach (var legSpawner in legs.ToArray())
                    {
                        Destroy(legSpawner);
                    }

                    // TODO: Move tower to the ground
                    // There is no MovingPlatform component, but the object type is MovingPlatforms.
                    // Not sure how to handle this.
                    // (https://lu.lcdruniverse.org/explorer/objects/15953)

                    // Damage tower, destroying it
                    Task.Run(() =>
                    {
                        towerObject.GetComponent<DestroyableComponent>().Damage(1, legSpawnerObject);
                    });

                    // Spawn enemy and schedule removal when tower respawns
                    var enemy = GameObject.Instantiate(new LevelObjectTemplate
                    {
                        Lot = (int) towerObject.Settings["enemyToSpawn"],
                        Position = towerObject.Transform.Position,
                        Rotation = towerObject.Transform.Rotation,
                        Scale = 1,
                        LegoInfo = new LegoDataDictionary(),
                    }, player.Zone);

                    World.Object.Start(enemy);
                    GameObject.Construct(enemy);

                    Zone.Schedule(() =>
                    {
                        if (enemy.Alive)
                            Destroy(enemy);
                    }, 1000 * (float) towerObject.Settings["respawn"]);

                    // Progress missions. Object LDF contains missions=0:1806_2032
                    var missions = ((string) towerObject.Settings["missions"])
                        .Split('_').Select(int.Parse);
                    if (player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                        foreach (var mission in missions)
                        {
                            if (!missionInventoryComponent.HasActive(mission))
                                continue;
                            var task = (ScriptTask) missionInventoryComponent.GetMission(mission).Tasks
                                .FirstOrDefault(m => m.Target == towerObject.Lot);
                            task?.ReportProgress(task.TaskId, task.Target);
                        }
                });
            }
        }
    }
}
