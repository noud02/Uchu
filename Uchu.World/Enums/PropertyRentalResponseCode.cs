namespace Uchu.World
{
    public enum PropertyRentalResponseCode
    {
        Ok = 0,
        AlreadySold = 4,
        NoneLeft = 5,
        YouCantAffordIt,
        DbFailed,
        PropertyNotReady,
        DontHaveAchievement,
    }
}