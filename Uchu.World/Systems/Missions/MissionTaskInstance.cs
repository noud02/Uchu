using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Systems.Missions
{
    /// <summary>
    /// Instance of a mission task, combines CdClient info and Uchu database info for an in-memory representation of both
    /// </summary>
    public abstract class MissionTaskInstance
    {
        public MissionTaskInstance(MissionInstance mission, int taskId, int missionTaskIndex)
        {
            Mission = mission;
            TaskId = taskId;
            MissionTaskIndex = missionTaskIndex;
            Progress = new List<float>();
        }

        #region properties
        
        /// <summary>
        /// The mission this task belongs to
        /// </summary>
        public MissionInstance Mission { get; private set; }
        
        /// <summary>
        /// The index of this task in the list of missions
        /// </summary>
        public int MissionTaskIndex { get; }
        
        /// <summary>
        /// The id of this task
        /// </summary>
        public int TaskId { get; private set; }
        
        /// <summary>
        /// A possible target of this task, for example a specific item to obtain
        /// </summary>
        public int Target { get; private set; }
        
        /// <summary>
        /// If there's more than one target, they're stored in this list
        /// </summary>
        private int[] TargetGroup { get; set; }

        private int[] _targets;

        public string TargetString { get; set; }
        
        /// <summary>
        /// All targets for this task, target + target group
        /// </summary>
        public int[] Targets
        {
            get
            {
                if (_targets == default)
                {
                    var targets = TargetGroup?.ToList() ?? new List<int>();
                    targets.Add(Target);
                    _targets = targets.ToArray();
                }
                
                return _targets;
            }
        }
        
        /// <summary>
        /// The complete overview of player progress for this task
        /// </summary>
        /// <example>
        /// If the task is to obtain 6 items and the player has collected 3 of them, this list might contain that lot
        /// 3 times to indicate its progress in the mission.
        /// </example>
        public List<float> Progress { get; private set; }
        
        /// <summary>
        /// The total progress (or values) this task requires for completion
        /// </summary>
        public int RequiredProgress { get; private set; }
        
        /// <summary>
        /// The current progress (or values) this task has
        /// </summary>
        public int CurrentProgress => Progress.Count;
        
        /// <summary>
        /// If the task is completed
        /// </summary>
        public virtual bool Completed => CurrentProgress >= RequiredProgress;
        
        /// <summary>
        /// Mission parameters, works basically like targets
        /// </summary>
        public int[] Parameters { get; private set; }
        
        /// <summary>
        /// The type of this task
        /// </summary>
        public abstract MissionTaskType Type { get; }
        #endregion properties

        /// <summary>
        /// Loads the information from a database task into this instance
        /// </summary>
        /// <param name="task">The database task to use as a template</param>
        public void LoadTemplate(MissionTasks task)
        {
            Target = task.Target ?? 0;
            RequiredProgress = task.TargetValue ?? 0;

            if (task.TargetGroup != default)
            {
                var targetGroup = task.TargetGroup
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();
                
                var targets = new List<int>();

                foreach (var target in targetGroup)
                {
                    if (int.TryParse(target, out var lot))
                    {
                        targets.Add(lot);
                    }
                }

                // Used for mission task type Discover and MinigameAchievement
                if (targets.Count == 0 && targetGroup.Count == 1 && targetGroup.First().Length > 0)
                    TargetString = targetGroup.First();

                TargetGroup = targets.ToArray();
            }
            else
            {
                TargetGroup = new int[0];
            }

            if (task.TaskParam1 != default)
            {
                var targetParameters = task.TaskParam1
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();
                
                var parameters = new List<int>();

                foreach (var parameter in targetParameters)
                {
                    if (int.TryParse(parameter, out var lot))
                    {
                        parameters.Add(lot);
                    }
                }

                Parameters = parameters.ToArray();
            }
            else
            {
                Parameters = new int[0];
            }
        }

        /// <summary>
        /// Loads the player progress for this task instance
        /// </summary>
        /// <param name="task">The player task information</param>
        public void LoadInstance(MissionTask task)
        {
            if (task.Values != null)
            {
                Progress = task.ValueArray().ToList();
            }
            else
            {
                Progress = new List<float>();
            }
        }

        /// <summary>
        /// Updates the progress for this task, a notification is sent to the client
        /// </summary>
        /// <param name="value">The value to add to the progress</param>
        /// <example>
        /// If the player were to obtain a certain amount of collectibles, <c>value</c> might be the item lot here.
        /// </example>
        protected void AddProgress(float value)
        {
            Progress.Add(value);
            MessageUpdateMissionTask();
        }

        /// <summary>
        /// Sends a message to the player updating them on the progress of a task
        /// </summary>
        private void MessageUpdateMissionTask()
        {
            Mission.Player.Message(new NotifyMissionTaskMessage
            {
                Associate = Mission.Player,
                MissionId = Mission.MissionId,
                TaskIndex = 1 << (MissionTaskIndex + 1),
                Updates = new[] { (float)CurrentProgress }
            });
        }

        /// <summary>
        /// Checks if the parent mission is complete when this task is completed, if so the mission is notified of
        /// completion.
        /// </summary>
        protected async Task CheckMissionCompletedAsync()
        {
            if (Mission.Completed)
                await Mission.SoftCompleteAsync();
        }
        
        /// <summary>
        /// Restarts the task status.
        /// </summary>
        public void Restart()
        {
            this.Progress = new List<float>();
        }
    }
}