using System.Collections;
using System.Collections.Generic;
using Squads.Weapons;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using System.Linq;

namespace Squads.CharacterElements
{
    [CreateAssetMenu(menuName = "Squads/Character", fileName = "New Character")]
    public class CharacterSO : ScriptableObject
    {
        #region Variables

            #region Character Attributes

                [Title("Character's Name"), ColoredBoxGroup("Character Attributes", 0, 1, 0, 0.5f, true, true, true, MarginBottom = 15), HideLabel]
                public string CharacterName;

                [Title("Point Cost"), HorizontalGroup("Character Attributes/Horizontal"), HideLabel]
                public int Cost;

                [Title("Starting Health"), HorizontalGroup("Character Attributes/Horizontal"), HideLabel, PropertySpace(0,15)]
                public int StartingHealth;


            #endregion

            #region Physical Character Attributes

                [ColoredBoxGroup("Physical Character Attributes", 0,0,1,0.5f, true, true, true, MarginBottom = 15)]
                [Title("Mesh"), HorizontalGroup("Physical Character Attributes/First"), HideLabel, InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Boxed)]
                public Mesh CharacterMesh;

                [Title("Mesh Material"), HorizontalGroup("Physical Character Attributes/First"), HideLabel, InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Boxed)]
                public Material CharacterMeshMaterial;

                // [Title("Animation Controller"), HorizontalGroup("Physical Character Attributes/Animations"), HideLabel]
                // public  animationController;
                /* 
                    AnimatorController is an UnityEditor component, and thus, can not easily be set in a SO, or loaded at runtime.
                    To do it, you have to perform a Resources.Load("String/to/the/file").
                    ie: animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/anim_Controller");


                    Consider making Characters also Prefabs. And still use SO? (for pre-match menus, 
                        and to load other data/settings on character.)
                */

                [Title("Avatar"), HorizontalGroup("Physical Character Attributes/Animations"), EnumToggleButtons, HideLabel, PropertySpace(0,15)]
                public ImpactMaterial ImpactMaterial;

                [Title("Avatar"), HorizontalGroup("Physical Character Attributes/Animations"), HideLabel, PropertySpace(0,15)]
                public Avatar Avatar;

            #endregion

            #region Control Components

                [Title("Locomotion"), ColoredBoxGroup("Control Components", 0.25f, 1, 1, 0.5f, true, true, true, MarginBottom = 15), EnumToggleButtons, HideLabel]
                public LocomotionTypes Locomotion;

                [Title("Camera Control"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Control Components")]
                public CameraControlTypes CameraControl;

                [Title("Hand Control"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Control Components")]
                public HandControlTypes HandControl;

            #endregion

            #region Weapons

                [Title("Weapon Control"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Control Components"), PropertySpace(0,15)]
                public WeaponControlTypes WeaponControl;
                [Title("Maximum Carriable Weapons"), HideLabel, ColoredBoxGroup("Weapons", 1,0,0,0.5f, true, true, true, MarginBottom = 15), ProgressBar(0,3,1,1, 1, Segmented =true, DrawValueLabel = true, ValueLabelAlignment = TextAlignment.Center)]
                public int MaximumCarriableWeapons;
                [Title("Usable Weapon Types"), HideLabel, ColoredBoxGroup("Weapons"), GUIColor(1,1,1, 1), EnumToggleButtons, PropertySpace(0, 15)]
                public WeaponTypes UsableWeaponTypes;

            #endregion

            #region Abilites
                
                [Title("Interact Controls"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Actions", 0.25f, 0, 1, 0.5f, true, true, true, MarginBottom = 15)]
                public InteractorTypes Interactor;
                [Title("Abilities"), ReadOnly, HideLabel, EnumToggleButtons, ColoredBoxGroup("Actions"), PropertySpace(0, 15)]
                public InteractAbilities AllowedInteractions;

            #endregion

            #region Upgrades

                [Title("Upgrades Types"), HideLabel, ColoredBoxGroup("Upgrades", 0, 0.5f, 0.5f, 0.75f, true, true, true, MarginBottom = 15), EnumToggleButtons, PropertySpace(0, 15)]
                public UpgradeTypes AllowedUpgradeTypes;

            #endregion


        #endregion


    }

}
