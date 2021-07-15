using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    public class SkeletonBuffs : NativeScript
    {
        private readonly int[] _skeletons =
        {
            // 6551 // GF - TIKI STROMLING "Stromling Skeleton"
            12000, 12001, 12002, 12003, 12004, 12005, // Miner, engineer, patroller + named variants
            13068, // AM watchtower skeleton
            14007, 14008, 14009, 16511, 16191, // Frakjaw instance
            14024, 14025, 14026, 14027, 14028, 14029, 14491, // Normal NJ enemies
            13995, // Bone Wolf
            14576, // DPS NJ Skeleton Dummy
            16047, 16048, 16049, 16050, // Chopov, Frakjaw, Krazi, Bonezai
            16289, // Another Frakjaw
        };

        public override Task LoadAsync()
        {
            foreach (var skeleton in Zone.GameObjects.Where(IsSkeleton))
            {
                if (!skeleton.TryGetComponent<BuffComponent>(out var buffComponent))
                    buffComponent = skeleton.AddComponent<BuffComponent>();
                buffComponent.AddBuff(new BuffInfo {BuffId = 2});
            }

            Listen(Zone.OnObject, newObject =>
            {
                if (!(newObject is GameObject newSkeleton) || !IsSkeleton(newSkeleton))
                    return;
                if (!newSkeleton.TryGetComponent<BuffComponent>(out var buffComponent))
                    buffComponent = newSkeleton.AddComponent<BuffComponent>();
                buffComponent.AddBuff(new BuffInfo {BuffId = 2});
            });

            return Task.CompletedTask;
        }

        private bool IsSkeleton(GameObject gameObject)
        {
            return _skeletons.Contains(gameObject.Lot);
        }
    }
}