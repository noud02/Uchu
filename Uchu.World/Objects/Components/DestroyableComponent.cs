using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Objects.Components;

namespace Uchu.World
{
    public class DestroyableComponent : Component, ISavableComponent
    {
        private uint _health;

        private uint _maxHealth;

        private uint _armor;

        private uint _maxArmor;

        private uint _imagination;

        private uint _maxImagination;
        
        public int[] Factions { get; set; } = new int[0];
        public int[] Friends { get; set; } = new int[0];
        
        public int[] Enemies { get; set; } = new int[0];
        
        public uint DamageAbsorptionPoints { get; set; }
        
        public bool Immune { get; set; }
        
        public bool GameMasterImmune { get; set; }
        
        public bool Shielded { get; set; }

        public GameObject LatestDamageSource { get; private set; }
        
        public string LatestEffect { get; private set; }

        public uint Health
        {
            get => _health;
            set
            {
                value = Math.Min(value, MaxHealth);

                if (value == _health) return;

                OnHealthChanged.Invoke(value, (int) ((int) value - _health));

                _health = value;

                GameObject.Serialize(GameObject);

                if (_health == 0) OnDeath.Invoke();
            }
        }

        public uint MaxHealth
        {
            get => _maxHealth;
            set
            {
                var delta = (int) ((int) value - _maxHealth);

                if (delta < 0 && _health > value)
                {
                    OnHealthChanged.Invoke(value, (int) ((int) value - _health));

                    _health = value;
                }
                
                OnMaxHealthChanged.Invoke(value, delta);

                _maxHealth = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint Armor
        {
            get => _armor;
            set
            {
                value = Math.Min(value, MaxArmor);

                if (value == _armor) return;

                OnArmorChanged.Invoke(value, (int) ((int) value - _armor));

                _armor = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint MaxArmor
        {
            get => _maxArmor;
            set
            {
                var delta = (int) ((int) value - _maxArmor);

                if (delta < 0 && _armor > value)
                {
                    OnArmorChanged.Invoke(value, (int) ((int) value - _armor));

                    _armor = value;
                }
                
                OnMaxArmorChanged.Invoke(value, delta);

                _maxArmor = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint Imagination
        {
            get => _imagination;
            set
            {
                value = Math.Min(value, MaxImagination);

                if (value == _imagination) return;

                OnImaginationChanged.Invoke(value, (int) ((int) value - _imagination));

                _imagination = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint MaxImagination
        {
            get => _maxImagination;
            set
            {
                var delta = (int) ((int) value - _maxImagination);

                if (delta < 0 && _imagination > value)
                {
                    OnImaginationChanged.Invoke(value, (int) ((int) value - _imagination));

                    _imagination = value;
                }
                
                OnMaxImaginationChanged.Invoke(value, delta);

                _maxImagination = value;

                GameObject.Serialize(GameObject);
            }
        }

        public bool Smashable { get; set; }
        
        public bool HasStats { get; set; }

        /// <summary>
        /// New Health, Delta
        /// </summary>
        public Event<uint, int> OnHealthChanged { get; }

        /// <summary>
        /// New Armor, Delta
        /// </summary>
        public Event<uint, int> OnArmorChanged { get; }

        /// <summary>
        /// New Imagination, Delta
        /// </summary>
        public Event<uint, int> OnImaginationChanged { get; }

        /// <summary>
        /// New MaxHealth, Delta
        /// </summary>
        public Event<uint, int> OnMaxHealthChanged { get; }

        /// <summary>
        /// New MaxArmor, Delta
        /// </summary>
        public Event<uint, int> OnMaxArmorChanged { get; }

        /// <summary>
        /// New MaxImagination, Delta
        /// </summary>
        public Event<uint, int> OnMaxImaginationChanged { get; }

        public Event OnDeath { get; }

        protected DestroyableComponent()
        {
            OnHealthChanged = new Event<uint, int>();
            
            OnArmorChanged = new Event<uint, int>();
            
            OnImaginationChanged = new Event<uint, int>();
            
            OnMaxHealthChanged = new Event<uint, int>();
            
            OnMaxArmorChanged = new Event<uint, int>();
            
            OnMaxImaginationChanged = new Event<uint, int>();
            
            OnDeath = new Event();
            
            Listen(OnStart, async () =>
            {
                if (GameObject is Player) CollectPlayerStats();
                else CollectObjectStats();

                var componentId = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);
                var destroyable = (await ClientCache.GetTableAsync<Core.Client.DestructibleComponent>()).FirstOrDefault(
                    c => c.Id == componentId
                );
                
                if (destroyable == default) return;

                Factions = new[] {destroyable.Faction ?? 1};

                Smashable = destroyable.IsSmashable ?? false;

                var faction = (await ClientCache.GetTableAsync<Factions>()).FirstOrDefault(
                    f => f.Faction == Factions[0]
                );
                
                if (faction?.EnemyList == default) return;
                
                if (string.IsNullOrWhiteSpace(faction.EnemyList)) return;

                Enemies = faction.EnemyList
                    .Replace(" ", "")
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                if (faction?.FriendList == default) return;
                
                if (string.IsNullOrWhiteSpace(faction.FriendList)) return;
                
                Friends = faction.FriendList
                    .Replace(" ", "")
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();
            });

            Listen(OnDestroyed, () =>
            {
                OnHealthChanged.Clear();
                OnArmorChanged.Clear();
                OnImaginationChanged.Clear();
                OnHealthChanged.Clear();
                OnMaxArmorChanged.Clear();
                OnMaxImaginationChanged.Clear();

                OnDeath.Clear();
            });
        }

        /// <summary>
        /// Damages a game object
        /// </summary>
        /// <remarks>
        /// Be careful when calling this method as it can trigger DieMessages and DropClientLootMessages causing a long
        /// runtime.
        /// </remarks>
        /// <param name="value">The amount of damage</param>
        /// <param name="source">The game object that caused the damage</param>
        /// <param name="effectHandler">Optional effect handler to display</param>
        public void Damage(uint value, GameObject source, string effectHandler = "")
        {
            LatestDamageSource = source;
            LatestEffect = effectHandler;
            
            var armorDamage = Math.Min(value, Armor);

            value -= armorDamage;
            Armor -= armorDamage;

            Health -= Math.Min(value, Health);
        }

        public void Heal(uint value)
        {
            var armorHeal = Math.Min(value, MaxArmor - Armor);

            value -= armorHeal;
            Armor += armorHeal;

            Health += Math.Min(value, MaxHealth - Health);
        }

        /// <summary>
        /// Boosts the base health of the game object, in case of a character also updates the base health there
        /// </summary>
        /// <param name="delta">The increase of the base health</param>
        public void BoostBaseHealth(uint delta)
        {
            MaxHealth += delta;
            Health += delta;
            
            if (!(GameObject is Player player && player.TryGetComponent<CharacterComponent>(out var character)))
                return;
            character.BaseHealth += (int) delta;
        }

        /// <summary>
        /// Boosts the base imagination of the game object, in case of a character also updates the base imagination there
        /// </summary>
        /// <param name="delta">The increase of the base imagination</param>
        public void BoostBaseImagination(uint delta)
        {
            MaxImagination += delta;
            Imagination += delta;
            
            if (!(GameObject is Player player && player.TryGetComponent<CharacterComponent>(out var character)))
                return;
            character.BaseImagination += (int) delta;
        }

        private void CollectObjectStats()
        {
            var componentId = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);
            var stats = ClientCache.GetTable<Core.Client.DestructibleComponent>().FirstOrDefault(
                o => o.Id == componentId
            );

            if (stats == default) return;
            HasStats = true;

            var rawHealth = stats.Life ?? 0;
            var rawArmor = (int) (stats.Armor ?? 0);
            var rawImagination = stats.Imagination ?? 0;

            _health = (uint) (rawHealth != -1 ? rawHealth : 0);
            _armor = (uint) (rawArmor != -1 ? rawArmor : 0);
            _imagination = (uint) (rawImagination != -1 ? rawImagination : 0);

            _maxHealth = Health;
            _maxArmor = Armor;
            _maxImagination = Imagination;
        }

        private void CollectPlayerStats()
        {
            if (!(GameObject is Player)) return;
            HasStats = true;
            
            using var ctx = new UchuContext();

            var character = ctx.Characters.First(c => c.Id == GameObject.Id);

            /*
             * Any additional stats gets added on by skills.
             */
            
            _health = (uint) character.CurrentHealth;
            _maxHealth = (uint) character.BaseHealth;

            _armor = (uint) character.CurrentArmor;
            _maxArmor = default;

            _imagination = (uint) character.CurrentImagination;
            _maxImagination = (uint) character.BaseImagination;
        }
        
        public void Construct(BitWriter writer)
        {
            writer.WriteBit(true);

            for (var i = 0; i < 9; i++) writer.Write<uint>(0);

            WriteStats(writer);

            if (HasStats)
            {
                writer.WriteBit(false);
                writer.WriteBit(false);

                if (Smashable)
                {
                    writer.WriteBit(false);
                    writer.WriteBit(false);
                }
            }

            writer.WriteBit(true);
            writer.WriteBit(false);
        }

        public void Serialize(BitWriter writer)
        {
            WriteStats(writer);

            writer.WriteBit(true);
            writer.WriteBit(false);
        }

        private void WriteStats(BitWriter writer)
        {
            if (!writer.Flag(HasStats)) return;
            
            writer.Write(Health);
            writer.Write<float>(MaxHealth);

            writer.Write(Armor);
            writer.Write<float>(MaxArmor);

            writer.Write(Imagination);
            writer.Write<float>(MaxImagination);

            writer.Write(DamageAbsorptionPoints);
            writer.WriteBit(Immune);
            writer.WriteBit(GameMasterImmune);
            writer.WriteBit(Shielded);

            writer.Write<float>(MaxHealth);
            writer.Write<float>(MaxArmor);
            writer.Write<float>(MaxImagination);

            writer.Write((uint) Factions.Length);

            foreach (var faction in Factions) writer.Write(faction);

            writer.WriteBit(Smashable);
        }
        
        /// <summary>
        /// Saves the character stats of the player
        /// </summary>
        /// <param name="context">The database context to save to</param>
        public async Task SaveAsync(UchuContext context)
        {
            var character = await context.Characters.Where(c => c.Id == GameObject.Id)
                .FirstAsync();

            character.CurrentHealth = (int) Health;
            character.CurrentArmor = (int) Armor;
            character.CurrentImagination = (int) Imagination;
            character.MaximumHealth = (int) MaxHealth;
            character.MaximumArmor = (int) MaxArmor;
            character.MaximumImagination = (int) MaxImagination;

            Logger.Debug($"Saved character stats for {GameObject}");
        }
    }
}