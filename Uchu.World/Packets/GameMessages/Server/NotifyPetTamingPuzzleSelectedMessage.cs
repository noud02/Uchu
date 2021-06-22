using System.Collections.Generic;
using System.Linq;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct NotifyPetTamingPuzzleSelectedMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.NotifyPetTamingPuzzleSelected;
        public Brick[] Bricks { get; set; }
    }
}