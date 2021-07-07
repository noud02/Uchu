using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;
using Uchu.World.Objects.Components;

namespace Uchu.World
{
    public class CharacterComponent : ReplicaComponent, ISavableComponent
    {
        public CharacterComponent()
        {
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        private async Task LoadAsync()
        {
            await using var uchuContext = new UchuContext();
                
            var character = await uchuContext.Characters
                .Include(c => c.Flags)
                .Include(c => c.UnlockedEmotes)
                .FirstOrDefaultAsync(c => c.Id == GameObject.Id);
            
            if (character == default)
                return;

            Currency = character.Currency;
            _universeScore = character.UniverseScore;
            await SetLevelAsync(character.Level, false);

            BaseHealth = character.BaseHealth;
            BaseImagination = character.BaseImagination;
            LastZone = character.LastZone == 0 ? 1000 : character.LastZone;
            
            // Cache all character emotes
            foreach (var unlockedEmote in character.UnlockedEmotes)
            {
                AddEmote(unlockedEmote.EmoteId);
            }
            
            // Cache all character flags
            foreach (var flag in character.Flags.Where(flag => flag.Flag != default))
            {
                await SetFlagAsync(flag.Flag, true, true);
            }

            HairColor = character.HairColor;
            HairStyle = character.HairStyle;
            ShirtColor = character.ShirtColor;
            PantsColor = character.PantsColor;
            EyebrowStyle = character.EyebrowStyle;
            EyeStyle = character.EyeStyle;
            MouthStyle = character.MouthStyle;
            TotalCurrencyCollected = character.TotalCurrencyCollected;
            TotalBricksCollected = character.TotalBricksCollected;
            TotalSmashablesSmashed = character.TotalSmashablesSmashed;
            TotalQuickBuildsCompleted = character.TotalQuickBuildsCompleted;
            TotalEnemiesSmashed = character.TotalEnemiesSmashed;
            TotalRocketsUsed = character.TotalRocketsUsed;
            TotalMissionsCompleted = character.TotalMissionsCompleted;
            TotalPetsTamed = character.TotalPetsTamed;
            TotalImaginationPowerUpsCollected = character.TotalImaginationPowerUpsCollected;
            TotalLifePowerUpsCollected = character.TotalLifePowerUpsCollected;
            TotalArmorPowerUpsCollected = character.TotalArmorPowerUpsCollected;
            TotalDistanceTraveled = character.TotalDistanceTraveled;
            TotalSuicides = character.TotalSuicides;
            TotalDamageTaken = character.TotalDamageTaken;
            TotalDamageHealed = character.TotalDamageHealed;
            TotalArmorRepaired = character.TotalArmorRepaired;
            TotalImaginationRestored = character.TotalImaginationRestored;
            TotalImaginationUsed = character.TotalImaginationUsed;
            TotalDistanceDriven = character.TotalDistanceDriven;
            TotalTimeAirborne = character.TotalTimeAirborne;
            TotalRacingImaginationPowerUpsCollected = character.TotalRacingImaginationPowerUpsCollected;
            TotalRacingImaginationCratesSmashed = character.TotalRacingImaginationCratesSmashed;
            TotalRacecarBoostsActivated = character.TotalRacecarBoostsActivated;
            TotalRacecarWrecks = character.TotalRacecarWrecks;
            TotalRacingSmashablesSmashed = character.TotalRacingSmashablesSmashed;
            TotalRacesFinished = character.TotalRacesFinished;
            TotalFirstPlaceFinishes = character.TotalFirstPlaceFinishes;
            LastActivity = character.LastActivity;
            FreeToPlay = character.FreeToPlay;
            LandingByRocket = character.LandingByRocket;
            Rocket = character.Rocket;
            LaunchedRocketFrom = character.LaunchedRocketFrom;
            CharacterId = character.Id;
            InventorySize = character.InventorySize;
            VaultInventorySize = character.VaultInventorySize;
            Lh = character.Lh;
            Rh = character.Rh;
            Name = character.Name;
        }
        
        /// <summary>
        /// Saves the contents of this component to the database
        /// </summary>
        /// <param name="uchuContext">The context to save to</param>
        public async Task SaveAsync(UchuContext context)
        {
            var character = await context.Characters
                .Include(c => c.Flags)
                .Include(c => c.UnlockedEmotes)
                .FirstOrDefaultAsync(c => c.Id == GameObject.Id);
            
            if (character == default)
                return;

            character.Currency = Currency;
            character.UniverseScore = UniverseScore;
            character.Level = Level;
            character.BaseHealth = BaseHealth;
            character.BaseImagination = BaseImagination;
            character.LastZone = LastZone;
            character.HairColor = HairColor;
            character.HairStyle = HairStyle;
            character.ShirtColor = ShirtColor;
            character.PantsColor = PantsColor;
            character.EyebrowStyle = EyebrowStyle;
            character.EyeStyle = EyeStyle;
            character.MouthStyle = MouthStyle;
            character.TotalCurrencyCollected = TotalCurrencyCollected;
            character.TotalBricksCollected = TotalBricksCollected;
            character.TotalSmashablesSmashed = TotalSmashablesSmashed;
            character.TotalQuickBuildsCompleted = TotalQuickBuildsCompleted;
            character.TotalEnemiesSmashed = TotalEnemiesSmashed;
            character.TotalRocketsUsed = TotalRocketsUsed;
            character.TotalMissionsCompleted = TotalMissionsCompleted;
            character.TotalPetsTamed = TotalPetsTamed;
            character.TotalImaginationPowerUpsCollected = TotalImaginationPowerUpsCollected;
            character.TotalLifePowerUpsCollected = TotalLifePowerUpsCollected;
            character.TotalArmorPowerUpsCollected = TotalArmorPowerUpsCollected;
            character.TotalDistanceTraveled = TotalDistanceTraveled;
            character.TotalSuicides = TotalSuicides;
            character.TotalDamageTaken = TotalDamageTaken;
            character.TotalDamageHealed = TotalDamageHealed;
            character.TotalArmorRepaired = TotalArmorRepaired;
            character.TotalImaginationRestored = TotalImaginationRestored;
            character.TotalImaginationUsed = TotalImaginationUsed;
            character.TotalDistanceDriven = TotalDistanceDriven;
            character.TotalTimeAirborne = TotalTimeAirborne;
            character.TotalRacingImaginationPowerUpsCollected = TotalRacingImaginationPowerUpsCollected;
            character.TotalRacingImaginationCratesSmashed = TotalRacingImaginationCratesSmashed;
            character.TotalRacecarBoostsActivated = TotalRacecarBoostsActivated;
            character.TotalRacecarWrecks = TotalRacecarWrecks;
            character.TotalRacingSmashablesSmashed = TotalRacingSmashablesSmashed;
            character.TotalRacesFinished = TotalRacesFinished;
            character.TotalFirstPlaceFinishes = TotalFirstPlaceFinishes;
            character.LastActivity = LastActivity;
            character.FreeToPlay = FreeToPlay;
            character.LandingByRocket = LandingByRocket;
            character.Rocket = Rocket;
            character.LaunchedRocketFrom = LaunchedRocketFrom;
            character.InventorySize = InventorySize;
            character.VaultInventorySize = VaultInventorySize;
            character.Lh = Lh;
            character.Rh = Rh;
            character.Name = Name;

            // Save all newly unlocked emotes
            foreach (var newlyUnlockedEmoteId in Emotes.Where(emoteId => 
                character.UnlockedEmotes.All(emote => emote.EmoteId != emoteId)))
            {
                character.UnlockedEmotes.Add(new UnlockedEmote
                {
                    EmoteId = newlyUnlockedEmoteId
                });
            }
            
            // Save all newly unlocked flags
            foreach (var newlyUnlockedFlag in Flags.Where(flagId => 
                character.Flags.All(flag => flag.Flag != flagId)))
            {
                character.Flags.Add(new CharacterFlag
                {
                    Flag = newlyUnlockedFlag
                });
            }

            Logger.Debug($"Saved character for {GameObject}");
        }
        
        public int LastZone { get; set; }
        public GameObject VehicleObject { get; set; }
        
        public bool IsPvP { get; set; }

        public bool IsGameMaster { get; set; }

        public byte GameMasterLevel { get; set; }

        public CharacterActivity Activity { get; set; }

        public long GuildId { get; set; } = -1;

        public string GuildName { get; set; }

        public override ComponentId Id => ComponentId.CharacterComponent;
        
        #region traits
        public string Name { get; private set; }
        public long CharacterId { get; private set; }
        public int InventorySize { get; set; }
        public int VaultInventorySize { get; set; }
        public int BaseImagination { get; set; }
        public int BaseHealth { get; set; }
        public string Rocket { get; set; }
        public int LaunchedRocketFrom { get; set; }
        public long LastActivity { get; private set; }
        public bool FreeToPlay { get; private set; }
        public bool LandingByRocket { get; set; }
        public long Rh { get; private set; }
        public long Lh { get; private set; }
        public long HairColor { get; private set; }
        public long HairStyle { get; private set; }
        public long ShirtColor { get; private set; }
        public long PantsColor { get; private set; }
        public long EyebrowStyle { get; private set; }
        public long EyeStyle { get; private set; }
        public long MouthStyle { get; private set; }
        public long TotalCurrencyCollected { get; private set; }
        public long TotalBricksCollected { get; private set; }
        public long TotalSmashablesSmashed { get; private set; }
        public long TotalQuickBuildsCompleted { get; private set; }
        public long TotalEnemiesSmashed { get; private set; }
        public long TotalRocketsUsed { get; private set; }
        public long TotalMissionsCompleted { get; private set; }
        public long TotalPetsTamed { get; private set; }
        public long TotalImaginationPowerUpsCollected { get; private set; }
        public long TotalLifePowerUpsCollected { get; private set; }
        public long TotalArmorPowerUpsCollected { get; private set; }
        public long TotalDistanceTraveled { get; private set; }
        public long TotalSuicides { get; private set; }
        public long TotalDamageTaken { get; private set; }
        public long TotalDamageHealed { get; private set; }
        public long TotalArmorRepaired { get; private set; }
        public long TotalImaginationRestored { get; private set; }
        public long TotalImaginationUsed { get; private set; }
        public long TotalDistanceDriven { get; private set; }
        public long TotalTimeAirborne { get; private set; }
        public long TotalRacingImaginationPowerUpsCollected { get; private set; }
        public long TotalRacingImaginationCratesSmashed { get; private set; }
        public long TotalRacecarBoostsActivated { get; private set; }
        public long TotalRacecarWrecks { get; private set; }
        public long TotalRacingSmashablesSmashed { get; private set; }
        public long TotalRacesFinished { get; private set; }
        public long TotalFirstPlaceFinishes { get; private set; }
        #endregion traits
        
        #region emotes
        /// <summary>
        /// The emotes this player has unlocked
        /// </summary>
        private HashSet<int> Emotes { get; } = new HashSet<int>();

        /// <summary>
        /// Adds an emote to the player emote inventory
        /// </summary>
        /// <param name="emoteId"></param>
        public void AddEmote(int emoteId)
        {
            if (Emotes.Contains(emoteId))
                return;
            
            Emotes.Add(emoteId);

            if (GameObject is Player player)
            {
                player.Message(new SetEmoteLockStateMessage
                {
                    Associate = player,
                    EmoteId = emoteId,
                    Lock = false
                });
            }
        }
        #endregion emotes
        
        #region flags
        
        /// <summary>
        /// The flags this player has
        /// </summary>
        private HashSet<int> Flags { get; } = new HashSet<int>();

        /// <summary>
        /// List of all the flags this user has unlocked
        /// </summary>
        public IEnumerable<int> FlagsList => Flags.ToList();
        
        /// <summary>
        /// Returns the flag value for a flag id
        /// </summary>
        /// <param name="flagId">The flag to find for the player</param>
        /// <returns><c>true</c> or <c>false</c> based on whether the player has the flag or not</returns>
        public bool GetFlag(int flagId) => Flags.Contains(flagId);
        
        /// <summary>
        /// Adds or removes a flag from the player based on the <c>state</c>
        /// </summary>
        /// <param name="flagId">The id of the flag to change</param>
        /// <param name="state"><c>true</c> if the flag should be added, <c>false</c> if the flag should be removed</param>
        /// <param name="silent">Whether mission progress and user flag updates are sent</param>
        public async Task SetFlagAsync(int flagId, bool state, bool silent = false)
        {
            if (state)
            {
                if (!silent && GameObject.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    await missionInventory.FlagAsync(flagId);

                if (!GetFlag(flagId))
                    Flags.Add(flagId);
            }
            else if (GetFlag(flagId))
            {
                Flags.Remove(flagId);
            }

            if (!silent && GameObject is Player player)
            {
                player.Message(new NotifyClientFlagChangeMessage
                {
                    Associate = player,
                    Flag = state,
                    FlagId = flagId
                });
            }
        }

        /// <summary>
        /// Whether a player has a certain faction or not
        /// </summary>
        /// <param name="factionFlagId">The flag Id to look up for the player</param>
        /// <returns>True if the player has a certain faction</returns>
        public bool HasFaction(FactionFlag factionFlagId) => GetFlag((int) factionFlagId);

        /// <summary>
        /// Whether this character belongs to the sentinel faction
        /// </summary>
        public bool IsSentinel => HasFaction(FactionFlag.Sentinel);
        
        /// <summary>
        /// Whether this character belongs to the assembly faction
        /// </summary>
        public bool IsAssembly => HasFaction(FactionFlag.Assembly);
        
        /// <summary>
        /// Whether this player belongs to the paradox faction
        /// </summary>
        public bool IsParadox => HasFaction(FactionFlag.Paradox);
        
        /// <summary>
        /// Whether this player belongs to the paradox faction
        /// </summary>
        public bool IsVentureLeague => HasFaction(FactionFlag.Venture);

        /// <summary>
        /// Returns the lot of a valid faction token for this character, if this character has multiple factions it will
        /// pick a random one between them.
        /// </summary>
        /// <remarks>
        /// If the character is in no faction this will return <c>Lot.FactionTokenProxy</c>
        /// </remarks>
        public Lot FactionToken
        {
            get
            {
                var possibleLots = new List<Lot>();
            
                if (IsAssembly) possibleLots.Add(Lot.AssemblyFactionToken);
                if (IsParadox) possibleLots.Add(Lot.ParadoxFactionToken);
                if (IsSentinel) possibleLots.Add(Lot.SentinelFactionToken);
                if (IsVentureLeague) possibleLots.Add(Lot.VentureFactionToken);

                // If this is a character with no valid factions, don't drop anything
                if (possibleLots.Count == 0)
                    return Lot.FactionTokenProxy;
            
                // Generally this will return the same faction token
                // but for characters with multiple factions this equally distributes the drops
               return possibleLots.Count == 1
                    ? possibleLots[0] 
                    : possibleLots[new Random().Next(0, possibleLots.Count)];
            }
        }

        #endregion flags
        
        #region currency

        /// <summary>
        /// Negative offset for the SetCurrency message.
        /// </summary>
        /// <remarks>
        /// Used when the client adds currency by itself. E.g, achievements.
        /// </remarks>
        public long HiddenCurrency { get; set; }

        /// <summary>
        /// Internal representation of the currency a player has
        /// </summary>
        private long _currency;
        
        /// <summary>
        /// The currency a player has
        /// </summary>
        public long Currency
        {
            get => _currency;
            set
            {
                _currency = value;

                if (GameObject is Player player)
                {
                    player.Message(new SetCurrencyMessage
                    {
                        Associate = player,
                        Currency = Currency - HiddenCurrency
                    });
                }
            }
        }

        public void AddCurrencySilently(long delta)
        {
            _currency += delta;
        }

        /// <summary>
        /// The amount of currency a player can pickup
        /// </summary>
        public long EntitledCurrency { get; set; }
        
        #endregion currency
        
        #region score

        /// <summary>
        /// A player's LU score
        /// </summary>
        private long _universeScore;

        /// <summary>
        /// A player's LU score
        /// </summary>
        public long UniverseScore
        {
            get => _universeScore;
            set
            {
                var oldScore = UniverseScore;
                _universeScore = value;
            
                if (GameObject is Player player)
                {
                    player.Message(new ModifyLegoScoreMessage
                    {
                        Associate = player,
                        Score = UniverseScore - oldScore
                    });
                }
            }
        }

        public void AddUniverseScoreSilently(long delta)
        {
            _universeScore += delta;
        }

        /// <summary>
        /// The universe score required to reach the next level
        /// </summary>
        public long RequiredUniverseScore { get; private set; }

        /// <summary>
        /// The level a player is currently at
        /// </summary>
        public long Level { get; private set; }

        /// <summary>
        /// Levels the player up
        /// </summary>
        /// <returns></returns>
        public async Task LevelUpAsync()
        {
            await SetLevelAsync(Level + 1);
        }

        /// <summary>
        /// Sets the level to the one provided, also checks if it's a valid level in the cd client
        /// </summary>
        /// <param name="level">The level to set</param>
        /// <param name="notifyClient">Whether to send the updated level to the client</param>
        public async Task SetLevelAsync(long level, bool notifyClient = true)
        {
            // Level 0 does not exist, set to 1 by default
            level = level == 0 ? 1 : level;

            // Make sure the level exists
            var levelLookup = await ClientCache.FindAsync<LevelProgressionLookup>(level);
            if (levelLookup == default)
                return;

            Level = level;
            RequiredUniverseScore = levelLookup.RequiredUScore ?? long.MaxValue;

            if (notifyClient && GameObject is Player player)
            {
                player.Message(new ModifyLegoScoreMessage
                {
                    Associate = player,
                    Score = RequiredUniverseScore == long.MaxValue 
                        ? UniverseScore 
                        : RequiredUniverseScore - UniverseScore
                });
            }
        }
        
        #endregion score
        
        #region serialization

        public override void Construct(BitWriter writer)
        {
            WritePart1(writer);
            WritePart2(writer);
            WritePart3(writer);

            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.Write((uint) HairColor);
            writer.Write((uint) HairStyle);
            writer.Write<uint>(0);
            writer.Write((uint) ShirtColor);
            writer.Write((uint) PantsColor);
            writer.Write<uint>(0);
            writer.Write<uint>(0);
            writer.Write((uint) EyebrowStyle);
            writer.Write((uint) EyeStyle);
            writer.Write((uint) MouthStyle);
            writer.Write((ulong) Id);
            writer.Write((ulong) LastActivity);
            writer.Write<ulong>(0);
            writer.Write((ulong) UniverseScore);
            writer.WriteBit(FreeToPlay);

            writer.Write((ulong) TotalCurrencyCollected);
            writer.Write((ulong) TotalBricksCollected);
            writer.Write((ulong) TotalSmashablesSmashed);
            writer.Write((ulong) TotalQuickBuildsCompleted);
            writer.Write((ulong) TotalEnemiesSmashed);
            writer.Write((ulong) TotalRocketsUsed);
            writer.Write((ulong) TotalMissionsCompleted);
            writer.Write((ulong) TotalPetsTamed);
            writer.Write((ulong) TotalImaginationPowerUpsCollected);
            writer.Write((ulong) TotalLifePowerUpsCollected);
            writer.Write((ulong) TotalArmorPowerUpsCollected);
            writer.Write((ulong) TotalDistanceTraveled);
            writer.Write((ulong) TotalSuicides);
            writer.Write((ulong) TotalDamageTaken);
            writer.Write((ulong) TotalDamageHealed);
            writer.Write((ulong) TotalArmorRepaired);
            writer.Write((ulong) TotalImaginationRestored);
            writer.Write((ulong) TotalImaginationUsed);
            writer.Write((ulong) TotalDistanceDriven);
            writer.Write((ulong) TotalTimeAirborne);
            writer.Write((ulong) TotalRacingImaginationPowerUpsCollected);
            writer.Write((ulong) TotalRacingImaginationCratesSmashed);
            writer.Write((ulong) TotalRacecarBoostsActivated);
            writer.Write((ulong) TotalRacecarWrecks);
            writer.Write((ulong) TotalRacingSmashablesSmashed);
            writer.Write((ulong) TotalRacesFinished);
            writer.Write((ulong) TotalFirstPlaceFinishes);
            writer.WriteBit(false);

            if (Rocket != default)
            {
                // Send the rocket if it is set to land by rocket and rocket is defined.
                writer.WriteBit(LandingByRocket);

                if (LandingByRocket)
                {
                    var rocketString = Rocket;
                    writer.Write((ushort) rocketString.Length);
                    writer.WriteString(rocketString, rocketString.Length, true);
                }
            }
            else
            {
                // Set the landing as not by rocket. Even if it is, this prevents a null reference exception.
                writer.WriteBit(false);
            }

            WritePart4(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            WritePart1(writer);
            WritePart2(writer);
            WritePart3(writer);
            WritePart4(writer);
        }

        private void WritePart1(BitWriter writer)
        {
            writer.WriteBit(true);

            var inVehicle = VehicleObject != null;

            writer.WriteBit(inVehicle);

            if (inVehicle) writer.Write(VehicleObject);

            writer.Write<byte>(0);
        }

        private void WritePart2(BitWriter writer)
        {
            var hasLevel = Level != 0;
            writer.WriteBit(hasLevel);
            if (hasLevel) writer.Write((uint) Level);
        }

        private static void WritePart3(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.WriteBit(false);
            writer.WriteBit(true);
        }

        private void WritePart4(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.WriteBit(IsPvP);
            writer.WriteBit(IsGameMaster);

            //writer.Write(GameMasterLevel); // Original code
            //writer.Write((GameMasterLevel != 1 ? GameMasterLevel : 0)); // This broke the component
            writer.Write(GameMasterLevel != 1 ? GameMasterLevel : (byte)0); // This casts to the correct type (u8)

            writer.WriteBit(false); // ???
            writer.Write<byte>(0); // ???

            writer.WriteBit(true); // Active Activity?
            writer.Write((uint) Activity);

            var hasGuild = GuildId != -1;

            writer.WriteBit(hasGuild);

            if (!hasGuild) return;

            writer.Write(GuildId);
            writer.Write((byte) GuildName.Length);
            writer.WriteString(GuildName, GuildName.Length, true);
            writer.WriteBit(true); // Guild Owner?
            writer.Write(-1); // Guild Creation date
        }
        
        #endregion serialization
    }
}
