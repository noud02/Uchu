using System.Linq;
using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class LuaScriptComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ScriptComponent;
        
        public LegoDataDictionary Data { get; set; }
        
        public string ScriptName { get; set; }
        
        public string ClientScriptName { get; set; }

        protected LuaScriptComponent()
        {
            Listen(OnStart, () =>
            {
                var scriptId = GameObject.Lot.GetComponentId(ComponentId.ScriptComponent);

                var script = ClientCache.Find<ScriptComponent>(scriptId);

                if (script == default)
                {
                    Logger.Warning($"{GameObject} has an invalid script component entry: {scriptId}");
                    
                    return;
                }

                if (GameObject.Settings.TryGetValue("custom_script_server", out var scriptOverride))
                {
                    ScriptName = (string) scriptOverride;
                }
                else
                {
                    ScriptName = script.Scriptname;
                }
                
                ClientScriptName = script.Clientscriptname;
            
                Logger.Debug($"{GameObject} -> {ScriptName}");
            });
        }

        public override void Construct(BitWriter writer)
        {
            var hasData = Data != null;

            writer.WriteBit(hasData);
            if (hasData) writer.WriteLdfCompressed(Data);
        }

        public override void Serialize(BitWriter writer)
        {
            
        }
    }
}