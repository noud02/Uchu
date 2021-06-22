using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct EchoStartSkillMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.EchoStartSkill;
        public bool UsedMouse { get; set; }
        [Default]
        public float CasterLatency { get; set; }
        [Default]
        public int CastType { get; set; }
        [Default]
        public Vector3 LastClickedPosition { get; set; }
        public GameObject OptionalOriginator { get; set; }
        [Default]
        public GameObject OptionalTarget { get; set; }
        [Default]
        [NiQuaternion]
        public Quaternion OriginatorRotation { get; set; }
        public byte[] Content { get; set; }
        public int SkillId { get; set; }
        [Default]
        public uint SkillHandle { get; set; }
    }
}