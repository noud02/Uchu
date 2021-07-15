using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class RailHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task CancelRailMovementHandler(CancelRailMovementMessage message, Player player)
        {
            await player.OnCancelRailMovement.InvokeAsync();
        }

        [PacketHandler]
        public async Task ClientRailMovementReadyHandler(ClientRailMovementReadyMessage message, Player player)
        {
            await player.OnRailMovementReady.InvokeAsync();
        }
    }
}