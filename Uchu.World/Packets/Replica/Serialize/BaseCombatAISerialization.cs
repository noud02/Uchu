using Uchu.Core;

namespace Uchu.World
{
    public struct BaseCombatAISerialization
    {
        public bool PerformingAction { get; set; }
        [Requires("PerformingAction")]
        public CombatAiAction Action { get; set; }
        [Requires("PerformingAction")]
        public GameObject Target { get; set; }
    }
}