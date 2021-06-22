using System.Numerics;

namespace Uchu.World
{
    public class SimplePhysicsComponent : StructReplicaComponent<SimplePhysicsConstruct,SimplePhysicsSerialization>
    {
        public bool HasPosition { get; set; } = true;
        
        public bool HasVelocity { get; set; }
        
        public bool HasAirSpeed { get; set; }
        
        public Vector3 LinearVelocity { get; set; }
        
        public Vector3 AngularVelocity { get; set; }
        
        public uint AirSpeed { get; set; }

        public override ComponentId Id => ComponentId.SimplePhysicsComponent;

        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public override SimplePhysicsConstruct GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.UnknownFlag1 = false;
            packet.UnknownFloat = 0;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            return packet;
        }
        
        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public override SimplePhysicsSerialization GetSerializePacket()
        {
            var packet = base.GetSerializePacket();
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            return packet;
        }
    }
}