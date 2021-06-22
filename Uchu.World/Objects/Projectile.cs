using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Systems.Behaviors;

namespace Uchu.World
{
    public class Projectile : Object
    {
        public long ClientObjectId { get; set; }
        
        public Lot Lot { get; set; }
        
        public GameObject Owner { get; set; }
        
        public GameObject Target { get; set; }
        
        public Vector3 Destination { get; set; }
        
        public float MaxDistance { get; set; }
        
        public float RadiusCheck { get; set; }

        public async Task ImpactAsync(byte[] data, GameObject target)
        {
            target ??= Target;
            
            await target.NetFavorAsync();

            var distance = Vector3.Distance(Destination, target.Transform.Position);
            if (distance > RadiusCheck)
                return;
            
            var tree = await BehaviorTree.FromLotAsync(Lot);
            await using var stream = new MemoryStream(data);
            var reader = new BitReader(stream);

            tree.Deserialize(Owner, reader, target: target, castType: SkillCastType.OnUse);
            tree.Use();
            
            Zone.BroadcastMessage(new DoClientProjectileImpactMessage()
            {
                Associate = Owner,
                Data = data,
                Owner = Owner,
                ProjectileId = ClientObjectId,
                Target = target
            });
        }

        public async Task CalculateImpactAsync(GameObject target)
        {
            target ??= Target;
            
            await target.NetFavorAsync();
            
            var distance = Vector3.Distance(Destination, target.Transform.Position);
            if (distance > RadiusCheck)
                return;
            
            var tree = await BehaviorTree.FromLotAsync(Lot);
            await using var stream = new MemoryStream();
            var writer = new BitWriter(stream);

            tree.Serialize(
                Owner,
                writer,
                tree.SkillRoots.First().Key,
                Owner.GetComponent<SkillComponent>().ClaimSyncId(),
                target.Transform.Position,
                target
            );
            
            tree.Execute();
            
            Zone.BroadcastMessage(new DoClientProjectileImpactMessage()
            {
                Associate = Owner,
                Data = stream.ToArray(),
                Owner = Owner,
                ProjectileId = ClientObjectId,
                Target = target
            });
        }
    }
}