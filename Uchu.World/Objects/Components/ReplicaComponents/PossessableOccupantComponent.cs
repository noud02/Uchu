namespace Uchu.World
{
    public class PossessableOccupantComponent : StructReplicaComponent<PossessableOccupantSerialization,PossessableOccupantSerialization>
    {
        public override ComponentId Id => ComponentId.Component107;

        public GameObject Possessed { get; set; }
    }
}