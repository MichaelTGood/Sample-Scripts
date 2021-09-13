using Cinemachine;
using Squads.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

namespace Squads.CharacterElements
{
    public class CameraControlBasic : MonoBehaviour
    {
        #region Variables

            [Header("Cameras")]
            [SerializeField] private CinemachineFreeLook characterCamera;
            [SerializeField] private CinemachineFreeLook aimCamera;
            private CinemachineFreeLook currentCamera;

            [Header("Camera Look Speeds")]
            [SerializeField] private Vector2 gamepadSpeed;
            [SerializeField] private Vector2 mouseSpeed;

            private const string gamepadScheme = "Gamepad";
            private const string mouseScheme = "Keyboard and Mouse";

            private Vector2 aimInput;
            private Character character;

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
            MatchInputManager.Inputs.Character.Aim.started += _ => { SetAimingCamera(true); };
            MatchInputManager.Inputs.Character.Aim.canceled += _ => { SetAimingCamera(false); };
        }

        private void OnDisable()
        {
            MatchInputManager.Inputs.Character.Aim.started -= _ => { SetAimingCamera(true); };
            MatchInputManager.Inputs.Character.Aim.canceled -= _ => { SetAimingCamera(false); };
        }



        private void LateUpdate()
		{
            if(!character.IsCharacterActive) return;

            aimInput = MatchInputManager.Inputs.Character.Look.ReadValue<Vector2>();
            Look();

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
		
        private void SetAimingCamera(bool isAiming)
        {
            if(isAiming)
            {
                aimCamera.m_XAxis.Value = characterCamera.m_XAxis.Value;
                aimCamera.m_YAxis.Value = characterCamera.m_YAxis.Value;
                aimCamera.Priority = 3;
                currentCamera = aimCamera;
            }
            else
            {
                characterCamera.m_XAxis.Value = aimCamera.m_XAxis.Value;
                characterCamera.m_YAxis.Value = aimCamera.m_YAxis.Value;
                aimCamera.Priority = 0;
                currentCamera = characterCamera;
            }
        }



	}
}
 