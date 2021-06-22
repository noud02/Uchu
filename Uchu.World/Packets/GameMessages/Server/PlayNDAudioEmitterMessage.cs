using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct PlayNDAudioEmitterMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PlayNDAudioEmitter;
        [Default]
        public long NDAudioCallbackMessageData { get; set; }
        [Default]
        public long NDAudioEmitterID { get; set; }
        public string NDAudioEventGUID { get; set; }
        public string NDAudioMetaEventName { get; set; }
        public bool Result { get; set; }
        public ObjectId TargetObjectIDForNDAudioCallbackMessages { get; set; }
    }
}