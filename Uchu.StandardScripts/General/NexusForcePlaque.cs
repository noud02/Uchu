using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    ///     LUA Reference: l_story_box_interact_client.lua
    /// </summary>
    public class NexusForcePlaque : NativeScript
    {
        private const string ScriptName = "l_story_box_interact_client.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName, true))
            {
                if (!gameObject.Settings.TryGetValue("storyText", out var text))
                    continue;

                var idString = (string) text;
                var id = int.Parse(idString.Substring(idString.Length - 2));
                var flag = 10000 + Zone.ZoneId + id;
                
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