using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ScriptName("ScriptComponent_1613_script_name__removed")]
    public class Altar : ObjectScript
    {
        private const int ImaginationLot = Lot.ThreeImagination;
        private const int ImaginationCount = 5;

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Altar(GameObject gameObject) : base(gameObject)
        {
            // Listen to players interacting with altars
            Listen(gameObject.OnInteract, player => {
                if (gameObject.GetComponent<QuickBuildComponent>().State != RebuildState.Completed)
                    return;

                // Drop imagination
                for (var i = 0; i < ImaginationCount; i++)
                {
                    var loot = InstancingUtilities.InstantiateLoot(ImaginationLot,
                        player, gameObject, gameObject.Transform.Position + Vector3.UnitY * 3);
                    Start(loot);
                }
            });
        }
    }
}
