using System;

namespace Systems.SimpleEntities.Data.Enums
{
    [Flags]
    public enum EntityTickFlags
    {
        None = 0,
        ForceTick = 1 << 0,
    }
}