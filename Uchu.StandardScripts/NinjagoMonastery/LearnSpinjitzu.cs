using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class LearnSpinjitzu : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                    return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    switch ((MissionId) instance.MissionId)
                    {
                        // Cole mission (Earth)
                        case MissionId.StudentofEarth:
                            characterComponent.SetFlagAsync(Flag.EarthSpinjitzu, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2053
                            break;
                        // Jay mission (Lightning)
                        case MissionId.StudentofLightning:
                            characterComponent.SetFlagAsync(Flag.LightningSpinjitzu, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2054
                            break;
                        // Zane mission (Ice)
                        case MissionId.StudentofIce:
                            characterComponent.SetFlagAsync(Flag.IceSpinjitzu, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2055
                            break;
                        // Kai mission (Fire)
                        case MissionId.StudentofFire:
                            characterComponent.SetFlagAsync(Flag.FireSpinjitzu, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2056
                            break;
                    }
                });
            });

            return Task.CompletedTask;
        }
    }
}
