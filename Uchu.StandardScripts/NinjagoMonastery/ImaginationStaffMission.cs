using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1603_script_name__removed")]
    public class ImaginationStaffMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ImaginationStaffMission(GameObject gameObject) : base(gameObject)
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnSkillEvent, async (target, effectHandler) =>
                {
                    // Check if player is targeting NPC
                    if (target != gameObject)
                        return;

                    // Check if player is executing right skill
                    if (effectHandler != "NinjagoSpinAttackEvent")
                        return;

                    // Complete script task
                    if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                        return;
                    await missionInventory.ScriptAsync(2543, gameObject.Lot);
                });
            });
        }
    }
}
