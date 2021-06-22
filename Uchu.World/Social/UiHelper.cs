using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Social
{
    public static class UiHelper
    {

        public static async Task AnnouncementAsync(Player player, string title, string message)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "message");
            
            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, message);
            Amf3Helper.WriteText(writer, "title");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, title);
            Amf3Helper.WriteText(writer, "visible");

            writer.Write((byte) Amf3Type.True);
            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "ToggleAnnounce"
            });
        }

        public static async Task StateAsync(Player player, string state)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "state");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, state);
            
            writer.Write((byte) Amf3Type.Null);

            player.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "pushGameState"
            });
        }

        public static async Task AddGuildMemberAsync(Player player, int index, GuildMember member)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "name");
            
            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, member.Name);
            Amf3Helper.WriteText(writer, "zone");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, member.Zone);
            Amf3Helper.WriteText(writer, "rank");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, member.Rank);
            Amf3Helper.WriteText(writer, "online");

            writer.Write((byte) (member.Online ? Amf3Type.True : Amf3Type.False));
            Amf3Helper.WriteText(writer, "index");

            writer.Write((byte) Amf3Type.Integer);
            Amf3Helper.WriteNumber2(writer, (uint) index);

            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "AddGuildMember"
            });
        }

        public static async Task SetGuildNameAsync(Player player, string name)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "guildName");
            
            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, name);
            
            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "SetGuildName"
            });
        }

        public static async Task ToggleAsync(Player player, string name, bool value)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "visible");

            writer.Write((byte) (value ? Amf3Type.True : Amf3Type.False));
            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = name
            });
        }

        public static async Task SingleArgumentGuiAsync(this Player @this, string name, string argument, string value)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, argument);
            
            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, value);
            
            writer.Write((byte) Amf3Type.Null);
            
            @this.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = @this,
                Content = stream.ToArray(),
                MessageName = name
            });
        }

        public static async Task MessageGuiAsync(this Player @this, string name, IDictionary<string, object> arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }
            
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);

            foreach (var (key, value) in arguments)
            {
                Amf3Helper.WriteText(writer, key);

                switch (value)
                {
                    case string str:
                        writer.Write((byte) Amf3Type.String);
                        Amf3Helper.WriteText(writer, str);
                        break;
                    case int integer:
                        writer.Write((byte) Amf3Type.Integer);
                        Amf3Helper.WriteNumber2(writer, (uint) integer);
                        break;
                    case uint unsigned:
                        writer.Write((byte) Amf3Type.Integer);
                        Amf3Helper.WriteNumber2(writer, unsigned);
                        break;
                    case bool boolean:
                        writer.Write((byte) (boolean ? Amf3Type.True : Amf3Type.False));
                        break;
                    case null:
                        writer.Write((byte) Amf3Type.Undefined);
                        break;
                }
            }

            writer.Write((byte) Amf3Type.Null);
            
            @this.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = @this,
                Content = stream.ToArray(),
                MessageName = name
            });
        }

        public static async Task MessageGuiAsync(this Player @this, string name)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Null);
            
            @this.Message(new UIMessageServerToSingleClientMessage
            {
                Associate = @this,
                Content = stream.ToArray(),
                MessageName = name
            });
        }

        public static void CentralNoticeGui(this Player @this, string text, int id = 0)
        {
            @this.Message(new NotifyClientFailedPreconditionMessage
            {
                Associate = @this,
                Id = id,
                Reason = text
            });
        }

        public static async Task StoryBoxGuiAsync(this Player @this, string text)
        {
            await StateAsync(@this, "Story");

            await Task.Delay(100);
            
            await @this.MessageGuiAsync("ToggleStoryBox", new Dictionary<string, object>()
            {
                {"visible", true},
                {"text", text}
            });
        }

        public static Task CloseMailboxGuiAsync(this Player @this) => ToggleAsync(@this, "ToggleMail", false);
        
        public static Task CountdownGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleFlashingText", state);
        
        public static Task BackpackGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleBackpack", state);
        
        public static Task GuildCreateGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleGuildCreate", state);
        
        public static Task GuildMenuGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleGuildUI", state);
        
        public static Task PassportGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "TogglePassport", state);
        
        public static Task HelpGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleHelpMenu", state);
        
        public static Task NewsGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleNews", state);
        
        public static Task FriendListGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleFriendsList", state);
        
        public static Task RespawnGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleRespawnDialog", state);
        
        public static Task PerformanceWarningGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "TogglePerformanceWarning", state);
        
        public static Task SpeedChatGuiAsync(this Player @this, bool state) => ToggleAsync(@this, "ToggleSC", state);
        
        public static Task OpenGuildChatAsync(this Player @this, bool state) => ToggleAsync(@this, "PlayerInGuild", state);
        
        public static Task CloseGuildChatAsync(this Player @this, bool state) => ToggleAsync(@this, "PlayerLeftGuild", state);
        
        public static Task OpenMailboxGuiAsync(this Player @this) => StateAsync(@this, "Mail");
    }
}