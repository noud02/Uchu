using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class DiscoverTask : MissionTaskInstance
    {
        public DiscoverTask(MissionInstance mission, int taskId, int missionTaskIndex)
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Discover;

        public override bool Completed => Progress.Contains(Target);

        public async Task ReportProgress(string poiGroup)
        {
            // Need to check TargetGroup here but it won't contain the POI group bc it is not an int
            if (Target != poiGroup)
                return;

            // Progress is a list of ints, POI group is a string, this does not work
            AddProgress(poiGroup);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}