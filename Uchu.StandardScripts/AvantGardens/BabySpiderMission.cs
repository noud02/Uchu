using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class BabySpiderMission : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 15902))
            {
                Listen(gameObject.OnInteract, async player =>
                {
                    if (player.TryGetComponent<CharacterComponent>(out var character))
                    {
                        await character.SetFlagAsync(74, true);
                        foreach (var spider in GetGroup("cagedSpider"))
                        {
                            player.Message(new FireEventClientSideMessage
                            {
                                Associate = spider,
                                Sender = player,
                                Arguments = "toggle",
                                Target = player,
                            });
                        }

                        if (player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager))
                        {
                            await inventoryManager.RemoveAllAsync(14553);
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }
    }
}