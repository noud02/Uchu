using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    ///     LUA Reference: l_binoculars_client.lua
    /// </summary>
    public class Binoculars : NativeScript
    {
        private const string ScriptName = "l_binoculars_client.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName, true))
            {
                if (!gameObject.Settings.TryGetValue("number", out var number))
                    continue;

                var worldId = ((int) Zone.ZoneId).ToString().Substring(0, 2);
                var flag = int.Parse($"{worldId}{number}");
                
                Listen(gameObject.OnInteract, async player =>
                {
                    if (player.TryGetComponent<CharacterComponent>(out var character))
                    {
                        await character.SetFlagAsync(flag, true);
                        player.Message(new FireEventClientSideMessage
                        {
                            Associate = gameObject,
                            Arguments = "achieve",
                            Target = gameObject,
                            Sender = player
                        });
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}