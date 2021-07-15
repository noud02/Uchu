using System.Linq;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public struct Lot
    {
        public readonly int Id;

        public Lot(int id)
        {
            Id = id;
        }

        public static implicit operator int(Lot lot)
        {
            return lot.Id;
        }

        public static implicit operator Lot(int id)
        {
            return new Lot(id);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                int i => (i == Id),
                Lot l => (l.Id == Id),
                _ => false
            };
        }

        public bool Equals(Lot other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override int GetHashCode()
        {
            return Id;
        }
        
        public Core.Client.Objects Object
        {
            get
            {
                var id = Id;
                return ClientCache.GetTable<Core.Client.Objects>().FirstOrDefault(o => o.Id == id);
            }
        }

        public int GetComponentId(ComponentId componentType)
        {
            return GetComponentId((int) componentType);
        }

        public int GetComponentId(int componentType)
        {
            var id = Id;
            var itemRegistryEntry = ClientCache.GetTable<ComponentsRegistry>().FirstOrDefault(
                r => r.Id == id && r.Componenttype == componentType
            );

            return itemRegistryEntry?.Componentid ?? 0;
        }

        public int[] GetComponentIds(ComponentId componentType)
        {
            return GetComponentIds((int) componentType);
        }
        
        public int[] GetComponentIds(int componentType)
        {
            var id = Id;
            var itemRegistryEntry = ClientCache.GetTable<ComponentsRegistry>().Where(
                r => r.Id == id && r.Componenttype == componentType
            );

            return itemRegistryEntry.Select(r => r.Componentid.Value).ToArray();
        }

        #region Consts

        public static readonly Lot ModularRocket = 6416;

        public static readonly Lot Spawner = 176;

        public const int Imagination = 935;
        public const int TwoImagination = 4035;
        public const int ThreeImagination = 11910;
        public const int FiveImagination = 11911;
        public const int TenImagination = 11918;
        public const int Health = 177;
        public const int TwoHealth = 11915;
        public const int ThreeHealth = 11916;
        public const int FiveHealth = 11917;
        public const int TenHealth = 11920;
        public const int Armor = 6431;
        public const int TwoArmor = 11912;
        public const int ThreeArmor = 11913;
        public const int FiveArmor = 11914;
        public const int TenArmor = 11919;

        public static readonly Lot RocketBuildNosecone = 6904;
        public static readonly Lot RocketBuildCockpit = 6910;
        public static readonly Lot RocketBuildTail = 6915;

        public const int FactionTokenProxy = 13763;
        public const int AssemblyFactionToken = 8318;
        public const int SentinelFactionToken = 8319;
        public const int ParadoxFactionToken = 8320;
        public const int VentureFactionToken = 8321;

        public const int SentinelFactionVendor = 6631;
        public const int ParadoxFactionVendor = 6632;
        public const int AssemblyFactionVendor = 6633;
        public const int VentureFactionVendor = 6634;
        public const int SentinelFactionVendorProxy = 7094;
        public const int ParadoxFactionVendorProxy = 7097;
        public const int AssemblyFactionVendorProxy = 7096;
        public const int VentureFactionVendorProxy = 7095;

        public const int SenseiWuNpc = 13789;
        public const int ColeNpc = 13790;
        public const int JayNpc = 13792;
        public const int AssemblyPetConsole = 13879;
        public const int HariHowzenNpc = 13798;

        public const int Chopov = 16047;
        public const int Frakjaw = 16049;
        public const int Krazi = 16049;
        public const int Bonezai = 16050;


        #endregion
    }
}
