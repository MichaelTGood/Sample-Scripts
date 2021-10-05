using System;

namespace Squads
{
    public enum LoadUpgradeTime
    {
        LoadDuringMatchStart,
        AddComponentToCharacter
    }

    [Flags]
    public enum UpgradeTypes
    {
        None = 0,
        Movement = 1,
        ElectricalSystems = 2,
        FirearmCombat = 4,
        MeleeCombat = 8,
        All = Movement | ElectricalSystems | FirearmCombat | MeleeCombat,
    }
}
