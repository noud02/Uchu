using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [RequireComponent(typeof(StatsComponent), true)]
    public class CollectibleComponent : ReplicaComponent
    {
        public ushort CollectibleId { get; set; }

        public override ComponentId Id => ComponentId.CollectibleComponent;

        public CollectibleComponent()
        {
            OnStart.AddListener(() =>
            {
                CollectibleId = (ushort) (int) GameObject.Settings["collectible_id"];

                foreach (var stats in GameObject.GetComponents<StatsComponent>()) stats.HasStats = false;
            });
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.Write(CollectibleId);
        }
    }
}