using System;
using System.Linq;
using System.Threading.Tasks;
using IronPython.Modules;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;

namespace Uchu.World
{
    [RequireComponent(typeof(DestroyableComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        private DestroyableComponent Stats { get; set; }

        public override ComponentId Id => ComponentId.DestructibleComponent;

        /// <summary>
        ///     Warning: Should not be used as a definitive client state. Could be unreliable.
        /// </summary>
        public bool Alive { get; set; } = true;

        public float ResurrectTime { get; set; }

        /// <summary>
        ///     Killer, Loot Owner
        /// </summary>
        public Event<GameObject, Player> OnSmashed { get; }

        public Event OnResurrect { get; }

        protected DestructibleComponent()
        {
            OnSmashed = new Event<GameObject, Player>();
            
            OnResurrect = new Event();
            
            Listen(OnStart, async () =>
            {
                if (GameObject.Settings.TryGetValue("respawn", out var resTimer))
                {
                    ResurrectTime = resTimer switch
                    {
                        uint timer => timer,
                        float timer => timer,
                        int timer => timer,
                        _ => ResurrectTime
                    };
                }

                var container = GameObject.AddComponent<LootContainerComponent>();

                await container.CollectDetailsAsync();
                
                GameObject.Layer = StandardLayer.Smashable;

                var entry = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);
                var cdClientComponent = (await ClientCache.FindAsync<Core.Client.DestructibleComponent>(entry));

                if (cdClientComponent == default)
                    Logger.Error($"{GameObject} has a corrupt Destructible Component of id: {entry}");
                

                if (GameObject.TryGetComponent(out DestroyableComponent stats))
                {
                    Stats = stats;
                    
                    Listen(Stats.OnDeath, async () =>
                    {
                        await SmashAsync(
                            Stats.LatestDamageSource,
                            Stats.LatestDamageSource is Player player ? player : default,
                            Stats.LatestEffect
                        );
                    });
                    
                    return;
                }
                
                Logger.Error($"{GameObject} has a {nameof(DestructibleComponent)} without a {nameof(World.DestroyableComponent)} component.");
            });
            
            Listen(OnDestroyed, () =>
            {
                OnResurrect.Clear();
                
                OnSmashed.Clear();
            });
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);
            
            GameObject.GetComponent<DestroyableComponent>().Construct(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            GameObject.GetComponent<DestroyableComponent>().Serialize(writer);
        }

        public async Task SmashAsync(GameObject smasher, Player owner = default, string animation = "")
        {
            if (!Alive)
                return;

            // Determine if this was smashed by a player or a player spawned entity
            if (smasher is AuthoredGameObject authored)
            {
                owner = authored.Author as Player;
            }
            owner ??= smasher as Player;
            
            Alive = false;

            // Check achievements and missions
            if (owner != null && owner.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.SmashAsync(GameObject.Lot);
            }

            Zone.BroadcastMessage(new DieMessage
            {
                Associate = GameObject,
                DeathType = animation,
                Killer = smasher,
                SpawnLoot = false,
                LootOwner = owner ?? GameObject
            });

            // Determine whether this was a player or a regular game object
            if (GameObject is Player player)
            {
                GeneratePlayerYieldsAsync(player);
            }
            else
            {
                GameObject.Layer -= StandardLayer.Smashable;
                GameObject.Layer += StandardLayer.Hidden;

                if (GameObject.Spawner == default)
                {
                    InitializeRespawn();
                }
                
                if (owner != null)
                {
                    await GenerateYieldsAsync(owner);
                }

                await OnSmashed.InvokeAsync(smasher, owner);
            }
        }
        
        private async Task GenerateYieldsAsync(Player owner)
        {
            var container = GameObject.GetComponent<LootContainerComponent>();
            
            foreach (var ItemPair in await container.GenerateLootYieldsAsync(owner))
            {
                var lot = ItemPair.Key;
                Lot item = lot;
                
                if (lot == Lot.FactionTokenProxy && owner.TryGetComponent<CharacterComponent>(out var character))
                {
                    item = character.FactionToken;
                    if (item == lot)
                        continue;
                }

                for (int i = 0; i < ItemPair.Value; ++i)
                {
                    var drop = InstancingUtilities.InstantiateLoot(item, owner, GameObject, Transform.Position);
                    Start(drop);
                }
            }

            var currency = container.GenerateCurrencyYields();

            if (currency > 0)
            {
                InstancingUtilities.InstantiateCurrency(currency, owner, GameObject, Transform.Position);
            }
        }

        private void GeneratePlayerYieldsAsync(Player owner)
        {
            if (GameObject.TryGetComponent<CharacterComponent>(out var character))
            {
                var coinToDrop = Math.Min((long) Math.Round(character.Currency * 0.1), 10000);
                character.Currency -= coinToDrop;

                InstancingUtilities.InstantiateCurrency((int) coinToDrop, owner, owner, Transform.Position);
            }
        }

        private void InitializeRespawn()
        {
            Task.Run(async () =>
            {
                await Task.Delay((int) (ResurrectTime * 1000));

                await ResurrectAsync();
            });
        }

        public async Task ResurrectAsync()
        {
            Alive = true;

            if (GameObject is Player player)
            {
                Zone.BroadcastMessage(new ResurrectMessage
                {
                    Associate = player
                });

                Stats.Health = Math.Min(Stats.MaxHealth, 4);
            }
            else
            {
                Stats.Health = Stats.MaxHealth;

                GameObject.Layer += StandardLayer.Smashable;
                GameObject.Layer -= StandardLayer.Hidden;
            }

            await OnResurrect.InvokeAsync();
        }
    }
}