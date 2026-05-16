using System;

namespace _.Scripts.Gameplay.World_Modules
{
    public static class EntityIdentityEnumFactory
    {
        public enum EntityIdentityEnum
        {
            ENT_PLAYER
        }

        public static string BuildIdentity(EntityIdentityEnum enumIdentity)
        {
            return enumIdentity switch
            {
                EntityIdentityEnum.ENT_PLAYER => "Player",
                _ => throw new ArgumentOutOfRangeException(nameof(enumIdentity), enumIdentity, null)
            };
        }
    }
}