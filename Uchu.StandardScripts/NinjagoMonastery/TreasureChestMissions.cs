using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1696_script_name__removed")]
    public class TreasureChest : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TreasureChest(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, async player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!player.TryGetComponent<InventoryManagerComponent>(out var inventory))
                    return;

                // Take away key and give contents of chest
                if (missionInventory.HasActive((int) gameObject.Settings["ScrollMission"]))
                {
                    await inventory.RemoveLotAsync((int) gameObject.Settings["KeyNum"], 1);
                    await inventory.AddLotAsync((int) gameObject.Settings["openItemID"], 1);
                }
            });
        }
    }
}
