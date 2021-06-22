using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class ServerSideEventHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task EventHandler(FireEventServerSideMessage message, Player player)
        {
            await player.OnFireServerEvent.InvokeAsync(message.Arguments, message);
        }
    }
}