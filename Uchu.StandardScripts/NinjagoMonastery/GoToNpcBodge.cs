using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class GoToNpcBodge : NativeScript
    {
        public override Task LoadAsync()
        {
            // Okay look this is a really stupid script but the cdclient is messed up.
            // Check the lu-explorer links I left here. The GoToNpc tasks have
            // entirely unrelated targets. (These targets aren't used for any other
            // GoToNpc tasks, so it should be fine to just call GoToNpc for them.)

            // https://lu.lcdruniverse.org/explorer/missions/1797
            var senseiWu = Zone.GameObjects.First(g => g.Lot == Lot.SenseiWuNpc);
            Listen(senseiWu.OnInteract, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!missionInventory.HasActive((int) MissionId.DangerBelow))
                    return;
                missionInventory.GoToNpcAsync(Lot.AssemblyPetConsole);
                missionInventory.GetMission((int) MissionId.DangerBelow).CompleteAsync();
            });

            // https://lu.lcdruniverse.org/explorer/missions/1798
            var jay = Zone.GameObjects.First(g => g.Lot == Lot.JayNpc);
            Listen(jay.OnInteract, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!missionInventory.HasActive((int) MissionId.NinjaofLightning))
                    return;
                missionInventory.GoToNpcAsync(Lot.HariHowzenNpc);
                missionInventory.GetMission((int) MissionId.NinjaofLightning).CompleteAsync();
            });

            return Task.CompletedTask;
        }
    }
}
