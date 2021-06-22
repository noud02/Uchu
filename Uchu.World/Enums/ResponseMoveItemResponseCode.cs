namespace Uchu.World
{
    public enum ResponseMoveItemResponseCode
    {
        Success,
        FailGeneric,
        FailInvFull,
        FailItemNotFound,
        FailCantMoveToThatInvType,
        FailNotNearBank,
        FailCantSwapItems,
        FailSourceType,
        FailWrongDestType,
        FailSwapDestType,
        FailCantMoveThinkingHat,
        FailDismountBeforeMoving,
    }
}