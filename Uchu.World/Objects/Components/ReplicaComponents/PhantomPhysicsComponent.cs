using System.Linq;
using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Physics;
using Uchu.World.Client;

namespace Uchu.World
{
    public class PhantomPhysicsComponent : ReplicaComponent
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
                    var cdcComponent = ClientCache.Find<Core.Client.PhysicsComponent>(phantomPhysicsComponentId);
                    var assetPath = cdcComponent?.Physicsasset;

                    // Give physics object correct dimensions
                    var physicsComponent = GameObject.AddComponent<PhysicsComponent>();
                    physicsComponent.SetPhysicsByPath(assetPath);
                }
            });
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (writer.Flag(HasPosition))
            {
                writer.Write(Transform.Position);
                writer.Write(Transform.Rotation);
            }

            writer.WriteBit(true);
            
            if (!writer.Flag(IsEffectActive)) return;

            writer.Write((uint) EffectType);
            writer.Write(EffectAmount);

            if (writer.Flag(AffectedByDistance))
            {
                writer.Write(MinDistance);
                writer.Write(MaxDistance);
            }

            if (!writer.Flag(EffectDirection != Vector3.Zero)) return;

            writer.Write(EffectDirection * EffectAmount);
        }
    }
}