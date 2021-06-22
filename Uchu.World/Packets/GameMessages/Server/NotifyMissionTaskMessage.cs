using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct NotifyMissionTaskMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.NotifyMissionTask;
        public int MissionId { get; set; }
        public int TaskIndex { get; set; }
        [StoreLengthAs(typeof(byte))]
        public float[] Updates { get; set; }
    }
}