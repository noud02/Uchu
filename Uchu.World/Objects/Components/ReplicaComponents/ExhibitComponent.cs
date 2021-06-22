namespace Uchu.World
{
    public class ExhibitComponent : StructReplicaComponent<ExhibitSerialization>
    {
        public override ComponentId Id => ComponentId.ExhibitComponent;

        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override ExhibitSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.ExhibitedLot = this.GameObject.Lot;
            return packet;
        }
    }
}