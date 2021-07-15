using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    public class SpawnerNetwork : GameObject
    {
        private readonly Random _random;

        private List<SpawnerComponent> SpawnerNodes { get; set; }

        /// <summary>
        /// Minimum number of entities to have spawned at a time.
        /// </summary>
        public uint SpawnsToMaintain { get; set; } = 1;

        /// <summary>
        /// Time to wait before spawning a new entity when one is destroyed, in milliseconds.
        /// </summary>
        public uint RespawnTime { get; set; } = 10000;

        /// <summary>
        /// Maximum number of entities to have spawned at a time.
        /// </summary>
        public int MaxToSpawn { get; set; }

        /// <summary>
        /// Whether this spawner network should start spawning as soon as it's loaded
        /// </summary>
        public bool ActivateOnLoad { get; set; }

        /// <summary>
        /// Event that's called after a game object is smashed and the spawner is waiting to respawn the game object
        /// </summary>
        public Event<Player> OnRespawnInitiated { get; }

        /// <summary>
        /// Event that's fired after a game object is smashed and the respawn time has passed
        /// </summary>
        public Event<Player> OnRespawnTimeCompleted { get; }

        public SpawnerNetwork()
        {
            _random = new Random();
            SpawnerNodes = new List<SpawnerComponent>();
            OnRespawnInitiated = new Event<Player>();
            OnRespawnTimeCompleted = new Event<Player>();

            Listen(OnRespawnInitiated, (lootOwner) =>
            {
                Zone.Schedule(() =>
                {
                    OnRespawnTimeCompleted.Invoke(lootOwner);
                    TrySpawn();
                }, RespawnTime);
            });

            Listen(OnDestroyed, () =>
            {
                foreach (var node in SpawnerNodes.ToArray())
                {
                    Destroy(node);
                }
            });
        }

        /// <summary>
        /// Number of currently existing spawned entities across all spawner nodes.
        /// </summary>
        public int ActiveCount => SpawnerNodes.Count(node => node.HasActiveSpawn);

        /// <summary>
        /// Add a new spawner node to this network.
        /// </summary>
        /// <param name="component">Spawner component to add.</param>
        public void AddSpawnerNode(SpawnerComponent component)
        {
            SpawnerNodes.Add(component);
        }

        /// <summary>
        /// Find a random spawner node in this network which doesn't have a spawned entity alive.
        /// </summary>
        /// <returns>A SpawnerComponent without a spawn if available, otherwise null.</returns>
        public SpawnerComponent FindInactiveNode()
        {
            var availableNodes = SpawnerNodes.Where(node =>
                node.HasActiveSpawn == false).ToArray();

            // All spawner nodes are in use
            if (availableNodes.Length == 0)
                return default;

            // Return random inactive node
            return availableNodes[_random.Next(availableNodes.Length)];
        }

        /// <summary>
        /// Spawn a new entity if min/max conditions are met and an empty spawner node is available.
        /// </summary>
        public void TrySpawn()
        {
            if (MaxToSpawn != -1 && ActiveCount >= MaxToSpawn)
                return;

            var node = FindInactiveNode();
            node?.Spawn();
        }

        /// <summary>
        /// Spawn entities until minimum number of entities is reached.
        /// </summary>
        public void SpawnAll()
        {
            for (var i = 0; i < SpawnsToMaintain; i++)
            {
                TrySpawn();
            }
        }
    }
    /// <summary>
    /// Component responsible for spawning objects. Can be part of a spawner network.
    /// <seealso cref="SpawnerNetwork"/>
    /// </summary>
    [ServerComponent(Id = ComponentId.SpawnerComponent)]
    public class SpawnerComponent : Component
    {
        /// <summary>
        /// Currently alive spawned entity, if it exists.
        /// </summary>
        public GameObject ActiveSpawn { get; private set; }

        /// <summary>
        /// Whether or not this spawner currently has a spawned object that's still alive.
        /// </summary>
        public bool HasActiveSpawn => ActiveSpawn != null;

        /// <summary>
        /// Time to wait before spawning a new entity when one is destroyed, in milliseconds.
        /// </summary>
        /// <remarks>
        /// Only used for standalone spawners; spawner networks use the respawn time defined in the network.
        /// </remarks>
        public int RespawnTime { get; set; } = 10000;

        /// <summary>
        /// The LOT of the object this spawner spawns.
        /// </summary>
        public Lot SpawnTemplate { get; set; }

        /// <summary>
        /// Scale for the objects that are spawned.
        /// </summary>
        public float SpawnScale { get; set; } = 1;

        /// <summary>
        /// Spawner node ID in the network it's in (sequential integers starting from 0).
        /// </summary>
        public uint SpawnerNodeId { get; set; }

        /// <summary>
        /// Network this spawner is in, if applicable.
        /// </summary>
        public SpawnerNetwork Network { get; set; }

        /// <summary>
        /// Whether or not this spawner is part of a spawner network.
        /// </summary>
        public bool IsNetworkSpawner { get; set; }
        
        /// <summary>
        /// Event that's called after a game object is smashed and the spawner is waiting to respawn the game object
        /// </summary>
        public Event<Player> OnRespawnInitiated { get; }
        
        /// <summary>
        /// Event that's fired after a game object is smashed and the respawn time has passed
        /// </summary>
        public Event<Player> OnRespawnTimeCompleted { get; }

        protected SpawnerComponent()
        {
            OnRespawnInitiated = new Event<Player>();
            OnRespawnTimeCompleted = new Event<Player>();

            Listen(OnStart, () =>
            {
                if (GameObject.Settings != default)
                {
                    if (GameObject.Settings.TryGetValue("is_network_spawner", out var isNetworkSpawner))
                    {
                        IsNetworkSpawner = (bool) isNetworkSpawner;
                    }
                }

                // Standalone spawner. Manages its own respawning.
                if (!IsNetworkSpawner)
                    Listen(OnRespawnTimeCompleted, player =>
                    {
                        Spawn();
                    });

                GameObject.Layer = StandardLayer.Spawner;
            });

            Listen(OnDestroyed, () =>
            {
                if (HasActiveSpawn)
                {
                    Destroy(ActiveSpawn);
                    ActiveSpawn = null;
                }

                OnRespawnInitiated.Clear();
                OnRespawnTimeCompleted.Clear();
            });
        }

        private GameObject GenerateSpawnObject()
        {
            var o = new LevelObjectTemplate
            {
                Lot = SpawnTemplate,
                Position = Transform.Position,
                Rotation = Transform.Rotation,
                Scale = SpawnScale,
                LegoInfo = GameObject.Settings,
                ObjectId = ObjectId.FromFlags(ObjectIdFlags.Spawned | ObjectIdFlags.Client)
            };

            var obj = GameObject.Instantiate(o, Zone, this);

            return obj;
        }

        public GameObject Spawn()
        {
            var obj = GenerateSpawnObject();
            Start(obj);
            GameObject.Construct(obj);
            ActiveSpawn = obj;

            Listen(obj.OnDestroyed, () =>
            {
                ActiveSpawn = null;
            });

            if (obj.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
            {
                Listen(destructibleComponent.OnSmashed, async (smasher, lootOwner) =>
                {
                    Destroy(obj);

                    if (IsNetworkSpawner)
                    {
                        // Part of a network which handles respawning
                        await Network.OnRespawnInitiated.InvokeAsync(lootOwner);
                    }
                    else
                    {
                        // Standalone spawner
                        await OnRespawnInitiated.InvokeAsync(lootOwner);
                        await Task.Delay(RespawnTime);
                        await OnRespawnTimeCompleted.InvokeAsync(lootOwner);
                    }
                });
            }

            return obj;
        }
    }
}
