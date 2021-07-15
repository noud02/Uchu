using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ApplyBuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ApplyBuff;

        private bool _targetCaster;
        private BuffInfo _buffInfo;

        public override async Task BuildAsync()
        {
            _targetCaster = await GetParameter<bool>("target_caster");
            // Although there is a parameter called add_immunity,
            // no behavior has this set to anything other than 0
            _buffInfo = new BuffInfo
            {
                BuffId = await GetParameter<uint>("buff_id"),
                CancelOnDeath = await GetParameter<bool>("cancel_on_death"),
                CancelOnDamaged = await GetParameter<bool>("cancel_on_death"),
                CancelOnUnequip = await GetParameter<bool>("cancel_on_unequip"),
                CancelOnUi = await GetParameter<bool>("cancel_on_ui"),
                CancelOnZone = await GetParameter<bool>("cancel_on_zone"),
                ApplyOnTeammates = await GetParameter<bool>("apply_on_teammates"),
                CancelOnLogout = await GetParameter<bool>("cancel_on_logout"),
                CancelOnRemoveBuff = await GetParameter<bool>("cancel_on_remove_buff"),
                DurationSecs = await GetParameter<uint>("duration_secs"),
            };
            // TODO: apply on teammates
            Logger.Debug($"Building Buff id {_buffInfo.BuffId}");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            Logger.Debug($"Executing Buff id {_buffInfo.BuffId}");
            var target = _targetCaster ? parameters.Context.Associate : parameters.BranchContext.Target;
            if (!target.TryGetComponent<BuffComponent>(out var buffComponent))
                buffComponent = target.AddComponent<BuffComponent>();
            buffComponent.AddBuff(_buffInfo);
        }

        public override void Dismantle(BehaviorExecutionParameters parameters)
        {
            Logger.Debug($"Dismantling Buff id {_buffInfo.BuffId}");
            var target = _targetCaster ? parameters.Context.Associate : parameters.BranchContext.Target;
            if (!target.TryGetComponent<BuffComponent>(out var buffComponent))
                buffComponent = target.AddComponent<BuffComponent>();
            buffComponent.RemoveBuff(_buffInfo);
        }
    }
}
