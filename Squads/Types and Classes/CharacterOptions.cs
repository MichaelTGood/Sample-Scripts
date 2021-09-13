using System;

namespace Squads
{

    #region ENUM OPTIONS

        public enum LocomotionTypes
        {
            LocomotionBasic,
            FlyingBasic
        }

        public enum CameraControlTypes
        {
            Basic,
            NoAim,
            FlyingDrone

        }

        public enum WeaponControlTypes
        {
            FirearmBasic,
            Akimbo,
            MeleeBasic
        }
        
        [Flags] 
        public enum WeaponTypes
        {
            None = 0,
            OneHand = 1,
            TwoHand = 2,
            MeleeShort = 4,
            MeleeLong = 8,
            Firearms = OneHand | TwoHand,
            Melee = MeleeShort | MeleeLong,
            All = 15,
        }

        [Flags]
        public enum Interact
        {
            None = 0,
            Door = 1,
            Keypad = 2,
            WallButton = 4,
            Hack = 8,
            Search = 16,
            All = Door | Keypad | Hack | Search,
        }

    #endregion
}
