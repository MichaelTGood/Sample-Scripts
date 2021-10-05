using System;

namespace Squads
{

    #region ENUM OPTIONS

        public enum LocomotionTypes
        {
            None,
            LocomotionBasic,
            FlyingBasic
        }

        public enum CameraControlTypes
        {
            None,
            CameraControlBasic,
            NoAim,
            FlyingDrone
        }

        public enum HandControlTypes
        {
            None,
            TwoHandBasic,
            DroneClaws
        }

        public enum WeaponControlTypes
        {
            None,
            FirearmControlBasic,
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
            All = OneHand | TwoHand | MeleeShort | MeleeLong,
        }

        public enum InteractorTypes
        {
            None,
            InteractBasic
        }

        [Flags]
        public enum InteractAbilities
        {
            None = 0,
            Door = 1,
            Keypad = 2,
            WallButton = 4,
            Hack = 8,
            Search = 16,
            All = Door | Keypad | WallButton | Hack | Search,
        }

        public enum ImpactMaterial
        {
            None,
            Metal,
            Flesh
        }

        public enum Team
        {
            Own,
            Opponent
        }

    #endregion
}
