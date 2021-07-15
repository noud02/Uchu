using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1620_script_name__removed")]
    public class LanternOfSpinjitzu : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public LanternOfSpinjitzu(GameObject gameObject) : base(gameObject)
        {
            // Listen to players interacting with railposts
            Listen(gameObject.OnInteract, async player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                    return;

                // If player has Cole's mission active, set flag to proceed with next mission
                if (missionInventory.HasActive(MissionId.LanternsofSpinjitzu))
                    await characterComponent.SetFlagAsync(Flag.UsedEarthLantern, true);
            });
        }
    }
}
