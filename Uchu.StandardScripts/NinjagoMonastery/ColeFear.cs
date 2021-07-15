using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1647_script_name__removed")]
    public class ColeFear : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ColeFear(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnEmoteReceived, (emote, player) =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;

                // Player should use Dragon Roar emote
                if (emote != 393)
                    return;

                // Script for mission 1818, target: Cole
                missionInventory.ScriptAsync(2581, Lot.ColeNpc);
            });
        }
    }
}
