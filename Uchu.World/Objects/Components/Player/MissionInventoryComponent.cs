using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;
using Uchu.World.Objects.Components;
using Uchu.World.Systems.Missions;

namespace Uchu.World
{
    /// <summary>
    /// Component responsible for missions and achievements a player has. Used for starting, updating and completing
    /// missions and achievements.
    /// </summary>
    public class MissionInventoryComponent : Component, ISavableComponent
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
                await using var uchuContext = new UchuContext();

                // On load, load all the missions from database and store them in memory
                var missions = await uchuContext.Missions.Where(
                    m => m.CharacterId == GameObject.Id
                ).ToArrayAsync();

                Missions = new List<MissionInstance>();
                
                foreach (var mission in missions)
                {
                    var instance = new MissionInstance(mission.MissionId, player);
                    await instance.LoadAsync(uchuContext);

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
        /// Saves the mission inventory of the player
        /// </summary>
        /// <param name="context">The database context to save to</param>
        public async Task SaveAsync(UchuContext context)
        {
            var character = await context.Characters.Where(c => c.Id == GameObject.Id)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .FirstAsync();

            foreach (var mission in AllMissions)
            {
                var savedMission = character.Missions
                    .FirstOrDefault(m => m.MissionId == mission.MissionId);
                
                if (savedMission == default)
                {
                    character.Missions.Add(CreateMissionFromMissionInstance(character, mission));
                }
                else
                {
                    UpdateMissionFromMissionInstance(savedMission, mission);
                }
            }

            Logger.Debug($"Saved mission inventory for {GameObject}");
        }
        
        /// <summary>
        /// Creates a database mission from a mission instance
        /// </summary>
        /// <param name="character">The character to create the mission for</param>
        /// <param name="mission">The mission instance information to use for the mission</param>
        /// <returns>The mission with information from the mission instance</returns>
        private static Mission CreateMissionFromMissionInstance(Character character, MissionInstance mission) 
            => new Mission 
            {
                Character = character,
                MissionId = mission.MissionId,
                State = (int) mission.State,
                CompletionCount = mission.CompletionCount,
                LastCompletion = mission.LastCompletion,
                Tasks = mission.Tasks.Select(task => new MissionTask
                {
                    TaskId = task.TaskId,
                    Values = task.Progress
                        .GroupBy(value => value)
                        .Select(value => new MissionTaskValue
                        {
                            Value = value.Key,
                            Count = value.Count()
                        }).ToList()
                }).ToList()
            };

        /// <summary>
        /// Takes a saved mission and updates its values, tasks and progress according to a mission instance
        /// </summary>
        /// <param name="savedMission">The saved mission to save the information to</param>
        /// <param name="mission">The mission information to save to the saved mission</param>
        private static void UpdateMissionFromMissionInstance(Mission savedMission, MissionInstance mission)
        {
            // Update an existing mission
            savedMission.State = (int) mission.State;
            savedMission.CompletionCount = mission.CompletionCount;
            savedMission.LastCompletion = mission.LastCompletion;

            foreach (var task in mission.Tasks)
            {
                var savedTask = savedMission.Tasks.FirstOrDefault(t => t.TaskId == task.TaskId);
                if (savedTask == default)
                    continue;
                savedTask.Values = CreateMissionTaskValuesFromProgress(task.Progress);
            }
        }
        
        /// <summary>
        /// Transforms a progress array into a mission task value array
        /// </summary>
        /// <param name="progress">The progress array to transform</param>
        /// <returns>The array of database mission task values from the progress array</returns>
        private static List<MissionTaskValue> CreateMissionTaskValuesFromProgress(IEnumerable<float> progress) => progress
            .GroupBy(value => value)
            .Select(value => new MissionTaskValue
            {
                Value = value.Key,
                Count = value.Count()
            }).ToList();
        
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
        /// Whether a player has an active mission that has the provided id as mission id.
        /// </summary>
        /// <param name="id">The id of the mission to find in the mission inventory</param>
        /// <returns><c>true</c> if the player has an active mission with the given id, <c>false</c> otherwise</returns>
        public bool HasActive(MissionId id) => HasActive((int) id);

        /// <summary>
        /// Checks if there's a mission that requires a certain item lot to be obtained
        /// </summary>
        /// <param name="lot">The lot to check for</param>
        /// <returns><c>true</c> if there's an active mission that requires the given item lot, <c>false</c> otherwise</returns>
        public bool HasActiveForItem(Lot lot)
        {
            lock (Missions)
            {
                return FindActiveTasksAsync<ObtainItemTask>().Any(task => task.Targets.Contains(lot))
                       || UnlockableAchievements<ObtainItemTask>(MissionTaskType.ObtainItem, lot).Any();
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
        /// <param name="missionId">The id of the mission to get from the inventory</param>
        /// <returns>A mission instance if the player has this mission, <c>default</c> otherwise.</returns>
        public MissionInstance GetMission(MissionId missionId)
        {
            return GetMission((int) missionId);
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
        /// Creates and loads a new mission securely into the mission inventory
        /// </summary>
        /// <param name="missionId">The missionId to create a mission for</param>
        /// <param name="gameObject">The game object to assign the mission to</param>
        /// <returns>The newly created mission instance</returns>
        private async Task<MissionInstance> AddMissionAsync(int missionId, GameObject gameObject)
        {
            var mission = new MissionInstance(missionId, (Player)GameObject);
            await mission.LoadAsync();

            lock (Missions) {
                Missions.Add(mission);
            }

            return mission;
        }

        /// <summary>
        /// Checks if the player can accept a mission based on whether it's repeatable, already started and if the
        /// requirements are met.
        /// </summary>
        /// <param name="mission"></param>
        /// <returns><c>true</c> if the player can accept this mission, <c>false</c> otherwise</returns>
        public bool CanAccept(MissionInstance mission) => 
            (mission.CanRepeat || !HasMission(mission.MissionId)) 
            && MissionParser.CheckPrerequiredMissions(mission.PrerequisiteMissions, AllMissions);
        
        /// <summary>
        /// Checks if the player has a mission available that hasn't been started yet because of incorrect prerequisites.
        /// If the player now has the proper prerequisites this returns <c>true</c>.
        /// </summary>
        /// <param name="id">The mission id of the mission to check if the player has it available</param>
        /// <returns><c>true</c> if the player can accept this mission, <c>false</c> otherwise</returns>
        public bool HasAvailable(int id) => GetMission(id) is { } mission 
                                            && MissionParser.CheckPrerequiredMissions(mission.PrerequisiteMissions, AllMissions);

        /// <summary>
        /// Messages the client about a mission offer
        /// </summary>
        /// <param name="missionId">The id of the mission the mission to offer</param>
        /// <param name="missionGiver">The giver of the mission</param>
        public void MessageOfferMission(int missionId, GameObject missionGiver)
        {
            if (!(GameObject is Player player))
                return;
            
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
            var mission = GetMission(missionId);
            
            // If the user doesn't have this mission yet, start it
            if (mission == default)
            {
                mission = await AddMissionAsync(missionId, GameObject);
                await mission.StartAsync();
                return;
            }
            
            // Repeat the mission if it is repeatable, such as a daily mission.
            if (mission.CanRepeat)
            {
                await mission.RestartAsync();
                return;
            }
            
            // Player is responding to an active mission.
            if (!mission.Completed)
            {
                MessageOfferMission(missionId, missionGiver);
                return;
            }
            
            // Complete mission
            await mission.CompleteAsync(rewardItem);
            missionGiver?.GetComponent<MissionGiverComponent>().HandleInteraction((Player)GameObject);
        }

        /// <summary>
        /// Completes a mission
        /// </summary>
        /// <param name="missionId">The id of the mission to complete</param>
        public async Task CompleteMissionAsync(int missionId)
        {
            // If the mission can't be found, complete it
            var mission = GetMission(missionId) ?? await AddMissionAsync(missionId, GameObject);
            await mission.CompleteAsync();
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
        /// <param name="taskId">The relevant script task ID</param>
        /// <param name="target">The target linked to this script task</param>
        /// <returns></returns>
        public async Task ScriptAsync(int taskId, Lot target)
        {
            foreach (var task in FindActiveTasksAsync<ScriptTask>())
            {
                await task.ReportProgress(taskId, target);
            }

            await StartUnlockableAchievementsAsync<ScriptTask>(MissionTaskType.Script, target, async task =>
            {
                await task.ReportProgress(taskId, target);
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
        /// Progresses discover tasks using POI name
        /// </summary>
        /// <param name="poiGroup">The entered POI</param>
        public async Task DiscoverAsync(string poiGroup)
        {
            Logger.Information($"discoverAsync {poiGroup}");
            foreach (var task in FindActiveTasksAsync<DiscoverTask>())
            {
                await task.ReportProgress(poiGroup);
            }

            await StartUnlockableAchievementsAsync<DiscoverTask>(MissionTaskType.Discover, poiGroup, async task =>
            {
                await task.ReportProgress(poiGroup);
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
        /// Progresses the taming pet tasks.
        /// </summary>
        /// <param name="Pet">the lot of the tamed pet</param>
        /// <returns>¯\_(ツ)_/¯</returns>
        public async Task TamePetAsync(Lot Pet)
        {
            foreach (var task in FindActiveTasksAsync<PetTameTask>())
            {
                await task.ReportProgress(Pet);
            }
            
            await StartUnlockableAchievementsAsync<PetTameTask>(MissionTaskType.TamePet, Pet, async task =>
            {
                await task.ReportProgress(Pet);
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
            m.Tasks.OfType<T>().Any(t => t.Type == type && (t.Targets.Contains((int) lot) || t.Parameters.Contains((int) lot)))
            && CanAccept(m)).ToArray();

        /// <summary>
        /// Returns a list of achievements that a player may start for a certain task type due to meeting it's prerequisites
        /// </summary>
        /// <remarks>
        /// A player may start an achievement if the achievement is of the requested type,
        ///  a player hasn't started it yet and the player has the proper prerequisites
        /// </remarks>
        /// <param name="type">The <see cref="MissionTaskType"/> of the achievement we seek</param>
        /// <param name="targetGroup">The string to search targetGroups of achievements for</param>
        /// <typeparam name="T">The mission task instance type we want to look for, linked to <c>type</c> param</typeparam>
        /// <returns>A list of all the achievements a player can unlock, given the task type and the lot</returns>
        private MissionInstance[] UnlockableAchievements<T>(MissionTaskType type, string targetGroup = null)
            where T : MissionTaskInstance => ClientCache.Achievements.Where(m =>
            m.Tasks.OfType<T>().Any(t => t.Type == type && t.TargetString == targetGroup)
            && CanAccept(m)).ToArray();

        /// <summary>
        /// Looks for possibly unlockable achievements given a mission task type and a lot and starts them
        /// </summary>
        /// <param name="type">The type of tasks we wish to look for achievements for</param>
        /// <param name="targetGroup">The targetGroup an achievement task might have</param>
        /// <param name="progress">Progress callback function to call if an unlockable achievement is found</param>
        /// <typeparam name="T">The type of mission task type we wish to search for, linked to <c>type</c> param.</typeparam>
        private async Task StartUnlockableAchievementsAsync<T>(MissionTaskType type, string targetGroup, Func<T, Task> progress = null)
            where T : MissionTaskInstance
        {
            foreach (var achievement in UnlockableAchievements<T>(type, targetGroup))
            {
                // Loading these here instead of out of the loop might seem odd but heuristically the chances of starting a
                // new achievement are much lower than not starting an achievement, that's why doing this in the loop
                // allows us to open less db transactions in the long run
                var mission = await AddMissionAsync(achievement.MissionId, GameObject);
                await mission.StartAsync();

                if (progress == null) continue;

                foreach (var task in mission.Tasks)
                {
                    await progress(task as T);
                }
            }
        }

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
                var mission = await AddMissionAsync(achievement.MissionId, GameObject);
                await mission.StartAsync();

                if (progress == null) continue;

                foreach (var task in mission.Tasks)
                {
                    await progress(task as T);
                }
            }
        }
    }
}
