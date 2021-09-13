using System;
using Cinemachine;
using Squads.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Squads.CharacterElements
{
    public class Character : MonoBehaviour, IHitable
    {
        #region Variables
            
            [SerializeField] private string characterName;
            [SerializeField] private TeamColor team;
            [SerializeField] private ImpactMaterial impactMaterial;
            [SerializeField] private CinemachineFreeLook cinemachineFreeLookCamera;
            
            // Character Stats
            private int startingHealth;
            private int health;
            private bool isCharacterActive;

            // Events
            public event Action EnableInput;
            public event Action DisableInput;

            // Accessors
            /// <summary> Is this character the currently active character for the player.
            /// </summary>
            public bool IsCharacterActive { get => isCharacterActive; }
            public string CharacterName { get => characterName; }
            public TeamColor Team { get => team; }
            public int Health { get => health; }

		#endregion

        private void Awake()
        {
            health = startingHealth;
        }

		private void OnEnable()
        {
            MatchInputManager.Inputs.Character.Deactivate.performed += Deactivate;
        }

		private void OnDisable()
        {
            MatchInputManager.Inputs.Character.Deactivate.performed -= Deactivate;
        }

        [ContextMenu("Activate")]
		public void Activate()
        {
            cinemachineFreeLookCamera.Priority = 2;
            MatchInputManager.SwitchToActionMap(MatchInputManager.Inputs.Character);
            isCharacterActive = true;
            EnableInput?.Invoke();
        }

		public void Deactivate() => Deactivate(new InputAction.CallbackContext());

		public void Deactivate(InputAction.CallbackContext ctx)
        {
            cinemachineFreeLookCamera.Priority = 0;
            MatchInputManager.SwitchToActionMap(MatchInputManager.Inputs.Commander);
            isCharacterActive = false;
            DisableInput?.Invoke();
        }

		public void Hit(int damage, out ImpactMaterial impactMaterial, TeamColor damagingTeam = TeamColor.None)
		{
            impactMaterial = this.impactMaterial;
            
            if(damagingTeam == team) return;

            if(health > 0) health = Mathf.Clamp(health - damage, 0, startingHealth);
        }
	}
}
