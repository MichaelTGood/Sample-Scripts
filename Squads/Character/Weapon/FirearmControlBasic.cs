using Kkachi;
using Squads.Inputs;
using Squads.Weapons;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace Squads.CharacterElements
{   
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(TwoHandBehaviour))]
    public class FirearmControlBasic : WeaponControl
    {
        #region Variables

            [Header("Aiming")]
            [SerializeField] private Rig aimRigLayer;
            [Range(0, 10)]
            [SerializeField] private float aimDrawSpeed = 1;
            [Range(0,1)]
            [SerializeField] private float fireWhileAimingAnimationTolerance = 0.8f;

            [Header("Weapon Objects")]
            [SerializeField] private Firearm weapon;

            private bool isAiming;
            private bool isFiring;
            private bool singleFiredShot;
            private float shotTimer;
            private float previousShotTimerCheck;
            float firingSpeed;
            private Character character;
            private TwoHandBehaviour hands;


        #endregion

        private void Awake()
        {
            character = GetComponent<Character>();
            hands = GetComponent<TwoHandBehaviour>();

            aimRigLayer.weight = 0;

            if(weapon == null)
            {
                foreach(var firearm in gameObject.GetComponentsInDirectChildren<Firearm>())
                {
                    if(firearm != null)
                    {
                        Equip(firearm);
                        break;
                    }
                }
            }

            firingSpeed = 1f / weapon.ShotsPerSecond;
        }

        #region Enablers and Disablers

            private void OnEnable()
            {
                character.EnableInput += EnableInput;
                character.DisableInput += DisableInput;
            }

            private void OnDisable()
            {
                character.EnableInput -= EnableInput;
                character.DisableInput -= DisableInput;
            }

            private void EnableInput()
            {
                MatchInputManager.Inputs.Character.Aim.started += StartAiming;
                MatchInputManager.Inputs.Character.Aim.canceled += _ => { isAiming = false; };
                MatchInputManager.Inputs.Character.Fire.performed += _ => { isFiring = true; };
                MatchInputManager.Inputs.Character.Fire.canceled += ResetFiring;

            }

            private void DisableInput()
            {
                MatchInputManager.Inputs.Character.Aim.started -= StartAiming;
                MatchInputManager.Inputs.Character.Aim.canceled -= _ => { isAiming = false; };
                MatchInputManager.Inputs.Character.Fire.performed -= _ => { isFiring = true; };
                MatchInputManager.Inputs.Character.Fire.canceled -= ResetFiring;

            }

        #endregion

        private void Update()
        {
            if(!character.IsCharacterActive) return;

            if(isFiring && aimRigLayer.weight > fireWhileAimingAnimationTolerance) Fire();

            AnimateAimRigLayer();
        }

        private void AnimateAimRigLayer()
		{
			if (isAiming)
			{
				aimRigLayer.weight = Mathf.Clamp01(aimRigLayer.weight + (Time.deltaTime * aimDrawSpeed));
			}
			else
			{
				aimRigLayer.weight = Mathf.Clamp01(aimRigLayer.weight - (Time.deltaTime * aimDrawSpeed));
			}
		}

        private void StartAiming(InputAction.CallbackContext ctx)
        {
            isAiming = true;
            firingSpeed = 1f / weapon.ShotsPerSecond;
        }

        private void ResetFiring(InputAction.CallbackContext ctx)
        {
            isFiring = false; 
            singleFiredShot = false;
            shotTimer = 0;
        }

        private void Fire()
        {
            if(!weapon.CanAutoFire)
            {
                if(singleFiredShot) return;
                else singleFiredShot = true;
            } 

            var timeSinceLastShot = Time.realtimeSinceStartup - previousShotTimerCheck;

            // Without this, the first shot fired will be skipped,
            // as the updateTime will be counting time when the player was not attempting to shoot.
            if(timeSinceLastShot > firingSpeed * 2) timeSinceLastShot = firingSpeed;

            shotTimer += timeSinceLastShot;
            previousShotTimerCheck = Time.realtimeSinceStartup;

            if(shotTimer >= firingSpeed)
            {
                shotTimer -= firingSpeed;
                if(shotTimer >= firingSpeed) shotTimer = 0;

                if(weapon != null)
                {
                    weapon.Fire(character.Team);    
                }
            }
        }

        private void Equip(Firearm newWeapon)
        {
            weapon = newWeapon;
            hands.NewHandsPlacement(weapon.GripTrigger, weapon.GripSupport);
        }

        #region Debug Functions

            [ContextMenu("Toggle IsAiming")]
            public void ToggleIsAiming()
            {
                isAiming = !isAiming;
            }

        #endregion

    }
}
