namespace Uchu.World
{
    public class RigidBodyPhantomPhysicsComponent : StructReplicaComponent<RigidBodyPhantomPhysicsSerialization>
    {
        public override ComponentId Id => ComponentId.RigidBodyPhantomPhysicsComponent;

        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override RigidBodyPhantomPhysicsSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.HasPosition = true;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            return packet;
        }
    }
}