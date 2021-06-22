using System.Linq;
using System.Numerics;
using Uchu.World.Client;

namespace Uchu.World
{
    public class PhantomPhysicsComponent : StructReplicaComponent<PhantomPhysicsSerialization>
    {
        public override ComponentId Id => ComponentId.PhantomPhysicsComponent;

        public bool HasPosition { get; set; } = true;

        public bool IsEffectActive { get; set; }

        public PhantomPhysicsEffectType EffectType { get; set; }

        public float EffectAmount { get; set; }
        
        public bool AffectedByDistance { get; set; }

        public float MinDistance { get; set; }
        
        public float MaxDistance { get; set; }
        
        public Vector3 EffectDirection { get; set; }

        protected PhantomPhysicsComponent()
        {
            Listen(OnStart, () =>
            {
                // This is for POIs with a POI key in the settings, used for example everywhere in AG except for
                // Monument Orange Path (AG_Mon_3).
                // Not all POIs are handled this way, for example those in GF are done using triggers.
                if (GameObject.Settings.ContainsKey("POI"))
                {
                    // Find physics asset path from cdclient
                    var phantomPhysicsComponentId = GameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                    var cdcComponent = ClientCache.GetTable<Core.Client.PhysicsComponent>()
                        .FirstOrDefault(r => r.Id == phantomPhysicsComponentId);
                    var assetPath = cdcComponent?.Physicsasset;

                    // Give physics object correct dimensions
                    var physicsComponent = GameObject.AddComponent<PhysicsComponent>();
                    physicsComponent.SetPhysicsByPath(assetPath);
                }
            });
        }
        
        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override PhantomPhysicsSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            packet.UnknownFlag = true;
            packet.EffectDirectionScaled = EffectDirection * EffectAmount;
            return packet;
        }
    }
}