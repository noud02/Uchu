using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World
{
    public class PetComponent : ReplicaComponent
    {
        private Player CurrentBuilder = default;

        private List<Brick> Bricks;
        private bool PetWild  => Owner == null;
        public GameObject Owner { get; set; }

        public uint ModerationStatus { get; set; }

        public GameObject PetInteractionObject { get; set; }

        public override ComponentId Id => ComponentId.PetComponent;

        protected PetComponent()
        {
            Bricks = new List<Brick>();

            Listen(OnStart, () =>
            {
                Listen(GameObject.OnInteract, OnInteract);
            });
        }
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write<uint>(0x4000002);
            writer.Write<uint>(0x00);

            var hasPetInteraction = PetInteractionObject != null;
            writer.WriteBit(hasPetInteraction);
            if (hasPetInteraction) writer.Write(PetInteractionObject);

            var hasOwner = Owner != null;
            writer.WriteBit(hasOwner);
            if (hasOwner) writer.Write(Owner);

            writer.WriteBit(true);
            writer.Write(ModerationStatus);

            writer.Write((byte) GameObject.Name.Length);
            writer.WriteString(GameObject.Name, GameObject.Name.Length, true);

            if (hasOwner)
            {
                writer.Write((byte) Owner.Name.Length);
                writer.WriteString(Owner.Name, Owner.Name.Length, true);
            }
            else
            {
                writer.Write<byte>(0);
            }
        }

        public async Task OnInteract(Player player)
        {
            if (PetWild)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(UchuServer.Config.ResourcesConfiguration.GameResourceFolder,
                    ClientCache.GetTable<TamingBuildPuzzles>().FirstOrDefault(i => i.NPCLot == GameObject.Lot)
                    .ValidPiecesLXF));

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Name == "Bricks")
                    {
                        foreach (XmlNode BrickNode in node.ChildNodes)
                        {
                            if (BrickNode.Name == "Brick")
                            {
                                Brick current = new Brick();
                                current.DesignID = UInt32.Parse(BrickNode.Attributes["designID"].Value);
                                foreach (XmlNode PartNode in BrickNode.ChildNodes)
                                {
                                    if (PartNode.Name == "Part")
                                    {
                                        Part CurrentPart = new Part();
                                        CurrentPart.Material = Int32.Parse(PartNode.Attributes["materials"].Value.Split(',')
                                            .ElementAt(0));
                                        current.DesignPart = CurrentPart;
                                    }
                                }
                                Bricks.Add(current);
                            }
                        }

                        break;
                    }
                }
                
                NotifyPetTamingMinigameMessage msg = new NotifyPetTamingMinigameMessage();

                msg.Associate = player;
                
                msg.ForceTeleport = true;
                msg.PlayerTaming = player;
                msg.PetId = GameObject.Id;
                msg.NotifyType = PetTamingNotifyType.Begin;
                
                Vector3 petPos = GameObject.Transform.Position;
                msg.PetDestinationPosition = petPos;
                Vector3 pos = player.Transform.Position;
                double deg = Math.Atan2(petPos.Z - pos.Z, petPos.X - pos.X) * 180 / Math.PI;
                var interaction_distance = GameObject.Settings.ContainsKey("interaction_distance") ? GameObject.Settings["interaction_distance"] : 0.0f;
                pos = new Vector3(
                    petPos.X + (float) interaction_distance * (float)Math.Cos(-deg),
                    petPos.Y,
                    petPos.Z + (float) interaction_distance * (float)Math.Sin(-deg)
                );
                msg.TeleportPosition = pos;

                msg.TeleportRotation = pos.QuaternionLookRotation(petPos);
                
                Zone.BroadcastMessage(msg);

                var nmsg = new NotifyPetTamingPuzzleSelectedMessage();
                nmsg.Associate = player;
                nmsg.Bricks = Bricks.ToArray();
                player.Message(nmsg);
                player.Message(new NotifyPetTamingMinigameMessage
                {
                    Associate = GameObject,
                    ForceTeleport = false,
                    NotifyType = PetTamingNotifyType.Begin,
                    PetId = (ObjectId)(ulong)0,
                    PetDestinationPosition = Vector3.Zero,
                    PlayerTaming = player,
                    TeleportPosition = Vector3.Zero,
                    TeleportRotation = Quaternion.Identity
                });

                // Create all the pet listeners for other events
                
                Listen(player.OnPetTamingTryBuild, OnPetTamingTryBuild);
                Listen(player.OnNotifyTamingBuildSuccessMessage, OnNotifyTamingBuildSuccessMessage);
            }
        }

        public async Task OnPetTamingTryBuild(PetTamingTryBuildMessage msg)
        {
            int CorrectCount = 0;

            foreach (var item in Bricks)
            {
                foreach (Brick item2 in msg.Bricks)
                {
                    if (item.DesignID == item2.DesignID)
                        CorrectCount += 1;
                }
            }
                
            PetTamingTryBuildResultMessage nmsg = new PetTamingTryBuildResultMessage();
            nmsg.Associate = msg.Associate;
            nmsg.Success = !(CorrectCount == Bricks.Count);
            nmsg.NumberCorrect = CorrectCount;
            (msg.Associate as Player).Message(nmsg);
        }
        
        public async Task OnNotifyTamingBuildSuccessMessage(NotifyTamingBuildSuccessMessage msg)
        {
            var player = (msg.Associate as Player);
            await player.GetComponent<MissionInventoryComponent>().TamePetAsync(GameObject.Lot);
            player.PlayFX("", "petceleb");
            
            // We need to create the build object
            /*CdClientContext context = new CdClientContext(); TODO: fix
            var model = GameObject.Instantiate(default,
                new Lot(context.TamingBuildPuzzlesTable.Where(i => i.NPCLot.Value == GameObject.Lot).First()
                    .PuzzleModelLot.Value), msg.BuildPosition);

            Start(model);*/
            
            player.Message(new NotifyTamingModelLoadedOnServerMessage
            {
                Associate = player
            });
            
            player.Message(new PetResponseMessage
            {
                Associate = player,
                Pet = GameObject,
                PetCommandType = 0,
                Response = 10, // Not entirely sure what this response ID is 
                TypeId = 0
            });

            var inventoryComponent = player.GetComponent<InventoryManagerComponent>();
            await inventoryComponent.AddLotAsync(GameObject.Lot, 1);
            
            var pet = inventoryComponent.FindItem(GameObject.Lot);

            player.Message(new AddPetToPlayerMessage
            {
                Associate = player,
                ElementalType = 0, // This appears to be just unused, they are in the DB but they weren't sent correctly in the packet captures
                Name = GameObject.Name,
                PetDBId = pet,
                PetLot = GameObject.Lot
            });
            
            player.Message(new RegisterPetIdMessage
            {
                Associate = player,
                Pet = GameObject,
            });
            
            player.Message(new RegisterPetDBIdMessage
            {
                Associate = player,
                Pet = pet,
            });
            
            player.Message( new MarkInventoryItemAsActiveMessage
            {
                Associate = player,
                Active = true,
                ItemId = pet,
            });
            
            // TODO: Add listener for name select
        }
    }
}