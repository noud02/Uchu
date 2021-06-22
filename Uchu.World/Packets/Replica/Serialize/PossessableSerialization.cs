using Uchu.Core;

namespace Uchu.World
{
    public struct PossessableSerialization
    {
        public bool UnknownFlag1 { get; set; }
        [Default]
        [Requires("UnknownFlag1")]
        public GameObject Driver { get; set; }
        [Requires("UnknownFlag1")]
        public bool UnknownFlag2 { get; set; }
        [Requires("UnknownFlag2")]
        public uint UnknownUint { get; set; }
        [Requires("UnknownFlag1")]
        public bool UnknownFlag3 { get; set; }
    }
}