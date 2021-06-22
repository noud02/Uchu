using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct SetPlayerControlSchemeMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.SetPlayerControlScheme;
        public bool DelayCameraSwitchIfInCinematic { get; set; }
        public bool SwitchCamera { get; set; }
        [Default]
        public int ControlScheme { get; set; }
    }
}