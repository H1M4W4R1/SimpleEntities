using System;

namespace Systems.SimpleEntities.Data.Enums
{
    [Flags]
    public enum StatusModificationFlags
    {
        None = 0,
        IgnoreConditions = 1 << 0,
        IgnoreStackLimit = 1 << 1,
        
    }
}