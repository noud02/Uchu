using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class TargetCasterBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public TargetCasterBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class TargetCasterBehavior : BehaviorBase<TargetCasterBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.TargetCaster;

        private BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override void DeserializeStart(BitReader reader, TargetCasterBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
            parameters.Parameters.BranchContext.Target = parameters.Context.Associate;
        }

        protected override void ExecuteStart(TargetCasterBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }

        public override void Dismantle(BehaviorExecutionParameters parameters)
        {
            parameters.BranchContext.Target = parameters.Context.Associate;
            Action.Dismantle(parameters);
        }
    }
}