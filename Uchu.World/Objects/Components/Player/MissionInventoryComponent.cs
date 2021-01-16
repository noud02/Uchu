using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World
{
    /// <summary>
    /// Component responsible for missions and achievements a player has. Used for starting, updating and completing
    /// missions and achievements.
    /// </summary>
    public class MissionInventoryComponent : Component
    {
        public MissionInventoryComponent()
        {
            OnAcceptMission = new Event<MissionInstance>();
            OnCompleteMission = new Event<MissionInstance>();
            
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        /// <summary>
        /// Called when a player accepted a new mission, provides the mission that was accepted
        /// </summary>
        public Event<MissionInstance> OnAcceptMission { get; }
        
        /// <summary>
        /// Called when a player completed a mission, provides the mission that was completed
        /// </summary>
        public Event<MissionInstance> OnCompleteMission { get; }
        
        /// <summary>
        /// Complete list of missions this player has, either active or completed
        /// </summary>
        private List<MissionInstance> Missions { get; set; }

        /// <summary>
        /// Missions and achievements that the player has that are currently not completed. Provided as an array for
        /// memory safe access.
        /// </summary>
        private MissionInstance[] ActiveMissions
        {
            get
            {
                lock (Missions)
                {
                    return Missions.Where(m => m.State == MissionState.Active
                                               || m.State == MissionState.CompletedActive).ToArray();
                }
            }
        }
        
        /// <summary>
        /// Missions and achievements that a player has. Provided as an array for memory safe access.
        /// </summary>
        public MissionInstance[] AllMissions
        {
            get
            {
                lock (Missions)
                {
                    return Missions.ToArray();
                }
            }
        }

        /// <summary>
        /// Missions and achievements that a player has completed. Provided as an array for memory safe access.
        /// </summary>
        public MissionInstance[] CompletedMissions
        {
            get
            {
                lock (Missions)
                {
                    return Missions.Where(m => m.State == MissionState.Completed).ToArray();
                }
            }
        }
        
        /// <summary>
        /// Loads all player missions, combining the cd client and uchu database into mission instances
        /// </summary>
        private async Task LoadAsync()
        {
            if (GameObject is Player player)
            {
                await using var cdContext = new CdClientContext();
                await using var uchuContext = new UchuContext();

                // On load, load all the missions from database and store them in memory
                var missions = await uchuContext.Missions.Where(
                    m => m.CharacterId == GameObject.Id
                ).ToArrayAsync();

                Missions = new List<MissionInstance>();
                
                foreach (var mission in missions)
                {
                    var instance = new MissionInstance(mission.MissionId);
                    await instance.LoadAsync(cdContext, uchuContext, player);

                    lock (Missions)
                    {
                        Missions.Add(instance);
                    }
                }

                Listen(player.OnRespondToMission, async (missionId, receiver, rewardLot) =>
                {
                    await RespondToMissionAsync(missionId, receiver, rewardLot);
                });
            }
        }

        /// <summary>
        /// Whether a player has an active mission that has the provided id as mission id.
        /// </summary>
        /// <param name="id">The id of the mission to find in the mission inventory</param>
        /// <returns><c>true</c> if the player has an active mission with the given id, <c>false</c> otherwise</returns>
        public bool HasActive(int id)
        {
            lock (Missions)
            {
                return ActiveMissions.Any(m => m.MissionId == id);
            }
        }

        /// <summary>
        /// Checks if there's a mission that requires a certain item lot to be obtained
        /// </summary>
        /// <param name="lot">The lot to check for</param>
        /// <returns><c>true</c> if there's an active mission that requires the given item lot, <c>false</c> otherwise</returns>
        public bool HasActiveForItem(Lot lot)
        {
            lock (Missions)
            {
                return ActiveMissions
                    .SelectMany(m => m.Tasks)
                    .Any(t => t.Type == MissionTaskType.ObtainItem && t.Targets.Contains((int)lot));
            }
        }

        /// <summary>
        /// Whether a player has completed a mission that has the provided id as mission id.
        /// </summary>
        /// <param name="id">The id of the mission to find in the mission inventory</param>
        /// <returns><c>true</c> if the player has a completed mission with the given id, <c>false</c> otherwise</returns>
        public bool HasCompleted(int id)
        {
            lock (Missions)
            {
                return Missions.Any(m => m.MissionId == id && m.State >= MissionState.Completed);
            }
        }

        /// <summary>
        /// Whether a player has a mission that has the provided id as mission id.
        /// </summary>
        /// <param name="id">The id of the mission to find in the mission inventory</param>
        /// <returns><c>true</c> if the player has a mission with the given id, <c>false</c> otherwise</returns>
        public bool HasMission(int id)
        {
            lock (Missions)
            {
                return Missions.Any(m => m.MissionId == id);
            }
        }

        /// <summary>
        /// Returns the mission with a given id from the mission inventory.
        /// </summary>
        /// <param name="id">The id of the mission to get from the inventory</param>
        /// <returns>A mission instance if the player has this mission, <c>default</c> otherwise.</returns>
        public MissionInstance GetMission(int id)
        {
            lock (Missions)
            {
                return Missions.FirstOrDefault(m => m.MissionId == id);
            }
        }

        /// <summary>
        /// Checks if the player can accept a mission based on whether it's repeatable, already started and if the
        /// requirements are met.
        /// </summary>
        /// <param name="mission"></param>
        /// <returns><c>true</c> if the player can accept this mission, <c>false</c> otherwise</returns>
        public bool CanAccept(MissionInstance mission) => 
            (mission.CanRepeat || !HasMission(mission.MissionId)) 
            && MissionParser.CheckPrerequiredMissions(mission.PrerequisiteMissions, CompletedMissions);
        
        /// <summary>
        /// Checks if the player has a mission available that hasn't been started yet because of incorrect prerequisites.
        /// If the player now has the proper prerequisites this returns <c>true</c>.
        /// </summary>
        /// <param name="id">The mission id of the mission to check if the player has it available</param>
        /// <returns><c>true</c> if the player can accept this mission, <c>false</c> otherwise</returns>
        public bool HasAvailable(int id) => GetMission(id) is { } mission 
                                            && MissionParser.CheckPrerequiredMissions(mission.PrerequisiteMissions, CompletedMissions);

        /// <summary>
        /// Messages the client about a mission offer
        /// </summary>
        /// <param name="missionId">The id of the mission the mission to offer</param>
        /// <param name="missionGiver">The giver of the mission</param>
        public void MessageOfferMission(int missionId, GameObject missionGiver)
        {
            var player = (Player) GameObject;
            
            player.Message(new OfferMissionMessage
            {
                Associate = GameObject,
                MissionId = missionId,
                QuestGiver = missionGiver
            });
            
            player.Message(new OfferMissionMessage
            {
                Associate = missionGiver,
                MissionId = missionId,
                QuestGiver = missionGiver
            });
        }

        /// <summary>
        /// Makes the player respond to a mission offer, if not started it starts it, if not completed it show a repeat
        /// of the mission offer, if completable it completes it
        /// </summary>
        /// <param name="missionId">The id of the mission to respond to</param>
        /// <param name="missionGiver">The giver of the mission</param>
        /// <param name="rewardItem">Whether items should be rewarded (multi-select)</param>
        private async Task RespondToMissionAsync(int missionId, GameObject missionGiver, Lot rewardItem)
        {
            await using var uchuContext = new UchuContext();

            MissionInstance mission = GetMission(missionId);
            
            // If the user doesn't have this mission yet, start it
            if (mission == default)
            {
                await using var cdContext = new CdClientContext();

                var instance = new MissionInstance(missionId);
                await instance.LoadAsync(cdContext, uchuContext, (Player)GameObject);
                
                lock (Missions) {
                    Missions.Add(instance);
                }
                
                return;
            }
            
            // Player is responding to an active mission.
            if (!mission.Completed)
            {
                MessageOfferMission(missionId, missionGiver);
                return;
            }
            
            // Complete mission
            await mission.CompleteAsync(uchuContext, rewardItem);
            missionGiver?.GetComponent<MissionGiverComponent>().HandleInteraction((Player)GameObject);
        }

        /// <summary>
        /// Completes a mission
        /// </summary>
        /// <param name="missionId">The id of the mission to complete</param>
        public async Task CompleteMissionAsync(int missionId)
        {
            MissionInstance mission;
            lock (Missions)
            {
                mission = Missions.FirstOrDefault(m => m.MissionId == missionId);
            }
            
            await using var uchuContext = new UchuContext();
            
            // If the player is completing a mission that hasn't started, start it first
            if (mission == default)
            {
                await using var cdContext = new CdClientContext();
                
                var instance = new MissionInstance(missionId);
                await instance.LoadAsync(cdContext, uchuContext, (Player)GameObject);
                await instance.CompleteAsync(uchuContext);
                
                lock (Missions)
                {
                    Missions.Add(instance);
                }
                
                return;
            }

            await mission.CompleteAsync(uchuContext);
        }

        /// <summary>
        /// Finds all active tasks of a certain mission task type
        /// </summary>
        /// <typeparam name="T">The type of the mission task instance to use</typeparam>
        /// <returns>List of tasks that are active and of the seeked after type</returns>
        private IEnumerable<T> FindActiveTasksAsync<T>() where T : MissionTaskInstance => ActiveMissions
                .SelectMany(m => m.Tasks.OfType<T>().Where(t => !t.Completed));

        /// <summary>
        /// Progresses all the smash tasks using the provided lot
        /// </summary>
        /// <param name="lot">The lot to progress the smash tasks with</param>
        public async Task SmashAsync(Lot lot)
        {
            foreach (var task in FindActiveTasksAsync<SmashTask>())
            {
                await task.ReportProgress(lot);
            }

            await StartUnlockableAchievementsAsync<SmashTask>(MissionTaskType.Smash, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        /// <summary>
        /// Progresses all collect tasks using the game object that was collected
        /// </summary>
        /// <param name="gameObject">The game object that was collected</param>
        public async Task CollectAsync(GameObject gameObject)
        {
            foreach (var task in FindActiveTasksAsync<CollectTask>())
            {
                await task.ReportProgress(gameObject);
            }

            await StartUnlockableAchievementsAsync<CollectTask>(MissionTaskType.Collect, gameObject.Lot, async task =>
            {
                await task.ReportProgress(gameObject);
            });
        }

        /// <summary>
        /// Progresses all script tasks using the given scripted id
        /// </summary>
        /// <param name="id">The id to progress the script tasks with</param>
        /// <returns></returns>
        public async Task ScriptAsync(int id)
        {
            foreach (var task in FindActiveTasksAsync<ScriptTask>())
            {
                await task.ReportProgress(id);
            }

            await StartUnlockableAchievementsAsync<ScriptTask>(MissionTaskType.Script, id, async task =>
            {
                await task.ReportProgress(id);
            });
        }

        /// <summary>
        /// Progresses all quick build tasks using the lot of the quick build and the quickbuild activity id
        /// </summary>
        /// <param name="lot">The lot of the object that was build</param>
        /// <param name="activity">The id of the quickbuild activity</param>
        public async Task QuickBuildAsync(Lot lot, int activity)
        {
            foreach (var task in FindActiveTasksAsync<QuickBuildTask>())
            {
                await task.ReportProgress(lot, activity);
            }

            await StartUnlockableAchievementsAsync<QuickBuildTask>(MissionTaskType.QuickBuild, lot, async task =>
            {
                await task.ReportProgress(lot, activity);
            });
        }

        /// <summary>
        /// Progresses the go to npc tasks using the given lot
        /// </summary>
        /// <param name="lot">The lot of the object that was interacted with</param>
        public async Task GoToNpcAsync(Lot lot)
        {
            foreach (var task in FindActiveTasksAsync<GoToNpcTask>())
            {
                await task.ReportProgress(lot);
            }
            
            await StartUnlockableAchievementsAsync<GoToNpcTask>(MissionTaskType.GoToNpc, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }
        
        /// <summary>
        /// Progresses all the tasks of the interact type using the given lot
        /// </summary>
        /// <param name="lot">The lot to progress the interact tasks with</param>
        public async Task InteractAsync(Lot lot)
        {
            foreach (var task in FindActiveTasksAsync<InteractTask>())
            {
                await task.ReportProgress(lot);
            }
            
            await StartUnlockableAchievementsAsync<InteractTask>(MissionTaskType.Interact, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        /// <summary>
        /// Progresses the use emote tasks using the emote id
        /// </summary>
        /// <param name="gameObject">The game object that did the emote</param>
        /// <param name="emote">The id of the emote to progress the tasks with</param>
        public async Task UseEmoteAsync(GameObject gameObject, int emote)
        {
            foreach (var task in FindActiveTasksAsync<UseEmoteTask>())
            {
                await task.ReportProgress(gameObject, emote);
            }

            await StartUnlockableAchievementsAsync<UseEmoteTask>(MissionTaskType.UseEmote, emote, async task =>
            {
                await task.ReportProgress(gameObject, emote);
            });
        }

        /// <summary>
        /// Progresses all use consumable tasks using the given lot
        /// </summary>
        /// <param name="lot">The lot to progress the tasks with</param>
        public async Task UseConsumableAsync(Lot lot)
        {
            foreach (var task in FindActiveTasksAsync<UseConsumableTask>())
            {
                await task.ReportProgress(lot);
            }

            await StartUnlockableAchievementsAsync<UseConsumableTask>(MissionTaskType.UseConsumable, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        /// <summary>
        /// Progresses all use skill tasks using the given skill id
        /// </summary>
        /// <param name="skillId">The skill id to progress the tasks with</param>
        public async Task UseSkillAsync(int skillId)
        {
            foreach (var task in FindActiveTasksAsync<UseSkillTask>())
            {
                await task.ReportProgress(skillId);
            }

            await StartUnlockableAchievementsAsync<UseSkillTask>(MissionTaskType.UseSkill, skillId, async task =>
            {
                await task.ReportProgress(skillId);
            });
        }

        /// <summary>
        /// Progresses the obtain item tasks using the given lot
        /// </summary>
        /// <param name="lot">The lot to progress the obtain item tasks with</param>
        public async Task ObtainItemAsync(Lot lot)
        {
            foreach (var task in FindActiveTasksAsync<ObtainItemTask>())
            {
                await task.ReportProgress(lot);
            }

            await StartUnlockableAchievementsAsync<ObtainItemTask>(MissionTaskType.ObtainItem, lot, async task =>
            {
                await task.ReportProgress(lot);
            });
        }

        /// <summary>
        /// Progresses all the mission complete tasks using the mission id
        /// </summary>
        /// <param name="id">The id to progress the mission complete tasks with</param>
        public async Task MissionCompleteAsync(int id)
        {
            foreach (var task in FindActiveTasksAsync<MissionCompleteTask>())
            {
                await task.ReportProgress(id);
            }

            await StartUnlockableAchievementsAsync<MissionCompleteTask>(MissionTaskType.MissionComplete, id, async task =>
            {
                await task.ReportProgress(id);
            });
        }

        /// <summary>
        /// Progresses all flag tasks using the given flag
        /// </summary>
        /// <param name="flag">The flag to report to flag tasks</param>
        public async Task FlagAsync(int flag)
        {
            foreach (var task in FindActiveTasksAsync<FlagTask>())
            {
                await task.ReportProgress(flag);
            }

            await StartUnlockableAchievementsAsync<FlagTask>(MissionTaskType.Flag, flag, async task =>
            {
                await task.ReportProgress(flag);
            });
        }

        /// <summary>
        /// Returns a list of achievements that a player may start for a certain task type due to meeting it's prerequisites
        /// </summary>
        /// <remarks>
        /// A player may start an achievement if the achievement is of the requested type,
        ///  a player hasn't started it yet and the player has the proper prerequisites
        /// </remarks>
        /// <param name="type">The <see cref="MissionTaskType"/> of the achievement we seek</param>
        /// <param name="lot">The lot for which we check if there's an achievement attached to it</param>
        /// <typeparam name="T">The mission task instance type we want to look for, linked to <c>type</c> param</typeparam>
        /// <returns>A list of all the achievements a player can unlock, given the task type and the lot</returns>
        private MissionInstance[] UnlockableAchievements<T>(MissionTaskType type, Lot lot)
            where T : MissionTaskInstance => ClientCache.Achievements.Where(m =>
                m.Tasks.OfType<T>().Any(t => t.Type == type && t.Targets.Contains((int) lot))
                && CanAccept(m)).ToArray();

        /// <summary>
        /// Looks for possibly unlockable achievements given a mission task type and a lot and starts them
        /// </summary>
        /// <param name="type">The type of tasks we wish to look for achievements for</param>
        /// <param name="lot">The lot an achievement might have in its targets</param>
        /// <param name="progress">Progress callback function to call if an unlockable achievement is found</param>
        /// <typeparam name="T">The type of mission task type we wish to search for, linked to <c>type</c> param.</typeparam>
        private async Task StartUnlockableAchievementsAsync<T>(MissionTaskType type, Lot lot, Func<T, Task> progress = null)
            where T : MissionTaskInstance
        {
            foreach (var achievement in UnlockableAchievements<T>(type, lot))
            {
                // Loading these here instead of out of the loop might seem odd but heuristically the chances of starting a
                // new achievement are much lower than not starting an achievement, that's why doing this in the loop
                // allows us to open less db transactions in the long run
                await using var cdContext = new CdClientContext();
                await using var uchuContext = new UchuContext();
                
                var instance = new MissionInstance(achievement.MissionId);
                await instance.LoadAsync(cdContext, uchuContext, (Player)GameObject);
                
                lock (Missions)
                {
                    Missions.Add(instance);
                }
                
                // For achievements there's always only one task
                if (progress != null)
                    await progress(instance.Tasks.First() as T);
            }
        }
    }
}