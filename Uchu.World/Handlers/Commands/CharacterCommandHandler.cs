using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;
using Uchu.World.Filters;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.World.Handlers.Commands
{
    public class CharacterCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "chat", Help = "Change chat level", GameMasterLevel = GameMasterLevel.Player)]
        public string ChangeChatLevel(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "chat <level>";

            if (!Enum.TryParse(typeof(PlayerChatChannel), arguments[0], out var level)) return "Invalid <level>";

            player.ChatChannel = (PlayerChatChannel) level;

            return $"Changed chat level to: {player.ChatChannel}";
        }
        
        [CommandHandler(Signature = "gmadditem", Help = "Give an item to yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> GiveItem(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "gmadditem <lot> <count(optional)> <inventory(optional)>";

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            uint count = 1;
            if (arguments.Length >= 2)
                if (!uint.TryParse(arguments[1], out count))
                    return "Invalid <count(optional)>";

            if (arguments.Length != 3)
            {
                await player.GetComponent<InventoryManagerComponent>().AddLotAsync(lot, count);
            }
            else
            {
                if (!uint.TryParse(arguments[2], out var inventory))
                    return "Invalid <inventory(optional)>";

                await player.GetComponent<InventoryManagerComponent>().AddLotAsync(lot, count, 
                    inventoryType: (InventoryType) inventory);
            }

            return $"Successfully added {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "remove", Help = "Remove an item from yourself",
            GameMasterLevel = GameMasterLevel.Mythran)]
        public async Task<string> RemoveItem(string[] arguments, Player player)
        {
            if (arguments.Length == 0 || arguments.Length > 2) return "remove <lot> <count(optional)>";

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            uint count = 1;
            if (arguments.Length == 2)
                if (!uint.TryParse(arguments[1], out count))
                    return "Invalid <count(optional)>";

            await player.GetComponent<InventoryManagerComponent>().RemoveLotAsync(lot, count);

            return $"Successfully removed {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "coin", Help = "Add or remove coin from yourself",
            GameMasterLevel = GameMasterLevel.Admin)]
        public string ChangeCoin(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "coin <delta>";

            // We parse this as an int instead of long, due to the max long causing bugs. Currency maxes out at a long.
            if (!int.TryParse(arguments[0], out var delta) || delta == default) return "Invalid <delta>";

            var character = player.GetComponent<CharacterComponent>();
            character.Currency += delta;

            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} coins";
        }

        [CommandHandler(Signature = "spawn", Help = "Spawn an object", GameMasterLevel = GameMasterLevel.Admin)]
        public string Spawn(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "spawn <lot> <x(optional)> <y(optional)> <z(optional)>";

            arguments = arguments.Select(a => a.Replace('.', ',')).ToArray();

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            var position = player.Transform.Position;

            var factionSided = arguments.Contains("-f");
            
            if (!factionSided)
            {
                if (arguments.Length >= 4)
                {
                    try
                    {
                        position = new Vector3
                        {
                            X = float.Parse(arguments[1].Replace('.', ',')),
                            Y = float.Parse(arguments[2].Replace('.', ',')),
                            Z = float.Parse(arguments[3].Replace('.', ','))
                        };
                    }
                    catch
                    {
                        return "Invalid <x(optional)>, <y(optional)>, or <z(optional)>";
                    }
                }
            }

            var rotation = player.Transform.Rotation;

            var obj = GameObject.Instantiate(new LevelObjectTemplate
            {
                Lot = lot,
                Position = position,
                Rotation = rotation,
                Scale = 1,
                LegoInfo = new LegoDataDictionary()
            }, player.Zone);

            Object.Start(obj);
            GameObject.Construct(obj);

            if (factionSided)
            {
                if (obj.TryGetComponent<DestroyableComponent>(out var stats))
                {
                    stats.Factions = new[] {int.Parse(arguments[2])};
                    stats.Enemies = new[] {int.Parse(arguments[3])};
                    
                    obj.GetComponent<DestructibleComponent>().ResurrectTime = 30;
                    obj.GetComponent<BaseCombatAiComponent>().Enabled = false;
                    
                    GameObject.Serialize(obj);
                }
            }

            return $"Successfully spawned {lot} at\npos: {position}\nrot: {rotation}";
        }

        [CommandHandler(Signature = "position", Help = "Get your position", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Position(string[] arguments, Player player)
        {
            return $"{player.Transform.Position}";
        }

        [CommandHandler(Signature = "rotation", Help = "Get your rotation", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Rotation(string[] arguments, Player player)
        {
            return $"{player.Transform.Rotation}";
        }

        [CommandHandler(Signature = "smash", Help = "Smash yourself", GameMasterLevel = GameMasterLevel.Player)]
        public async Task<string> Smash(string[] arguments, Player player)
        {
            var animation = "violent";

            if (arguments.Length != default)
            {
                animation = arguments[default];
            }

            await player.GetComponent<DestructibleComponent>().SmashAsync(player, player, animation);

            return "You smashed yourself";
        }
        
        [CommandHandler(Signature = "buffme", Help = "Boost stats for testing", GameMasterLevel = GameMasterLevel.Admin)]
        public string Buffme(string[] arguments, Player player)
        {
            DestroyableComponent comp = player.GetComponent<DestroyableComponent>();

            comp.MaxArmor = 999;
            comp.MaxHealth = 999;
            comp.Armor = 999;
            comp.Health = 999;

            comp.MaxImagination = 999;
            comp.Imagination = 999;

            return "Buffed";
        }

        [CommandHandler(Signature = "freecam", Help = "Fly around in a free camera. Initial positon is at 0, 0, 0.", GameMasterLevel = GameMasterLevel.Admin)]
        public string Freecam(string[] arguments, Player player)
        {
            if (arguments.Length == 0)
                return "Usage: /freecam <on/off>";

            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    player.Message(new SetPlayerControlSchemeMessage
                    {
                        Associate = player,
                        DelayCameraSwitchIfInCinematic = false,
                        SwitchCamera = true,
                        ControlScheme = 9
                    });
                    break;
                case "false":
                case "off":
                    player.Message(new SetPlayerControlSchemeMessage
                    {
                        Associate = player,
                        DelayCameraSwitchIfInCinematic = false,
                        SwitchCamera = true,
                        ControlScheme = 0
                    });
                    break;
                default:
                    return "Usage: /freecam <on/off>";
            }

            return "Toggled freecam.";
        }

        [CommandHandler(Signature = "dab", Help = "dab", GameMasterLevel = GameMasterLevel.Player)]
        public void Dab(string[] arguments, Player player)
        {
            player.Animate("cute-spin-exit");
        }

        [CommandHandler(Signature = "fly", Help = "Change jetpack state", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Fly(string[] arguments, Player player)
        {
            var doHover = arguments.Contains("hover");

            player.TryGetComponent<ControllablePhysicsComponent>(out var controllablePhysicsComponent);

            if (arguments.Length == 0 || (arguments.Length == 1 && doHover))
            {
                controllablePhysicsComponent.Flying = !controllablePhysicsComponent.Flying;
            }
            else if (arguments.Length >= 1)
            {
                switch (arguments[0].ToLower())
                {
                    case "true":
                    case "on":
                        controllablePhysicsComponent.Flying = true;
                        break;
                    case "false":
                    case "off":
                        controllablePhysicsComponent.Flying = false;
                        break;
                    default:
                        return "Invalid <state(on/off)>";
                }
            }

            if (arguments.Length == 2 || (arguments.Length == 3 && doHover))
            {
                if (float.TryParse(arguments[1], out float speed))
                {
                    controllablePhysicsComponent.JetPackAirSpeed = speed;
                }
            }

            float jetPackMaxAirSpeed = controllablePhysicsComponent.JetPackAirSpeed + 5;

            player.Message(new SetJetPackModeMessage
            {
                Associate = player,
                BypassChecks = true,
                Use = controllablePhysicsComponent.Flying,
                DoHover = doHover,
                EffectId = 36,
                AirSpeed = controllablePhysicsComponent.JetPackAirSpeed,
                MaxAirSpeed = jetPackMaxAirSpeed
            });

            return $"Toggled jetpack state: {controllablePhysicsComponent.Flying}";
        }

        [CommandHandler(Signature = "hover", Help = "Change hover state", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Hover(string[] arguments, Player player)
        {
            string[] hover = {"hover"};
            return Fly(arguments.Concat(hover).ToArray(), player);
        }

        [CommandHandler(Signature = "group", Help = "Search for objects with group", GameMasterLevel = GameMasterLevel.Admin)]
        public string Group(string[] arguments, Player player)
        {
            var builder = new StringBuilder();
            
            if (arguments.Length == default)
            {
                foreach (var gameObject in player.Zone.GameObjects)
                {
                    if (!gameObject.Settings.TryGetValue("groupID", out var groupId)) continue;

                    if (!(groupId is string groupIdString)) continue;

                    builder.AppendLine($"{groupIdString}");
                }

                return builder.ToString();
            }

            foreach (var gameObject in NativeScript.GetGroup(player.Zone, arguments[0]))
            {
                builder.AppendLine($"{gameObject}");
            }

            return builder.ToString();
        }

        [CommandHandler(Signature = "fx", Help = "Play FX", GameMasterLevel = GameMasterLevel.Admin)]
        public string Fx(string[] arguments, Player player)
        {
            if (arguments.Length != 3) return "fx <name> <type> <id>";

            var name = arguments[0];
            var type = arguments[1];
            var id = int.Parse(arguments[2]);

            player.PlayFX(name, type, id);

            Task.Run(async () =>
            {
                await Task.Delay(10000);

                player.StopFX(name);
            });

            return $"Successfully played FX: {name}, {type}, {id}";
        }

        [CommandHandler(Signature = "near", Help = "Get nearest object", GameMasterLevel = GameMasterLevel.Player)]
        public string Near(string[] arguments, Player player)
        {
            var current = player.Zone.GameObjects[0];

            if (!arguments.Contains("-m"))
            {
                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default) continue;

                    if (!arguments.Contains("-sp"))
                    {
                        if (gameObject.TryGetComponent<SpawnerComponent>(out _)) continue;
                    }
                    
                    if (arguments.Contains("-npm"))
                    {
                        if (gameObject.TryGetComponent<PrimitiveModelComponent>(out _)) continue;
                    }

                    if (arguments.Contains("-t"))
                    {
                        if (!gameObject.TryGetComponent<TriggerComponent>(out _)) continue;
                    }

                    if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        current = gameObject;
                }
            }
            else
            {
                current = player;
            }

            if (current == default) return "No objects in this zone.";

            var info = new StringBuilder();

            if (arguments.Contains("-i"))
                info.Append($"[{current.Id}] ");

            info.Append($"[{current.Lot}] \"{(string.IsNullOrWhiteSpace(current.Name) ? current.ClientName : current.Name)}\"");

            if (arguments.Contains("-cq"))
            {
                if (current.TryGetComponent<LuaScriptComponent>(out var scriptComponent))
                    info.AppendLine($"\nServer Script: {scriptComponent.ScriptName}\nClient Script: {scriptComponent.ClientScriptName}");
            }

            if (arguments.Contains("-g"))
            {
                if (current.Settings.TryGetValue("groupID", out var group))
                    info.AppendLine($"\nGroupID: {group}");
            }
                
            if (arguments.Contains("-s")) info.Append($"\n{current.Settings}");

            if (arguments.Contains("-l")) info.Append($"\nLayers: {Convert.ToString(current.Layer.Value, 2)}");

            var components = arguments.Contains("-r") ? current.ReplicaComponents : current.GetAllComponents();

            if (!arguments.Contains("-c") && !arguments.Contains("-r")) goto finish;

            foreach (var component in components)
            {
                info.Append($"\n{component.GetType().Name}");

                if (component is DestroyableComponent stats)
                {
                    info.Append($" {stats.HasStats}");
                }

                if (!arguments.Contains("-p")) continue;

                foreach (var property in component.GetType().GetProperties())
                    info.Append($"\n: {property.Name} = {property.GetValue(component)}");
            }

            if (arguments.Contains("-in"))
            {
                current.OnInteract.Invoke(player);
            }

            finish:

            return info.ToString();
        }

        [CommandHandler(Signature = "setname", Help = "Set name of gameobject", GameMasterLevel = GameMasterLevel.Admin)]
        public string SetName(string[] arguments, Player player)
        {
            if (arguments.Length == default)
            {
                return "/setname <name>";
            }
            
            var current = player.Zone.GameObjects[0];

            if (arguments.Length > 1 && arguments.Contains("-m"))
            {
                current = player;
            }
            else
            {
                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default) continue;

                    if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                    if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        current = gameObject;
                }
            }

            current.Name = arguments[0];

            return $"Successfully set name to {arguments[0]}";
        }

        [CommandHandler(Signature = "score", Help = "Change your U-score", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Score(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "score <delta>";

            if (!int.TryParse(arguments[0], out var delta)) return "Invalid <delta>";

            var character = player.GetComponent<CharacterComponent>();
            character.UniverseScore += delta;

            GameObject.Serialize(player);

            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} score";
        }

        [CommandHandler(Signature = "level", Help = "Set your level", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Level(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "level <level>";

            if (!long.TryParse(arguments[0], out var level)) return "Invalid <level>";

            var character = player.GetComponent<CharacterComponent>();
            await character.SetLevelAsync(level);

            GameObject.Serialize(player);

            return $"Successfully set your level to {level}";
        }
        
        [CommandHandler(Signature = "stat", Help = "Set a stat", GameMasterLevel = GameMasterLevel.Admin)]
        public string Stat(string[] arguments, Player player)
        {
            if (arguments.Length != 2) return "stat <stat> <value>";

            if (!long.TryParse(arguments[1], out var value)) return "Invalid <value>";

            var stats = player.GetComponent<DestroyableComponent>();

            var stat = arguments[0].ToLower().Replace("-", "").Replace("_", "");
            switch (stat)
            {
                case "health":
                    stats.Health = (uint) value;
                    break;
                case "maxhealth":
                    stats.MaxHealth = (uint) value;
                    break;
                case "armor":
                    stats.Armor = (uint) value;
                    break;
                case "maxarmor":
                    stats.MaxArmor = (uint) value;
                    break;
                case "imagination":
                    stats.Imagination = (uint) value;
                    break;
                case "maximagination":
                    stats.MaxImagination = (uint) value;
                    break;
                default:
                    return $"{stat} is not a valid <stat>";
            }
            
            GameObject.Serialize(player);

            return $"Successfully set {arguments[0]} to {value}";
        }
        
        [CommandHandler(Signature = "pvp", Help = "Change PvP state", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Pvp(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "pvp <state(on/off)>";

            bool state;
            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    state = false;
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            player.GetComponent<CharacterComponent>().IsPvP = state;

            GameObject.Serialize(player);

            return $"Successfully set your pvp state to {state}";
        }

        [CommandHandler(Signature = "gm", Help = "Change Game Master state", GameMasterLevel = GameMasterLevel.Mythran)]
        public string GameMaster(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "gm <state(on/off)>";

            bool state;
            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    state = false;
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            player.GetComponent<CharacterComponent>().IsGameMaster = state;

            GameObject.Serialize(player);

            return $"Successfully set your GameMaster state to {state}";
        }

        [CommandHandler(Signature = "gmlevel", Help = "Set GameMaster Level state", GameMasterLevel = GameMasterLevel.Admin)]
        public string GmLevel(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "gmlevel <level>";

            if (!byte.TryParse(arguments[0], out var gmlevel)) return "Invalid <level>";

            player.GetComponent<CharacterComponent>().GameMasterLevel = gmlevel;

            player.GameMasterLevel = gmlevel;

            return $"Successfully set your GameMaster level to {gmlevel}";
        }

        [CommandHandler(Signature = "layer", Help = "Change your layer", GameMasterLevel = GameMasterLevel.Admin)]
        public string Layer(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "layer <layer>";

            if (!long.TryParse(arguments[0], out var layer)) return "Invalid <layer>";

            if (!player.Perspective.TryGetFilter<MaskFilter>(out var filter)) return $"No {nameof(MaskFilter)} applied";
            
            if (filter.ViewMask == layer)
                filter.ViewMask -= layer;
            else filter.ViewMask += layer;

            return $"Layer set to {Convert.ToString(filter.ViewMask.Value, 2)}";
        }

        [CommandHandler(Signature = "brick", Help = "Spawns a floating brick",
            GameMasterLevel = GameMasterLevel.Mythran)]
        public async Task<string> Brick(string[] arguments, Player player)
        {
            var bricks = new List<GameObject>();
            
            for (var i = 0; i < 10; i++)
            {
                var brick = GameObject.Instantiate(
                    player.Zone,
                    31,
                    player.Transform.Position + Vector3.UnitY * (7 + i),
                    Quaternion.Identity
                );

                Object.Start(brick);
                GameObject.Construct(brick);

                bricks.Add(brick);
                
                await Task.Delay(100 - i * 10);
                
                if (i == 0) i++;
            }

            await Task.Delay(500);

            foreach (var brick in bricks)
            {
                Object.Destroy(brick);
            }

            return $"Spawned thing";
        }

        [CommandHandler(Signature = "tp", Help = "Teleport", GameMasterLevel = GameMasterLevel.Admin)]
        public string Teleport(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "tp <target>/<x> <y> <z>";

            Vector3 position;

            var relativeX = arguments[0].StartsWith('~');
            if (relativeX) arguments[0] = arguments[0].Remove(default, 1);

            if (!relativeX && !float.TryParse(arguments[0], out _))
            {
                var target = player.Zone.Players.FirstOrDefault(p => p.Name == arguments[0]);

                if (target == default) return $"No player named {arguments[0]}";
                
                player.Teleport(target.Transform.Position);

                return $"Going to {player.Name}";
            }

            if (arguments.Length != 3) return "tp <x> <y> <z>";
            
            var relativeY = arguments[1].StartsWith('~');
            if (relativeY) arguments[1] = arguments[1].Remove(default, 1);
            
            var relativeZ = arguments[2].StartsWith('~');
            if (relativeZ) arguments[2] = arguments[2].Remove(default, 1);

            if (!float.TryParse(arguments[0], out position.X)) return "Invalid <x>";
            if (!float.TryParse(arguments[1], out position.Y)) return "Invalid <y>";
            if (!float.TryParse(arguments[2], out position.Z)) return "Invalid <z>";
            
            if (relativeX) position.X += player.Transform.Position.X;
            if (relativeY) position.Y += player.Transform.Position.Y;
            if (relativeZ) position.Z += player.Transform.Position.Z;
            
            player.Teleport(position);

            return $"Going to {position}";
        }

        [CommandHandler(Signature = "animate", Help = "Perform an animation", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Animate(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "animate <animationId>";

            var scale = 1f;

            if (arguments.Length == 2)
            {
                if (!float.TryParse(arguments[1], out scale))
                {
                    return "Invalid <scale>";
                }
            }

            GameObject associate = player;

            if (arguments.Contains("-n"))
            {
                associate = player.Zone.GameObjects[0];

                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default) continue;

                    if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                    if (Vector3.Distance(associate.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        associate = gameObject;
                }
            }
            
            player.Zone.BroadcastMessage(new PlayAnimationMessage
            {
                Associate = associate,
                AnimationsId = arguments[0],
                Scale = scale
            });

            return $"Attempting to play {arguments[0]} animation";
        }

        [CommandHandler(Signature = "addcomponent", Help = "Add a component to an object", GameMasterLevel = GameMasterLevel.Admin)]
        public string AddComponent(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "addcomponent <type>";

            var type = Type.GetType(arguments[0]);

            if (type == default) return "Invalid <type>";

            player.AddComponent(type);

            return $"Successfully added {type.Name} to {player}";
        }

        [CommandHandler(Signature = "testmap", Help = "Transfer to world", GameMasterLevel = GameMasterLevel.Mythran)]
        public string World(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "testmap <zoneId>";

            if (!int.TryParse(arguments[0], out var id)) return "Invalid <zoneId>";

            var WorldTable = ClientCache.Find<ZoneTable>(id);

            if (WorldTable == default)
            {
                return $"Can't find world with ID {id}";
            }

            var path = Path.Combine(UchuServer.Config.ResourcesConfiguration?.GameResourceFolder, Path.Combine("maps", WorldTable.ZoneName.ToLower()));

            if (File.Exists(path) && WorldTable.LocStatus > 0)
            {
                //
                // We don't want to lock up the server on a world server request, as it may take time.
                //

                player.Zone.BroadcastMessage(new SetStunnedMessage
                {
                    Associate = player,
                    CantMove = true,
                    CantJump = true,
                    CantAttack = true,
                    CantTurn = true,
                    CantUseItem = true,
                    CantEquip = true,
                    CantInteract = true
                });

                var _ = Task.Run(async () =>
                {
                    if (!await player.SendToWorldAsync((ZoneId)id))
                    {
                        player.SendChatMessage($"Failed to transfer to {id}, please try later.");
                    }
                });

                return $"Requesting transfer to {id}, please wait...";
            } 
            else
            {
                return $"Can't find world with ID {id}";
            }           
        }

        [CommandHandler(Signature = "monitor", Help = "Get server info", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Monitor(string[] arguments, Player player)
        {
            var process = Process.GetCurrentProcess();

            static string GetMemory(long memory)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };

                var len = memory;
                
                var order = 0;
                
                while (len >= 1024 && order < sizes.Length - 1) {
                    order++;
                    len /= 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }
            
            var builder = new StringBuilder();

            if (arguments.Length == 0 || arguments.Contains("-i"))
            {
                builder.AppendLine($"Id: {process.Id}");
            }

            if (arguments.Length == 0 || arguments.Contains("-t"))
            {
                var span = DateTime.Now - process.StartTime;
                
                builder.AppendLine($"Uptime: {span.Days}D, {span.Hours}H, {span.Minutes}m, {span.Seconds}s, {span.Milliseconds}ms");
            }
            
            if (arguments.Length == 0 || arguments.Contains("-m"))
            {
                builder.AppendLine($"Memory: {GetMemory(process.PrivateMemorySize64)}");
            }

            if (arguments.Length == 0 || arguments.Contains("-wm"))
            {
                builder.AppendLine($"Working Memory: {GetMemory(process.WorkingSet64)}");
            }

            if (arguments.Length == 0 || arguments.Contains("-pm"))
            {
                builder.AppendLine($"Peak Memory: {GetMemory(process.PeakWorkingSet64)}");
            }

            if (arguments.Length == 0 || arguments.Contains("-pam"))
            {
                builder.AppendLine($"Paged Memory: {GetMemory(process.PagedMemorySize64)}");
            }

            if (arguments.Length == 0 || arguments.Contains("-gc"))
            {
                GC.Collect();
                
                builder.AppendLine($"GC Memory: {GetMemory(GC.GetTotalMemory(true))}");
            }

            if (arguments.Length == 0 || arguments.Contains("-p"))
            {
                var span = process.TotalProcessorTime;

                builder.AppendLine($"CPU Time: {span.Days}D, {span.Hours}H, {span.Minutes}m, {span.Seconds}s, {span.Milliseconds}ms");
            }

            if (arguments.Length == 0 || arguments.Contains("-l"))
            {
                var start = DateTime.Now;
                
                var before = process.TotalProcessorTime;

                await Task.Delay(1000);

                var time = process.TotalProcessorTime - before;

                builder.AppendLine(
                    $"Load: {(int) (time.TotalMilliseconds / (DateTime.Now - start).TotalMilliseconds / Environment.ProcessorCount * 100)}%"
                );
            }

            builder.Length--;
            
            return builder.ToString();
        }
        
        [CommandHandler(Signature = "getemote", Help = "Unlock an emote", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> GetEmote(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "getemote <emote>";

            Emotes emote;
            
            if (int.TryParse(arguments[0], out var id))
            {
                emote = await ClientCache.FindAsync<Emotes>(id);
            }
            else
            {
                await using var cdClient = new CdClientContext();
                emote = cdClient.EmotesTable.FirstOrDefault(c => c.AnimationName == arguments[0].ToLower());
            }

            if (emote?.Id == default)
            {
                return "Invalid <emote>";
            }

            var state = false;

            if (arguments.Length == 2)
            {
                switch (arguments[1].ToLower())
                {
                    case "true":
                    case "on":
                        state = true;
                        break;
                    case "false":
                    case "off":
                        break;
                    default:
                        return "Invalid <state(on/off)>";
                }
            }

            player.Message(new SetEmoteLockStateMessage
            {
                Associate = player,
                EmoteId = emote.Id.Value,
                Lock = state
            });

            return $"Set emote: \"{emote.AnimationName}\" [{emote.Id}] lock state to {state}";
        }

        [CommandHandler(Signature = "unload", Help = "Unload scripts", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Unload(string[] arguments, Player player)
        {
            var builder = new StringBuilder();
            
            foreach (var scriptPack in player.Zone.ScriptManager.ScriptPacks)
            {
                if (arguments.Length != 0 && !arguments.Contains(scriptPack.Name)) continue;

                await scriptPack.UnloadAsync();

                builder.AppendLine($"Unloaded: {scriptPack.Name}");
            }

            if (builder.Length > 0)
                builder.Length--;

            return builder.ToString();
        }

        [CommandHandler(Signature = "reload", Help = "Reload scripts", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Reload(string[] arguments, Player player)
        {
            var builder = new StringBuilder();
            
            foreach (var scriptPack in player.Zone.ScriptManager.ScriptPacks)
            {
                if (arguments.Length != 0 && !arguments.Contains(scriptPack.Name)) continue;
                
                await scriptPack.ReloadAsync();

                builder.AppendLine($"Reloaded: {scriptPack.Name}");
            }

            if (builder.Length > 0)
                builder.Length--;

            return builder.ToString();
        }

        [CommandHandler(Signature = "mailbox", Help = "Set mailbox state", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> MailBox(string[] arguments, Player player)
        {
            if (arguments.Length == 0) return "/mailbox <state(on/off)>";
            
            var state = false;

            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            if (state)
            {
                await player.OpenMailboxGuiAsync();
            }
            else
            {
                await player.CloseMailboxGuiAsync();
            }

            return $"Set mailbox state to: {state}";
        }

        [CommandHandler(Signature = "announce", Help = "Send an announcement", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Announce(string[] arguments, Player player)
        {
            if (arguments.Length < 2) return "/annouce <title> <message>";

            var args = arguments.ToList();

            var title = args[0];

            args.RemoveAt(0);

            foreach (var zonePlayer in player.Zone.Players)
            {
                await UiHelper.AnnouncementAsync(zonePlayer, title, string.Join(" ", args));
            }

            return "Sent announcement";
        }

        [CommandHandler(Signature = "complete", Help = "Complete active missions", GameMasterLevel = GameMasterLevel.Mythran)]
        public async Task<string> Complete(string[] arguments, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>().AllMissions;
            var args = new List<int>();
            var achievements = arguments.Contains("-a");
            var list = arguments.ToList();

            if (achievements)
                list.Remove("-a");

            arguments = list.ToArray();
            
            foreach (var argument in arguments)
            {
                if (int.TryParse(argument, out var id))
                    args.Add(id);
            }
            
            await using var context = new UchuContext();
            foreach (var mission in missions)
            {
                if (mission.State == MissionState.Completed)
                    continue;

                if (!mission.IsMission && !achievements) continue;
                
                if (args.Count > 0 && !args.Contains(mission.MissionId))
                    continue;

                await mission.CompleteAsync();
            }

            return "Completed missions";
        }

        [CommandHandler(Signature = "clear", Help = "Clears your inventory", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Clear(string[] arguments, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();
            foreach (var item in inventory[InventoryType.Items].Items)
            {
                await inventory.RemoveItemAsync(item);
            }

            return "Cleared inventory";
        }

        [CommandHandler(Signature = "setflag", Help = "Sets a client flag", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> SetFlag(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "/setflag <flag>";

            if (!int.TryParse(arguments[0], out var flag))
                return "/setflag <flag>";

            var character = player.GetComponent<CharacterComponent>();
            await character.SetFlagAsync(flag, true);

            return $"Set flag {arguments[0]}";
        }

        [CommandHandler(Signature = "triggercelebrate", Help = "Triggers celebration", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> TriggerCelebrat(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "/triggercelebrate <CelebrationID>";

            if (!int.TryParse(arguments[0], out var id))
                return "/triggercelebrate <CelebrationID>";

            await player.TriggerCelebration((CelebrationId)id);

            return $"Triggered Celebration {arguments[0]}";
        }

        [CommandHandler(Signature = "removeflag", Help = "Removes a client flag", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> RemoveFlag(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "/removeflag <flag>";

            if (!int.TryParse(arguments[0], out int flag))
                return "/removeflag <flag>";

            var character = player.GetComponent<CharacterComponent>();
            await character.SetFlagAsync(flag, false);

            return $"Removed flag {arguments[0]}";
        }

        [CommandHandler(Signature = "inventory", Help = "Set inventory size", GameMasterLevel = GameMasterLevel.Admin)]
        public string Inventory(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "inventory <size>";

            if (!uint.TryParse(arguments[0], out var size)) return "Invalid <size>";

            player.GetComponent<InventoryManagerComponent>()[InventoryType.Items].Size = (int) size;

            return $"Set inventory size to: {size}";
        }

        [CommandHandler(Signature = "control", Help = "Change control scheme", GameMasterLevel = GameMasterLevel.Admin)]
        public string Control(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "control <scheme-id>";

            if (!int.TryParse(arguments[0], out var controlScheme)) return "Invalid <scheme-id>";
            
            player.Message(new SetPlayerControlSchemeMessage
            {
                Associate = player,
                DelayCameraSwitchIfInCinematic = false,
                SwitchCamera = true,
                ControlScheme = controlScheme
            });

            if (arguments.Length > 1)
            {
                if (float.TryParse(arguments[1], out var timed))
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay((int) (timed * 1000));
                        
                        player.Message(new SetPlayerControlSchemeMessage
                        {
                            Associate = player,
                            DelayCameraSwitchIfInCinematic = false,
                            SwitchCamera = true,
                            ControlScheme = 0
                        });
                    });
                }
            }

            return $"Switched control scheme to: {controlScheme}";
        }

        [CommandHandler(Signature = "mimic", Help = "Active mimic test", GameMasterLevel = GameMasterLevel.Admin)]
        public string Mimic(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "mimic <lot>";

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            var gameObject = MirrorGameObject.Instantiate(lot, player);

            Object.Start(gameObject);
            GameObject.Construct(gameObject);

            return $"Active mimic on: {gameObject}";
        }

        [CommandHandler(Signature = "gravity", Help = "Sets gravity scale", GameMasterLevel = GameMasterLevel.Admin)]
        public string Gravity(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "gravity <scale (0-2)>";

            if (!float.TryParse(arguments[0], out var scale)) return "Invalid <scale (0-2)>";

            player.GravityScale = scale;

            return $"Successfully set gravity scale to: {player.GravityScale}";
        }
    }
}
