using System.Collections;
using System.Collections.Generic;
using Squads.Weapons;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace Squads.CharacterElements
{
    [CreateAssetMenu(menuName = "Squads/Character", fileName = "New Character")]
    public class CharacterSO : ScriptableObject
    {
        #region Variables

            #region Character Attributes

                [Title("Character's Name"), ColoredBoxGroup("Character Attributes", 0, 1, 0, 0.5f, true, true, true, MarginBottom = 15), HideLabel]
                [SerializeField] private string characterName;

                [Title("Point Cost"), HorizontalGroup("Character Attributes/Horizontal"), HideLabel]
                [SerializeField] private int cost;

                [Title("Starting Health"), HorizontalGroup("Character Attributes/Horizontal"), HideLabel, PropertySpace(0,15)]
                [SerializeField] private float startingHealth;


            #endregion

            #region Physical Character Attributes

                [ColoredBoxGroup("Physical Character Attributes", 0,0,1,0.5f, true, true, true, MarginBottom = 15)]
                [Title("Mesh"), HorizontalGroup("Physical Character Attributes/First"), HideLabel, PreviewField(200, ObjectFieldAlignment.Center)]
                [SerializeField] private Mesh characterMesh;

                [Title("Mesh Material"), HorizontalGroup("Physical Character Attributes/First"), HideLabel, PreviewField(150, ObjectFieldAlignment.Center)]
                [SerializeField] private Material CharacterMeshMaterial;

                // [Title("Animation Controller"), HorizontalGroup("Physical Character Attributes/Animations"), HideLabel]
                // [SerializeField] private  animationController;
                /* 
                    AnimatorController is an UnityEditor component, and thus, can not easily be set in a SO, or loaded at runtime.
                    To do it, you have to perform a Resources.Load("String/to/the/file").
                    ie: animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/anim_Controller");


                    Consider making Characters also Prefabs. And still use SO? (for pre-match menus, 
                        and to load other data/settings on character.)
                */

                [Title("Avatar"), HorizontalGroup("Physical Character Attributes/Animations"), HideLabel, PropertySpace(0,15)]
                [SerializeField] private Avatar avatar;

            #endregion

            #region Control Components

                [Title("Locomotion"), ColoredBoxGroup("Control Components", 0.25f, 1, 1, 0.5f, true, true, true, MarginBottom = 15), EnumToggleButtons, HideLabel]
                [SerializeField] 
                private LocomotionTypes locomotion;

                [Title("Camera Control"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Control Components")]
                [SerializeField] 
                private CameraControlTypes cameraControl;

                [Title("Weapon Control"), EnumToggleButtons, HideLabel, ColoredBoxGroup("Control Components"), PropertySpace(0,15)]
                [SerializeField] 
                private WeaponControlTypes weaponControl;

            #endregion

            #region Weapons

                [Title("Maximum Carriable Weapons"), HideLabel, ColoredBoxGroup("Weapons", 1,0,0,0.5f, true, true, true, MarginBottom = 15), ProgressBar(0,3,1,1, 1, Segmented =true, DrawValueLabel = true, ValueLabelAlignment = TextAlignment.Center)]
                [SerializeField] private int maximumCarriableWeapons;
                [Title("Usable Weapon Types"), HideLabel, ColoredBoxGroup("Weapons"), GUIColor(1,1,1, 1), EnumToggleButtons, PropertySpace(0, 15)]
                [SerializeField] private WeaponTypes usableWeaponTypes;

            #endregion

            #region Abilites

                [Title("Abilities"), HideLabel, ColoredBoxGroup("Actions", 0.25f, 0, 1, 0.5f, true, true, true, MarginBottom = 15), EnumToggleButtons, PropertySpace(0, 15)]
                [SerializeField] private Interact allowedInteractions;

            #endregion

            #region Accessors

            #endregion


        #endregion




    }

}
