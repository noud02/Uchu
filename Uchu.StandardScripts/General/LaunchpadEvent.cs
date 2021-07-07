using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    public class LaunchpadEvent : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, async (eventName, message) =>
                {
                    if (eventName == "ZonePlayer")
                    {
                        var launchpad = message.Associate.GetComponent<RocketLaunchpadComponent>();
                        var id = launchpad.GameObject.Lot.GetComponentId(ComponentId.RocketLaunchComponent);
                        var launchpadComponent = await ClientCache.FindAsync<RocketLaunchpadControlComponent>(id);

                        if (launchpadComponent.TargetZone != null)
                        {
                            var target = (ZoneId)launchpadComponent.TargetZone;
                            
                            // We don't want to lock up the server on a world server request, as it may take time.
                            var _ = Task.Run(async () =>
                            {
                                player.GetComponent<CharacterComponent>().LaunchedRocketFrom = Zone.ZoneId;
                                var success = await player.SendToWorldAsync(target);

                                if (!success)
                                {
                                    player.SendChatMessage($"Failed to transfer to {target}, please try later.");
                                }
                            });
                        }
                    }
                });

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}