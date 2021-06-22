using System.Numerics;

namespace Uchu.World
{
    public class ControllablePhysicsComponent : StructReplicaComponent<ControllablePhysicsConstruct,ControllablePhysicsSerialization>
    {
        public uint JetpackEffectId { get; set; }

        public bool Flying { get; set; }

        public float JetPackAirSpeed { get; set; } = 10;

        public bool HasPosition { get; set; } = true;

        public bool IsOnGround { get; set; } = true;

        public bool NegativeAngularVelocity { get; set; }

        public bool HasVelocity { get; set; }

        public Vector3 Velocity { get; set; }

        public bool HasAngularVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public GameObject Platform { get; set; }

        public Vector3 PlatformPosition { get; set; }

        public float GravityMultiplier { get; set; } = 1;

        public float SpeedMultiplier { get; set; } = 1;

        public override ComponentId Id => ComponentId.ControllablePhysicsComponent;
        
        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public override ControllablePhysicsConstruct GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.HasJetpackEffect = (JetpackEffectId != 0);
            packet.BypassFlyingChecks = false;
            packet.UnknownFlag1 = true;
            packet.HasSpeedOrGravityMultiplier = (!GravityMultiplier.Equals(1) || !SpeedMultiplier.Equals(1));
            packet.UnknownFlag2 = true;
            packet.UnknownUint8 = 0;
            packet.UnknownFlag3 = false;
            packet.UnknownFlag4 = true;
            packet.UnknownFlag5 = false;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            packet.IsOnRail = NegativeAngularVelocity;
            packet.HasPlatform = (Platform != null);
            packet.UnknownFlag7 = false;
            return packet;
        }
        
        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public override ControllablePhysicsSerialization GetSerializePacket()
        {
            var packet = base.GetSerializePacket();
            packet.HasSpeedOrGravityMultiplier = (!GravityMultiplier.Equals(1) || !SpeedMultiplier.Equals(1));
            packet.UnknownFlag1 = true;
            packet.UnknownUint1 = 0;
            packet.UnknownFlag2 = false;
            packet.UnknownFlag3 = true;
            packet.UnknownFlag4 = false;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            packet.IsOnRail = NegativeAngularVelocity;
            packet.HasPlatform = (Platform != null);
            packet.UnknownFlag6 = false;
            packet.UnknownFlag7 = false;
            return packet;
        }
    }
}