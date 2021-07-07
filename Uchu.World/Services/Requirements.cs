using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Services
{
    public static class Requirements
    {
        private static bool Check(bool a, bool b, Mode mode)
        {
            return mode switch
            {
                Mode.And => (a && b),
                Mode.Or => (a || b),
                Mode.None => false,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
        
        private enum Mode
        {
            None,
            And,
            Or
        }

        private static async Task<bool> CheckAsync(string cur, Player player)
        {
            if (string.IsNullOrWhiteSpace(cur)) return true;

            return await CheckPreconditionAsync(int.Parse(cur), player);
        }
        
        public static async Task<bool> CheckRequirementsAsync(string requirements, Player player)
        {
            if (string.IsNullOrWhiteSpace(requirements)) return true;

            var cur = new StringBuilder();
            var res = true;
            var mode = Mode.And;

            for (var i = 0; i < requirements.Length; i++)
            {
                var chr = requirements[i];

                switch (chr)
                {
                    case ' ':
                        break;

                    case '&':
                    case ',':
                    {
                        res = Check(res, await CheckAsync(cur.ToString(), player), mode);

                        cur.Clear();

                        if (!res)
                            return false;

                        mode = Mode.And;
                        break;
                    }

                    case '|':
                    {
                        res = Check(res, await CheckAsync(cur.ToString(), player), mode);

                        cur.Clear();

                        mode = Mode.Or;
                        break;
                    }

                    case '(':
                        res = Check(res, await CheckRequirementsAsync(requirements.Substring(i + 1), player), mode);
                        break;

                    case ')':
                        return Check(res, await CheckAsync(cur.ToString(), player), mode);

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case ':':
                        cur.Append(chr);
                        break;
                }
            }

            res = Check(res, await CheckAsync(cur.ToString(), player), mode);

            return res;
        }
        
        public static async Task<bool> CheckPreconditionAsync(int id, Player player)
        {
            var precondition = await ClientCache.FindAsync<Preconditions>(id);
            if (precondition?.Type == default)
            {
                Logger.Error($"Invalid precondition: {id}!");

                return false;
            }

            var type = (PreconditionType) precondition.Type;

            switch (type)
            {
                case PreconditionType.ItemEquipped:
                    return ItemEquipped(precondition, player);
                case PreconditionType.ItemNotEquipped:
                    return ItemNotEquipped(precondition, player);
                case PreconditionType.HasItem:
                    return HasItem(precondition, player);
                case PreconditionType.DoesNotHaveItem:
                    return DoesNotHaveItem(precondition, player);
                case PreconditionType.HasAchievement:
                    return HasAchievement(precondition, player);
                case PreconditionType.MissionAvailable:
                    return MissionAvailable(precondition, player);
                case PreconditionType.OnMission:
                    return OnMission(precondition, player);
                case PreconditionType.MissionComplete:
                    return MissionComplete(precondition, player);
                case PreconditionType.PetDeployed:
                    break;
                case PreconditionType.HasFlag:
                    break;
                case PreconditionType.WithinShape:
                    break;
                case PreconditionType.InBuild:
                    break;
                case PreconditionType.TeamCheck:
                    break;
                case PreconditionType.IsPetTaming:
                    break;
                case PreconditionType.HasFaction:
                    break;
                case PreconditionType.DoesNotHaveFaction:
                    break;
                case PreconditionType.HasRacingLicence:
                    break;
                case PreconditionType.DoesNotHaveRacingLicence:
                    break;
                case PreconditionType.LegoClubMember:
                    break;
                case PreconditionType.NoInteraction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private static bool ItemEquipped(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryComponent>();

            var lot = preconditions.TargetLOT.InterpretCollection().First();

            return inventory.HasEquipped(lot);
        }

        private static bool ItemNotEquipped(Preconditions preconditions, Player player)
        {
            return !ItemEquipped(preconditions, player);
        }

        private static bool HasItem(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();
            
            var lot = preconditions.TargetLOT.InterpretCollection().First();

            var count = preconditions.TargetCount ?? 1;

            return inventory.Items.Where(i => i.Lot == lot).Sum(i => i.Count) >= count;
        }

        private static bool DoesNotHaveItem(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();
            
            var lot = preconditions.TargetLOT.InterpretCollection().First();

            return inventory.Items.All(i => i.Lot != lot);
        }

        private static bool HasAchievement(Preconditions preconditions, Player player) 
            => MissionComplete(preconditions, player);

        private static bool MissionAvailable(Preconditions preconditions, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>();
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missions.HasAvailable(id);
        }

        private static bool OnMission(Preconditions preconditions, Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missionInventory.HasActive(id);
        }

        private static bool MissionComplete(Preconditions preconditions, Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missionInventory.HasCompleted(id);
        }
    }
}