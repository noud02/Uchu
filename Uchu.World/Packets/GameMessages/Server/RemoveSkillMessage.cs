namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RemoveSkillMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RemoveSkill;
        public bool FromSkillSet { get; set; }
        public uint SkillId { get; set; }
    }
}