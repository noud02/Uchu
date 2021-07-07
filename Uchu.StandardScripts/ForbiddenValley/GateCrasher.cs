using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.ForbiddenValley
{
    [ZoneSpecific(1400)]
    public class GateCrasher : NativeScript
    {
        public override Task LoadAsync()
        {

            var gameObjects = HasLuaScript("l_act_bounce_over_wall.lua");

            foreach (var gameObject in gameObjects)
            {
                // Load physics object information from cdclient
                var phantomPhysicsComponentId = gameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                var cdcComponent = ClientCache.Find<Core.Client.PhysicsComponent>(phantomPhysicsComponentId);
                var assetPath = cdcComponent?.Physicsasset;

                // Configure physics object
                var physics = gameObject.AddComponent<PhysicsComponent>();
                physics.SetPhysicsByPath(assetPath);

                // Listen for players entering
                Listen(physics.OnEnter, async other =>
                {
                    if (!(other.GameObject is Player player)) return;

                    if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;

                    await missionInventoryComponent.ScriptAsync(1241, gameObject.Lot);
                });
            }

            return Task.CompletedTask;
        }
    }
}