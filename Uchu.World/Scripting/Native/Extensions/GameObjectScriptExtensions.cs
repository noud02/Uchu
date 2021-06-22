namespace Uchu.World.Scripting.Native
{
    public static class GameObjectScriptExtensions
    {
        public static void Animate(this GameObject @this, string animation, bool playImmediate = false)
        {
            @this.Zone.BroadcastMessage(new PlayAnimationMessage
            {
                Associate = @this,
                AnimationId = animation,
                PlayImmediate = playImmediate,
                Priority = 0.4f,
                Scale = 1,
            });
        }

        public static void PlayFX(this GameObject @this, string name, string type, int id = -1)
        {
            @this.Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Associate = @this,
                EffectId = id,
                EffectType = type,
                Name = name,
                Priority = 1,
                Scale = 1,
                Serialize = true,
            });
        }

        public static void StopFX(this GameObject @this, string name, bool killImmediate = false)
        {
            @this.Zone.BroadcastMessage(new StopFXEffectMessage
            {
                Associate = @this,
                Name = name,
                KillImmediate = killImmediate
            });
        }

        public static string[] GetGroups(this GameObject @this)
        {
            if (!@this.Settings.TryGetValue("groupID", out var groupId)) return new string[0];

            if (!(groupId is string groupIdString)) return new string[0];
                
            var groups = groupIdString.Split(';');

            return groups;
        }
    }
}