using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Kkachi;
using Squads.Inputs;

namespace Squads.CharacterElements
{
    public class LocomotionBasic : Locomotion
    {
        #region Variables

            [SerializeField] private Transform cameraTransform;

            [Header("Movement")]
            [Tooltip("The clamp for the speed, when the walk button is pressed. Not used for analog input.")]
            [SerializeField] private float walkSpeed = 0.333f;
            [Tooltip("How fast the character turns toward the camera's direction on movement. Higher is faster.")]
            [Range(5, 10)]
            [SerializeField] private float turnSpeed = 10;
            [SerializeField] private float jumpHeight;
            [SerializeField] private float jumpMultiplier;


            
            // Locomotion
            private Vector2 moveInput;
            private bool isWalking;
            private bool isSprinting;
            private bool isAiming;

            // Jump and Vertical
            // private float verticalVelocity;
            // private float terminalVelocity = 53f;
            
            // Animator
            private Animator animator;
            private int anim_VelocityX;
            private int anim_VelocityZ;
            private int anim_Jump;

            // Character Components
            private Character character;
            private Rigidbody rb;
            private CharacterController characterController;

		#endregion

		private void Awake()
		{
			if (cameraTransform == null) cameraTransform = Camera.main.transform;

			animator = GetComponent<Animator>();
			AssignAnimationIDs();

            character = GetComponent<Character>();
            characterController = GetComponent<CharacterController>();
            rb = GetComponent<Rigidbody>();
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
                MatchInputManager.Inputs.Character.Move.canceled += _ => moveInput = Vector2.zero;
                MatchInputManager.Inputs.Character.Aim.performed += _ => { isAiming = true; };
                MatchInputManager.Inputs.Character.Aim.canceled += _ => { isAiming = false; };
                MatchInputManager.Inputs.Character.Walk.performed += _ => { isWalking = true; };
                MatchInputManager.Inputs.Character.Walk.canceled += _ => { isWalking = false; };
            }

            private void DisableInput()
            {
                MatchInputManager.Inputs.Character.Move.canceled -= _ => moveInput = Vector2.zero;
                MatchInputManager.Inputs.Character.Aim.performed -= _ => { isAiming = true; };
                MatchInputManager.Inputs.Character.Aim.canceled -= _ => { isAiming = false; };
                MatchInputManager.Inputs.Character.Walk.performed -= _ => { isWalking = true; };
                MatchInputManager.Inputs.Character.Walk.canceled -= _ => { isWalking = false; };
                // Stop character from moving
                moveInput = Vector2.zero;
                Move();
            }

        #endregion

		private void AssignAnimationIDs()
		{
			anim_VelocityX = Animator.StringToHash("VelocityX");
			anim_VelocityZ = Animator.StringToHash("VelocityZ");
			anim_Jump = Animator.StringToHash("Jump");
		}

		private void Update()
		{
			if (!character.IsCharacterActive) return;

			Rotate();

			moveInput = MatchInputManager.Inputs.Character.Move.ReadValue<Vector2>();
			Move();

		}



		private void Rotate()
		{
			if (isAiming) // Turn while aiming.
			{
				Quaternion yawRotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);
				transform.rotation = Quaternion.Slerp(transform.rotation, yawRotation, turnSpeed * Time.deltaTime);

			}
			else if (moveInput != Vector2.zero) // Turn while moving
			{
				// Note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
				Quaternion yawRotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);
				transform.rotation = Quaternion.Slerp(transform.rotation, yawRotation, turnSpeed * Time.deltaTime);
			}
		}

        private void Move()
        {
            if(isAiming || isWalking)
            {
                moveInput.ClampFromInverse(walkSpeed);
            }

            animator.SetFloat(anim_VelocityX, moveInput.x);
            animator.SetFloat(anim_VelocityZ, moveInput.y);
        }





        private IEnumerator Jump()
        {
            Vector3 jumpForce = new Vector3(characterController.velocity.x * jumpMultiplier, jumpHeight, characterController.velocity.z * jumpMultiplier);
            // Vector3 jumpForce = new Vector3(0, jumpHeight, 0);
            Debug.Log($"Jump force: {jumpForce}");
            characterController.enabled = false;
            animator.applyRootMotion = false;
            rb.AddForce(jumpForce, ForceMode.Impulse);
            
            animator.SetTrigger(anim_Jump);

            yield return new WaitForSeconds(0.85f);
            characterController.enabled = true;
            animator.applyRootMotion = true;

            yield return null;
        }
        
    }
}
