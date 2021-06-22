using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class ModularBuilderComponent : Component
    {
        public GameObject BasePlate { get; private set; }
        
        public bool IsBuilding { get; private set; }

        protected ModularBuilderComponent()
        {
            Listen(OnStart, () =>
            {
                var inventory = GameObject.GetComponent<InventoryComponent>();

                Listen(inventory.OnEquipped, item =>
                {
                    if (item.ItemType == ItemType.LootModel)
                    {
                        StartBuildingWithItem(item);
                    }
                    
                    return Task.CompletedTask;
                });
            });
        }

        public async Task StartBuildingAsync(StartBuildingWithItemMessage message)
        {
            if (!(GameObject is Player player))
                return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            var thinkingHat = inventory[InventoryType.Items].Items.First(i => i.Lot == 6086);

            await thinkingHat.EquipAsync(true);
            
            IsBuilding = true;
            BasePlate = message.Associate;
            
            player.Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = message.FirstTime,
                BuildArea = message.Associate,
                StartPosition = Transform.Position,
                
                SourceBag = message.SourceBag,
                Source = message.Source,
                SourceLot = message.SourceLot,
                SourceType = 8,
                
                Target = message.Target,
                TargetLot = message.TargetLot,
                TargetPosition = message.TargetPosition,
                TargetType = message.TargetType
            });
        }

        public void StartBuildingWithItem(Item item)
        {
            if (!(GameObject is Player player)) return;

            player.Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = false,
                BuildArea = BasePlate,
                StartPosition = Transform.Position,
                
                SourceBag = item.Inventory.InventoryType,
                Source = item,
                SourceLot = item.Lot,
                SourceType = 8, // TODO: find out how to get this
                
                Target = BasePlate,
                TargetLot = BasePlate.Lot,
                TargetPosition = BasePlate.Transform.Position,
                TargetType = 0
            });
        }
        
        public async Task FinishBuilding(Lot[] models)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            // Remove all the items that were used for building this module
            foreach (var lot in models)
            {
                await inventory.RemoveLotAsync(lot, 1, InventoryType.TemporaryModels);
            }

            var model = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(models.Select(s => s.Id))
            };
            
            await inventory.AddLotAsync(6416, 1, model, InventoryType.Models);

            await ConfirmFinish();
        }
        
        public void DoneArranging(DoneArrangingWithItemMessage message)
        {
        }

        public async Task Pickup(Lot lot)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            
            var item = inventory[InventoryType.TemporaryModels].Items.First(i => i.Lot == lot);

            await item.EquipAsync(true);

            /*
            As<Player>().Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = false
            });
            */
        }

        public async Task ConfirmFinish()
        {
            if (!(GameObject is Player player))
                return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            if (inventory[InventoryType.TemporaryModels] != null)
            {
                foreach (var temp in inventory[InventoryType.TemporaryModels].Items)
                {
                    await inventory.MoveItemBetweenInventoriesAsync(
                        temp,
                        temp.Count,
                        InventoryType.TemporaryModels,
                        InventoryType.Models
                    );
                }
            }
            
            var thinkingHat = inventory[InventoryType.Items].Items.First(i => i.Lot == 6086);
            await thinkingHat.UnEquipAsync();
            
            player.Message(new FinishArrangingWithItemMessage
            {
                Associate = GameObject,
                BuildArea = BasePlate
            });
            
            IsBuilding = false;
        }
    }
}
