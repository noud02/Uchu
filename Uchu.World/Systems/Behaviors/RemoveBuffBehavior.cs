using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class RemoveBuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.RemoveBuff;

        private uint _buffId;

        public override async Task BuildAsync()
        {
            Logger.Debug($"Building RemoveBuff. Buff id {_buffId}");
            // Although there is a parameter called remove_immunity,
            // no behavior has this set to anything other than 0
            _buffId = await GetParameter<uint>("buff_id");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            Logger.Debug($"Executing RemoveBuff. Buff id {_buffId}");
            var target = parameters.BranchContext.Target;
            if (!target.TryGetComponent<BuffComponent>(out var buffComponent))
                buffComponent = target.AddComponent<BuffComponent>();
            buffComponent.RemoveBuffById(_buffId);
        }
    }
}