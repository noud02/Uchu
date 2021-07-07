using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Filters
{
    public class RenderDistanceFilter : IPerspectiveFilter
    {
        public float Distance { get; set; }

        public Player Player { get; set; }
        
        public bool Override { get; set; }
        
        public Vector3 OverrideReferencePosition { get; set; }

        public void Initialize(Player player)
        {
            Player = player;
            var zone = ClientCache.Find<ZoneTable>((int) Player.Zone.ZoneId);

            Distance = zone?.Ghostdistance ?? 500;
        }

        public Task Tick()
        {
            return Task.CompletedTask;
        }

        public bool View(GameObject gameObject)
        {
            if (gameObject?.Transform == default) return false;

            if (gameObject is Player) return true;

            if (Override && Vector3.Distance(gameObject.Transform.Position, OverrideReferencePosition) <= Distance)
            {
                return true;
            }
            
            return Vector3.Distance(gameObject.Transform.Position, Player.Transform.Position) <= Distance;
        }
    }
}