using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.Scripting;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ZoneSpecific(1300)]
    public class TikiTorch: NativeScript
    {
        private const string ScriptName = "ScriptComponent_946_script_name__removed";
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName))
            {
                Listen(gameObject.OnInteract, async player =>
                {
                    player.Message(new PlayAnimationMessage
                    {
                        Associate = gameObject,
                        AnimationId = "interact",
                        PlayImmediate = false,
                        TriggerOnCompleteMessage = false,
                        Priority = 0.4f,
                        Scale = 1,
                    });
                    player.Message(new ScriptNetworkVarUpdateMessage
                    {
                        Associate = gameObject,
                        Data = new LegoDataDictionary
                        {
                            {"bIsInUse", true},
                        },
                    });

                    for (int i = 0; i < 2; i++)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, player, gameObject, gameObject.Transform.Position + Vector3.UnitY);
                        Start(loot);
                    }
                    
                    await Task.Delay(500);
                    player.Message(new ScriptNetworkVarUpdateMessage
                    {
                        Associate = gameObject,
                        Data = new LegoDataDictionary
                        {
                            {"bIsInUse", false},
                        },
                    });
                });
            }

            return Task.CompletedTask;
        }
    }
}