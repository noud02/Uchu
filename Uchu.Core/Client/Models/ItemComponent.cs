using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ItemComponent")]
	[CacheMethod(CacheMethod.Burst)]
	public class ItemComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("equipLocation")]
		public string EquipLocation { get; set; }

		[Column("baseValue")]
		public int? BaseValue { get; set; }

		[Column("isKitPiece")]
		public bool? IsKitPiece { get; set; }

		[Column("rarity")]
		public int? Rarity { get; set; }

		[Column("itemType")]
		public int? ItemType { get; set; }

		[Column("itemInfo")]
		public long? ItemInfo { get; set; }

		[Column("inLootTable")]
		public bool? InLootTable { get; set; }

		[Column("inVendor")]
		public bool? InVendor { get; set; }

		[Column("isUnique")]
		public bool? IsUnique { get; set; }

		[Column("isBOP")]
		public bool? IsBOP { get; set; }

		[Column("isBOE")]
		public bool? IsBOE { get; set; }

		[Column("reqFlagID")]
		public int? ReqFlagID { get; set; }

		[Column("reqSpecialtyID")]
		public int? ReqSpecialtyID { get; set; }

		[Column("reqSpecRank")]
		public int? ReqSpecRank { get; set; }

		[Column("reqAchievementID")]
		public int? ReqAchievementID { get; set; }

		[Column("stackSize")]
		public int? StackSize { get; set; }

		[Column("color1")]
		public int? Color1 { get; set; }

		[Column("decal")]
		public int? Decal { get; set; }

		[Column("offsetGroupID")]
		public int? OffsetGroupID { get; set; }

		[Column("buildTypes")]
		public int? BuildTypes { get; set; }

		[Column("reqPrecondition")]
		public string ReqPrecondition { get; set; }

		[Column("animationFlag")]
		public int? AnimationFlag { get; set; }

		[Column("equipEffects")]
		public int? EquipEffects { get; set; }

		[Column("readyForQA")]
		public bool? ReadyForQA { get; set; }

		[Column("itemRating")]
		public int? ItemRating { get; set; }

		[Column("isTwoHanded")]
		public bool? IsTwoHanded { get; set; }

		[Column("minNumRequired")]
		public int? MinNumRequired { get; set; }

		[Column("delResIndex")]
		public int? DelResIndex { get; set; }

		[Column("currencyLOT")]
		public int? CurrencyLOT { get; set; }

		[Column("altCurrencyCost")]
		public int? AltCurrencyCost { get; set; }

		[Column("subItems")]
		public string SubItems { get; set; }

		[Column("audioEventUse")]
		public string AudioEventUse { get; set; }

		[Column("noEquipAnimation")]
		public bool? NoEquipAnimation { get; set; }

		[Column("commendationLOT")]
		public int? CommendationLOT { get; set; }

		[Column("commendationCost")]
		public int? CommendationCost { get; set; }

		[Column("audioEquipMetaEventSet")]
		public string AudioEquipMetaEventSet { get; set; }

		[Column("currencyCosts")]
		public string CurrencyCosts { get; set; }

		[Column("ingredientInfo")]
		public string IngredientInfo { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("forgeType")]
		public int? ForgeType { get; set; }

		[Column("SellMultiplier")]
		public float? SellMultiplier { get; set; }
	}
}