using RakDotNet.IO;

namespace Uchu.World
{
    public class ClientRailMovementReadyMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ClientRailMovementReady;
    }
}