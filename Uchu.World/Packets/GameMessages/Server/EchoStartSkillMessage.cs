using System;
using System.Numerics;
using InfectedRose.Core;
using RakDotNet.IO;

namespace Uchu.World
{
    public class EchoStartSkillMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.EchoStartSkill;

        public bool UsedMouse { get; set; }

        public float CasterLatency { get; set; }

        public int CastType { get; set; }

        public Vector3 LastClickedPosition { get; set; } = Vector3.Zero;

        public GameObject OptionalOriginator { get; set; }

        public GameObject OptionalTarget { get; set; }

        public Quaternion OriginatorRotation { get; set; } = Quaternion.Identity;

        public byte[] Content { get; set; }

        public int SkillId { get; set; }

        public uint SkillHandle { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(UsedMouse);

            var hasLatency = Math.Abs(CasterLatency) > 0.001;
            writer.WriteBit(hasLatency);
            if (hasLatency) writer.Write(CasterLatency);

            var hasCastType = CastType != default;
            writer.WriteBit(hasCastType);
            if (hasCastType) writer.Write(CastType);

            var hasClickPosition = LastClickedPosition != Vector3.Zero;
            writer.WriteBit(hasClickPosition);
            if (hasClickPosition) writer.Write(LastClickedPosition);

            writer.Write(OptionalOriginator);

            var hasTarget = OptionalTarget != default;
            writer.WriteBit(hasTarget);
            if (hasTarget) writer.Write(OptionalTarget);

            var hasRotation = OriginatorRotation != Quaternion.Identity;
            writer.WriteBit(hasRotation);
            if (hasRotation) writer.WriteNiQuaternion(OriginatorRotation);

            writer.Write((uint) Content.Length);
            foreach (var b in Content) writer.Write(b);

            writer.Write(SkillId);

            var hasHandle = SkillHandle != default;
            writer.WriteBit(hasHandle);
            if (hasHandle) writer.Write(SkillHandle);
        }
    }
}
