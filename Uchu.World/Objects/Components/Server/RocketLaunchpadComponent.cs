using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.RocketLaunchComponent)]
    public class RocketLaunchpadComponent : Component
    {
        protected RocketLaunchpadComponent()
        {
            Listen(OnStart, () =>
            {
                Listen(GameObject.OnInteract, OnInteract);
            });
        }

        public void OnInteract(Player player)
        {
            var rocket = player.GetComponent<InventoryManagerComponent>()[InventoryType.Models].Items.FirstOrDefault(
                item => item.Lot == Lot.ModularRocket
            );

            if (rocket == default)
            {
                Logger.Error($"Could not find a valid rocket for {player}");
                return;
            }

            rocket.WorldState = ObjectWorldState.Attached;
            
            player.Message(new FireEventClientSideMessage()
            {
                Associate = GameObject,
                Arguments = "RocketEquipped",
                Target = rocket,
                Sender = player
            });

            if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                return;
            characterComponent.LandingByRocket = true;
        }
    }
}
