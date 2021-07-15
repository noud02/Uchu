using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseSkillTask : MissionTaskInstance
    {
        public UseSkillTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.UseSkill;

        public override bool Completed => Progress.Count >= RequiredProgress;

        public async Task ReportProgress(int skillId, Lot? target = null)
        {
            if (!Parameters.Contains(skillId))
                return;

            // TODO: Check if target is correct, if applicable
            // This is only used for the Spinjitzu Initiate achievements (1139 and 1140).
            // They're supposed to progress when you smash one of the targets using the specified skill.
            // if (Targets.Any(x => x != 0) && (target == null || !Targets.Contains((int) target)))
            //     return;

            AddProgress(skillId);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}
