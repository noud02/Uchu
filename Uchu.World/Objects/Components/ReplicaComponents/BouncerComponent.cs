using System.Numerics;

namespace Uchu.World
{
    public class BouncerComponent : StructReplicaComponent<BouncerSerialization>
    {
        public bool PetRequired { get; set; }
        
        public Vector3 BouncerDestination { get; set; }

        public override ComponentId Id => ComponentId.BouncerComponent;

        protected BouncerComponent()
        {
            Listen(OnStart, () =>
            {
                if (GameObject.Settings.TryGetValue("bouncer_destination", out var value))
                {
                    BouncerDestination = (Vector3) value;
                }
            });
        }
        
        
        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override BouncerSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.UnknownFlag = true;
            packet.PetNotRequired = !PetRequired;
            return packet;
        }
    }
}