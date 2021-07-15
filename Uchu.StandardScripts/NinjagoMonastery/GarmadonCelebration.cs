using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("l_garmadon_celebration_server.lua")]
    public class GarmadonCelebration : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public GarmadonCelebration(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();

            // Listen to player entering the trigger area
            Listen(physics.OnEnter, (other) =>
            {
                if (!(other.GameObject is Player player)) return;

                // Check if player has seen celebration before
                var character = player.GetComponent<CharacterComponent>();
                if (character.GetFlag(Flag.WitnessedGarmadonCelebration)) return;

                // Show celebration
                player.TriggerCelebration(CelebrationId.LordGarmadon);
                character.SetFlagAsync(Flag.WitnessedGarmadonCelebration, true);
            });
        }
    }
}
