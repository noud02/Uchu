using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    public class PetHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task PetTamingTryBuildHandler(PetTamingTryBuildMessage message, Player player)
        {
            await player.OnPetTamingTryBuild.InvokeAsync(message);
        }

        [PacketHandler]
        public void ClientExitTamingMinigameHandler(ClientExitTamingMinigameMessage message, Player player)
        {
            var msg = new NotifyPetTamingMinigameMessage();
            msg.NotifyType = PetTamingNotifyType.Quit;
            msg.PetId = (ObjectId)(ulong)0;
            msg.PlayerTaming = player;
            msg.ForceTeleport = false;
            msg.PetDestinationPosition = Vector3.Zero;
            msg.TeleportRotation = Quaternion.Identity;
            msg.TeleportPosition = Vector3.Zero;
            msg.Associate = player;
            player.Message(msg);
        }

        [PacketHandler]
        public async Task NotifyTamingBuildSuccessMessageHandler(NotifyTamingBuildSuccessMessage message, Player player)
        {
            await player.OnNotifyTamingBuildSuccessMessage.InvokeAsync(message);
        }
    }
}