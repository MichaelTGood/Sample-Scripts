using Cinemachine;
using Squads.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

namespace Squads.CharacterElements
{
    public class CameraControlBasic : CameraControl
    {
        #region Variables

            [Header("Cameras")]
            [SerializeField] private CinemachineFreeLook characterCamera;
            [SerializeField] private CinemachineFreeLook aimCamera;
            private CinemachineFreeLook currentCamera;

            [Header("Aim Settings")]
            [SerializeField] private int normalFOV = 40;
            [SerializeField] private int aimFOV = 15;
            [Range(1,100)]
            [SerializeField] private float fovChangeSpeed = 2f;

            [Header("Camera Look Speeds")]
            [SerializeField] private Vector2 gamepadSpeed;
            [SerializeField] private Vector2 mouseSpeed;

            private const string gamepadScheme = "Gamepad";
            private const string mouseScheme = "Keyboard and Mouse";

            private bool isAiming = false;
            private bool adjustFOV;
            private Vector2 aimInput;
            private Character character;

            private float cameraFOV {
                get => characterCamera.m_Lens.FieldOfView;
                set => characterCamera.m_Lens.FieldOfView = value;
            }

        #endregion


        private void Awake()
        {
            if(characterCamera == null)
            {
                characterCamera = transform.parent.GetComponentInChildren<CinemachineFreeLook>();
            }
            currentCamera = characterCamera;

            character = GetComponent<Character>();
        }

        private void OnEnable()
        {
            isAiming = false;
            MatchInputManager.Inputs.Character.Aim.started += SetAiming;
            MatchInputManager.Inputs.Character.Aim.canceled += SetAiming;
        }

        private void OnDisable()
        {
            MatchInputManager.Inputs.Character.Aim.started -= SetAiming;
            MatchInputManager.Inputs.Character.Aim.canceled -= SetAiming;
        }



        private void LateUpdate()
		{
            if(!character.IsCharacterActive) return;

            aimInput = MatchInputManager.Inputs.Character.Look.ReadValue<Vector2>();
            Look();

            if(adjustFOV) ChangeAimFOV(Time.deltaTime);

		}

		public void Look()
        {
            currentCamera.m_XAxis.m_InputAxisValue = aimInput.x;
            currentCamera.m_YAxis.m_InputAxisValue = aimInput.y;
        }


        public void OnControlsChanged(PlayerInput input)
        {
			SetCameraControlSpeed(input.currentControlScheme);
        }

		private void SetCameraControlSpeed(string scheme)
		{
			switch (scheme)
			{
				case gamepadScheme:
					characterCamera.m_XAxis.m_MaxSpeed = gamepadSpeed.x;
					characterCamera.m_YAxis.m_MaxSpeed = gamepadSpeed.y;
					break;
				case mouseScheme:
					characterCamera.m_XAxis.m_MaxSpeed = mouseSpeed.x;
					characterCamera.m_YAxis.m_MaxSpeed = mouseSpeed.y;
					break;
				default:
					Debug.LogError("Unknown Control Scheme");
					break;
			}
		}
		
        private void SetAiming(InputAction.CallbackContext ctx)
        {
            isAiming = !isAiming;
            adjustFOV = true;
        }

        private void ChangeAimFOV(float deltaTime)
        {
            float fovAdjustment = deltaTime * fovChangeSpeed;
            if(isAiming) fovAdjustment *= -1;

            cameraFOV += fovAdjustment;

            if(isAiming && cameraFOV <= aimFOV)
            {
                cameraFOV = aimFOV;
                adjustFOV = false;
            }
            else if (!isAiming && cameraFOV >= normalFOV)
            {
                cameraFOV = normalFOV;
                adjustFOV = false;
            }

        }


	}
}
 