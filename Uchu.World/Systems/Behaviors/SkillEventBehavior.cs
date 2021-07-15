using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SkillEventBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SkillEvent;

        public override async Task BuildAsync()
        {
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            parameters.Context.Associate.OnSkillEvent.Invoke(parameters.BranchContext.Target, EffectHandler);
        }
    }
}
