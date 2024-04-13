using System;
using BML.Scripts.Attributes;

namespace BML.Scripts {
    [Flags]
    public enum DamageType
    {
        // None = 0,
        Spray = 1 << 0,
        Vacuum = 1 << 1,
        EnemyDamage = 1 << 2,
    }
}
