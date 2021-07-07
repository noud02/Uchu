using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Systems.Missions
{
    /// <summary>
    /// Combined instance of the CdClient mission template and the Uchu data base mission progress.
    /// </summary>
    /// <remarks>
    /// Can be loaded in on world load or started during gameplay to be used for the player throughout the world.
    /// </remarks>
    public class MissionInstance
    {
        #region properties
        
        #region template
        /// <summary>
        /// The id of the mission in the CdClient
        /// </summary>
        public int MissionId { get; }
        
        /// <summary>
        /// The prerequisite missions of this mission
        /// </summary>
        public string PrerequisiteMissions { get; private set; }
        
        /// <summary>
        /// Whether this is a mission or an achievement
        /// </summary>
        public bool IsMission { get; private set; }
        
        /// <summary>
        /// If this mission has a reward that the player has to explicitly choose (e.g. one of 3 weapons).
        /// </summary>
        public bool IsChoiceReward { get; private set; }
        
        /// <summary>
        /// Meta information regarding the type of this mission
        /// </summary>
        public string DefinedType { get; private set; }
        
        /// <summary>
        /// Meta information regarding the sub type of this mission
        /// </summary>
        public string DefinedSubType { get; private set; }

        /// <summary>
        /// Currency that's rewarded once the mission is completed
        /// </summary>
        public long RewardCurrency { get; private set; }
        
        /// <summary>
        /// Currency that's rewarded once the mission has been completed more than once
        /// </summary>
        public long RewardCurrencyRepeatable { get; private set; }
        
        /// <summary>
        /// Lego Score that's rewarded once the mission has been completed
        /// </summary>
        public int RewardScore { get; private set; }
        
        /// <summary>
        /// Maximum health boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxHealth { get; private set; }
        
        /// <summary>
        /// Maximum imagination boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxImagination { get; private set; }
        
        /// <summary>
        /// Maximum inventory boost that's rewarded once the mission has been completed
        /// </summary>
        public int RewardMaxInventory { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote1 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote2 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote3 { get; private set; }
        
        /// <summary>
        /// Optional emote that's rewarded once the mission is completed
        /// </summary>
        public int RewardEmote4 { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem1 { get; private set; }
        
        /// <summary>
        /// The amount of the first item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem1Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem1Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the first item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem1RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem2 { get; private set; }
        
        /// <summary>
        /// The amount of the second item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem2Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem2Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the second item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem2RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem3 { get; private set; }
        
        /// <summary>
        /// The amount of the third item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem3Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem3Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of the third item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem3RepeatableCount { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem4 { get; private set; }
        
        /// <summary>
        /// The amount of the fourth item that's rewarded once the mission is completed
        /// </summary>
        public int RewardItem4Count { get; private set; }
        
        /// <summary>
        /// Optional item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem4Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of fourth item that's rewarded once the mission is completed more than once
        /// </summary>
        public int RewardItem4RepeatableCount { get; private set; }

        /// <summary>
        /// Additional space in the vault
        /// </summary>
        public int RewardBankInventory { get; private set; }

        #endregion template
        
        /// <summary>
        /// Whether the mission is part of mission of the day
        /// </summary>
        public bool InMissionOfTheDay { get; private set; }

        /// <summary>
        /// Cooldown time for repeating the mission.
        /// </summary>
        public long CooldownTime { get; private set; }
        
        #region instance
        /// <summary>
        /// The player that started this mission
        /// </summary>
        public Player Player { get; private set; }
        
        /// <summary>
        /// The current state of this mission for the player
        /// </summary>
        public MissionState State { get; private set; }
        
        /// <summary>
        /// Whether the player may repeat this mission
        /// </summary>
        public bool Repeatable { get; private set; }
        
        /// <summary>
        /// The amount of times the player completed this mission
        /// </summary>
        public int CompletionCount { get; private set; }
        
        /// <summary>
        /// The last time the player completed this mission
        /// </summary>
        public long LastCompletion { get; private set; }
        
        /// <summary>
        /// If this player is repeating this mission
        /// </summary>
        public bool Repeat => CompletionCount != 0;
        #endregion instance
        
        /// <summary>
        /// Map that contains all the possible mission tasks with their associated assembly types
        /// </summary>
        private static Dictionary<MissionTaskType, Type> TaskTypes { get; }
        
        /// <summary>
        /// All the tasks that need to be completed for this mission
        /// </summary>
        public List<MissionTaskInstance> Tasks { get; private set; }
        
        /// <summary>
        /// Checks if this mission is completed by checking if all sub-tasks are completed
        /// </summary>
        /// <returns><c>true</c> if completed, <c>false</c> otherwise</returns>
        public bool Completed => Tasks.All(t => t.Completed);
        
        /// <summary>
        /// Checks if this mission is can be repeated and the cooldown time is satisfied.
        /// </summary>
        /// <returns><c>true</c> if can be repeated right now, <c>false</c> otherwise</returns>
        public bool CanRepeat => IsMission && State == MissionState.Completed && (Repeatable || InMissionOfTheDay) && (LastCompletion == default || LastCompletion + (CooldownTime * 60) <= DateTimeOffset.Now.ToUnixTimeSeconds());
        
        #endregion properties
        static MissionInstance()
        {
            TaskTypes = new Dictionary<MissionTaskType, Type>()
            {
                [MissionTaskType.Smash] = typeof(SmashTask),
                [MissionTaskType.Script] = typeof(ScriptTask),
                [MissionTaskType.QuickBuild] = typeof(QuickBuildTask),
                [MissionTaskType.Collect] = typeof(CollectTask),
                [MissionTaskType.GoToNpc] = typeof(GoToNpcTask),
                [MissionTaskType.UseEmote] = typeof(UseEmoteTask),
                [MissionTaskType.UseConsumable] = typeof(UseConsumableTask),
                [MissionTaskType.UseSkill] = typeof(UseSkillTask),
                [MissionTaskType.ObtainItem] = typeof(ObtainItemTask),
                [MissionTaskType.Interact] = typeof(InteractTask),
                [MissionTaskType.MissionComplete] = typeof(MissionCompleteTask),
                [MissionTaskType.Flag] = typeof(FlagTask),
                [MissionTaskType.TamePet] = typeof(PetTameTask),
                [MissionTaskType.Discover] = typeof(DiscoverTask),
            };
        }
        
        public MissionInstance(int missionId, Player player)
        {
            MissionId = missionId;
            Player = player;
        }

        /// <summary>
        /// Loads the mission template from the cd client and optionally the mission instance from the uchu database
        /// if it's been started before.
        /// </summary>
        /// <param name="cdContext">The cd client context to use when loading the mission templates</param>
        /// <param name="uchuContext">Can be provided to load mission information from the database</param>
        public async Task LoadAsync(UchuContext uchuContext = default)
        {
            await LoadTemplateAsync();
            if (uchuContext != default)
                await LoadInstanceAsync(uchuContext);
        }

        /// <summary>
        /// Loads generic CdClient information about the mission.
        /// </summary>
        private async Task LoadTemplateAsync()
        {
            var mission = await ClientCache.FindAsync<Core.Client.Missions>(MissionId);

            PrerequisiteMissions = mission.PrereqMissionID;
            IsMission = mission.IsMission ?? true;
            IsChoiceReward = mission.IsChoiceReward ?? false;
            DefinedType = mission.Definedtype;
            DefinedSubType = mission.Definedsubtype;
            
            // Possible stat rewards
            RewardMaxHealth = mission.Rewardmaxhealth ?? 0;
            RewardMaxImagination = mission.Rewardmaximagination ?? 0;
            RewardMaxInventory = mission.Rewardmaxinventory ?? 0;
            RewardCurrency = mission.Rewardcurrency ?? 0;
            RewardCurrencyRepeatable = mission.Rewardcurrencyrepeatable ?? 0;
            RewardScore = mission.LegoScore ?? 0;
            Repeatable = mission.Repeatable ?? false;
            InMissionOfTheDay = mission.InMOTD ?? false;
            CooldownTime = mission.CooldownTime ?? 0;
            if (InMissionOfTheDay && CooldownTime == 0)
            {
                // Prevents infinite loop of getting and completing daily quest. ~22 hour timer is used by other daily quests.
                CooldownTime = 1300;
            }
            
            // Emotes
            RewardEmote1 = mission.Rewardemote ?? -1;
            RewardEmote2 = mission.Rewardemote2 ?? -1;
            RewardEmote3 = mission.Rewardemote3 ?? -1;
            RewardEmote4 = mission.Rewardemote4 ?? -1;
            
            // First optional reward item
            RewardItem1 = mission.Rewarditem1 ?? 0;
            RewardItem1Count = mission.Rewarditem1count ?? 1;
            RewardItem1Repeatable = mission.Rewarditem1repeatable ?? 0;
            RewardItem1RepeatableCount = mission.Rewarditem1repeatcount ?? 1;
            
            // Second optional reward item
            RewardItem2 = mission.Rewarditem2 ?? 0;
            RewardItem2Count = mission.Rewarditem2count ?? 1;
            RewardItem2Repeatable = mission.Rewarditem2repeatable ?? 0;
            RewardItem2RepeatableCount = mission.Rewarditem2repeatcount ?? 1;
            
            // Third optional reward item
            RewardItem3 = mission.Rewarditem3 ?? 0;
            RewardItem3Count = mission.Rewarditem3count ?? 1;
            RewardItem3Repeatable = mission.Rewarditem3repeatable ?? 0;
            RewardItem3RepeatableCount = mission.Rewarditem3repeatcount ?? 1;
            
            // Fourth optional reward item
            RewardItem4 = mission.Rewarditem4 ?? 0;
            RewardItem4Count = mission.Rewarditem4count ?? 1;
            RewardItem4Repeatable = mission.Rewarditem4repeatable ?? 0;
            RewardItem4RepeatableCount = mission.Rewarditem4repeatcount ?? 1;

            // Optional increment for vault inventory size
            RewardBankInventory = mission.Rewardbankinventory ?? 0;


            // Load all the tasks for this mission
            var tasks = await ClientCache.MissionTasksWithMissionIdCacheTable.FindAllAsync<MissionTasks>(MissionId);
            Tasks = new List<MissionTaskInstance>();

            var index = 0;
            foreach (var task in tasks)
            {
                var taskType = (MissionTaskType) (task.TaskType ?? 0);
                if (!TaskTypes.TryGetValue(taskType, out var type))
                {
                    Logger.Debug($"No {nameof(MissionTaskInstance)} for {taskType} found.");
                    continue;
                }

                if (task.Uid != null)
                {
                    var instance = (MissionTaskInstance) Activator.CreateInstance(type, this, task.Uid.Value,
                        index);
                    if (instance == default || task.Uid == default)
                    {
                        Logger.Error($"Invalid task: {type} [{task.Uid}]");
                        continue;
                    }
                
                    instance.LoadTemplate(task);
                    Tasks.Add(instance);
                }

                index++;
            }
        }
        
        /// <summary>
        /// Loads specific uchu context information about the mission.
        /// </summary>
        private async Task LoadInstanceAsync(UchuContext context)
        {
            // Mission instance information
            var mission = await context.Missions
                .Include(m => m.Tasks)
                .ThenInclude(t => t.Values)
                .FirstOrDefaultAsync(m => m.CharacterId == Player.Id && m.MissionId == MissionId);
            
            // Start the mission if it hasn't been started, otherwise load the database information
            if (mission != default)
            {
                State = (MissionState)mission.State;
                CompletionCount = mission.CompletionCount;
                LastCompletion = mission.LastCompletion;

                foreach (var task in Tasks)
                {
                    var taskInstance = mission.Tasks.First(t => t.TaskId == task.TaskId);
                    if (taskInstance == default)
                        continue;
                
                    task.LoadInstance(taskInstance);
                }
            }
        }

        /// <summary>
        /// Starts this mission instance and notifies the client
        /// </summary>
        public async Task StartAsync()
        {
            if (State != default)
                return;
            
            State = MissionState.Active;

            await NotifyClientStartAsync();
        }
        
        /// <summary>
        /// Restarts this mission instance and notifies the client
        /// </summary>
        public async Task RestartAsync()
        {
            State = MissionState.CompletedActive;
            foreach (var missionTask in Tasks)
            {
                missionTask.Restart();
            }

            await NotifyClientStartAsync();
        }

        /// <summary>
        /// Notifies the client of starting the mission
        /// </summary>
        private async Task NotifyClientStartAsync()
        {
            MessageNotifyMission();
            MessageMissionTypeState(MissionLockState.New);
            
            await SyncObtainItemTasksWithInventory();
            if (Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.OnAcceptMission.InvokeAsync(this);
            }
        }

        /// <summary>
        /// Syncs the obtain item tasks of a player with the inventory they currently have, can be used to shorttrack
        /// newly started tasks.
        /// </summary>
        private async Task SyncObtainItemTasksWithInventory()
        {
            if (!Player.TryGetComponent<InventoryManagerComponent>(out var inventoryManagerComponent))
                return;
            
            var items = inventoryManagerComponent.Items;
            
            foreach (var task in Tasks.OfType<ObtainItemTask>())
            {
                var toGive = items.Where(i => task.Targets.Contains((int) i.Lot))
                    .Sum(i => i.Count);

                for (var i = 0; i < toGive; i++)
                {
                    await task.ReportProgress(task.Target);
                    if (task.Completed)
                        break;
                }
            }
        }

        /// <summary>
        /// Completes this mission by updating the state, notifying the player and handing out all the rewards
        /// </summary>
        /// <param name="rewardItem">If this mission had a reward choice, this item will be chosen as reward</param>
        /// <exception cref="InvalidOperationException">If this mission hasn't been loaded yet</exception>
        public async Task CompleteAsync(Lot rewardItem = default)
        {
            if (State == default)
                await StartAsync();
                
            if (State == MissionState.Completed) 
               return;
            
            UpdateMissionState(MissionState.Unavailable, true);
            await SendRewardsAsync(rewardItem);
            
            LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds(); 
            State = MissionState.Completed; 
            CompletionCount++; 
            
            MessageNotifyMission();

            foreach (var item in Tasks)
            {
                if (item.Type != MissionTaskType.ObtainItem) continue;
                if (item.Parameters.Length == 0 || (item.Parameters[0] & 1) == 0)
                {
                    if (!Player.TryGetComponent<InventoryManagerComponent>(out var inventory)) continue;
                    await inventory.RemoveLotAsync(item.Target, (uint) item.RequiredProgress);
                }
            }
            
            if (!Player.TryGetComponent<MissionInventoryComponent>(out var missionInventory)) 
                return;
            
            await missionInventory.MissionCompleteAsync(MissionId); 
            await missionInventory.OnCompleteMission.InvokeAsync(this);
        }

        /// <summary>
        /// Sends the client all the rewards associated with this mission
        /// </summary>
        /// <param name="rewardItem">A specific item that should be rewarded, only
        /// rewarded if it's in one of the mission rewards.</param>
        private async Task SendRewardsAsync(Lot rewardItem)
        {
            await RewardCurrencyAndScoreAsync();
            RewardPlayerEmotes();
            RewardPlayerStats();
            await RewardPlayerLootAsync(rewardItem);
        }


        /// <summary>
        /// Rewards the player with currency based on the mission template
        /// </summary>
        /// <remarks>
        /// If this is an achievement the currency is updated silently without a notify message
        /// as the client updates the currency locally.
        /// </remarks>
        private async Task RewardCurrencyAndScoreAsync()
        {

            if (!Player.TryGetComponent<CharacterComponent>(out var character))
                return;
            
            var currency = Repeat ? RewardCurrencyRepeatable : RewardCurrency;
            var score = RewardScore;

            if (currency == 0 && score == 0) return;
            
            if (IsMission)
            {
                character.Currency += currency;
                character.UniverseScore += score;
            }
            else
            {
                // Achievement, client adds these itself so we don't need to notify
                character.AddCurrencySilently(currency);
                character.AddUniverseScoreSilently(score);

                // TODO: look into this, is it still necessary with the silent increment?
                // The client adds currency rewards as an offset, in my testing. Therefore we
                // have to account for this offset.
                character.HiddenCurrency += currency;
            }
        }
        
        /// <summary>
        /// Rewards the player with emotes that might be unlocked from completing this mission
        /// </summary>
        private void RewardPlayerEmotes()
        {
            if (!Player.TryGetComponent<CharacterComponent>(out var character))
                return;
            
            var emotes = new[] { RewardEmote1, RewardEmote2, RewardEmote3, RewardEmote4 };
            foreach (var i in emotes.Where(e => e != -1))
            {
                character.AddEmote(i);
            }
        }

        /// <summary>
        /// Rewards the mission instance player with the stat rewards associated to this mission
        /// </summary>
        private void RewardPlayerStats()
        {
            if (!Player.TryGetComponent<DestroyableComponent>(out var stats))
                return;
            
            stats.BoostBaseHealth((uint) RewardMaxHealth);
            stats.BoostBaseImagination((uint) RewardMaxImagination);
        }

        /// <summary>
        /// Rewards the player by giving them the optional loot associated with this mission and also
        /// optionally updating their bag size.
        /// </summary>
        /// <param name="rewardItem">A specific item that should be rewarded, only
        /// rewarded if it's in one of the mission rewards.</param>
        private async Task RewardPlayerLootAsync(Lot rewardItem)
        {
            if (!Player.TryGetComponent<InventoryManagerComponent>(out var inventory))
                return;
            
            inventory[InventoryType.Items].Size += Repeat ? 0 : RewardMaxInventory;
            // This increases the space for both VaultItems and VaultModels
            inventory[InventoryType.VaultItems].Size += RewardBankInventory;
            
            var rewards = new (Lot, int)[]
            {
                (Repeat ? RewardItem1Repeatable : RewardItem1, Repeat ? RewardItem1RepeatableCount : RewardItem1Count),
                (Repeat ? RewardItem2Repeatable : RewardItem2, Repeat ? RewardItem2RepeatableCount : RewardItem2Count),
                (Repeat ? RewardItem3Repeatable : RewardItem3, Repeat ? RewardItem3RepeatableCount : RewardItem3Count),
                (Repeat ? RewardItem4Repeatable : RewardItem4, Repeat ? RewardItem4RepeatableCount : RewardItem4Count),
            };

            if (IsChoiceReward)
            {
                var (_, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);
                await inventory.AddLotAsync(rewardItem, (uint) Math.Max(count, 1),
                    lootType: IsMission ? LootType.Mission: LootType.Achievement);
            }
            else
            {
                foreach (var (rewardLot, rewardCount) in rewards)
                {
                    if (rewardLot <= 0)
                        continue;
                    
                    var count = Math.Max(rewardCount, 1);
                    await inventory.AddLotAsync(rewardLot, (uint) count,
                        lootType: IsMission ? LootType.Mission: LootType.Achievement);
                }
            }
        }

        /// <summary>
        /// If this mission is an achievement, completes it. If this mission is a mission, allows the player to complete
        /// it at the mission giver.
        /// </summary>
        public async Task SoftCompleteAsync()
        {
            if (IsMission)
            {
                UpdateMissionState(State == MissionState.CompletedActive
                    ? MissionState.CompletedReadyToComplete
                    : MissionState.ReadyToComplete);
                return;
            }

            await CompleteAsync();
        }

        /// <summary>
        /// Updates the mission state by saving to the database and notifying the client
        /// </summary>
        /// <remarks>
        /// When editing multiple properties of the mission instance, set <c>State</c> explicitly,
        /// save using <c>SaveInstanceAsync</c> and notify the client using <c>MessageNotifyMission</c>
        /// afterwards, this results in less database sessions.
        /// </remarks>
        /// <param name="state">The new state to set this mission to</param>
        /// <param name="sendingRewards">Whether or not this state will result in rewards</param>
        public void UpdateMissionState(MissionState state, bool sendingRewards = false)
        {
            State = state;
            MessageNotifyMission(sendingRewards);
        }

        /// <summary>
        /// Notifies a player of the state of an active mission
        /// </summary>
        /// <param name="sendingRewards"></param>
        private void MessageNotifyMission(bool sendingRewards = false)
        {
            Player.Message(new NotifyMissionMessage
            {
                Associate = Player,
                MissionId = MissionId,
                MissionState = State,
                SendingRewards = sendingRewards
            });
        }

        /// <summary>
        /// Notifies the player of the lock state of a mission
        /// </summary>
        /// <param name="state">The state to send to the player</param>
        private void MessageMissionTypeState(MissionLockState state)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = Player,
                LockState = state,
                SubType = DefinedSubType,
                Type = DefinedType
            });
        }
    }
}