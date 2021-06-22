using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ClientExitTamingMinigameMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ClientExitTamingMinigame;
		public bool VoluntaryExit { get; set; }
	}
}