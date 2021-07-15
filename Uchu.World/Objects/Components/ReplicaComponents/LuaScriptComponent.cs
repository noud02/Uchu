using System;
using System.Linq;
using InfectedRose.Core;
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
                // Get the script component or custom script.
                var scriptId = GameObject.Lot.GetComponentId(ComponentId.ScriptComponent);
                var script = ClientCache.GetTable<ScriptComponent>().FirstOrDefault(s => s.Id == scriptId);
                if (script == default)
                {
                    Logger.Warning($"{GameObject} has an invalid script component entry: {scriptId}");
                    return;
                }

                if (GameObject.Settings.TryGetValue("custom_script_server", out var scriptOverride) && (string) scriptOverride != "")
                {
                    ScriptName = (string) scriptOverride;
                }
                else
                {
                    ScriptName = script.Scriptname;
                }
                
                // Set the script name.
                this.ClientScriptName = script.Clientscriptname;
                Logger.Debug($"{GameObject} -> {this.ScriptName}, {this.ClientScriptName}");
                
                // Start the object script.
                var newObjectScriptName = Zone.ScriptManager.ObjectScriptTypes.Keys.FirstOrDefault(
                    objectScriptName => (this.ScriptName ?? "").ToLower().EndsWith(objectScriptName))
                        ?? Zone.ScriptManager.ObjectScriptTypes.Keys.FirstOrDefault(
                            objectScriptName => (this.ClientScriptName ?? "").ToLower().EndsWith(objectScriptName));
                if (newObjectScriptName == null) return;
                this.Zone.LoadObjectScript(this.GameObject, Zone.ScriptManager.ObjectScriptTypes[newObjectScriptName]);
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
