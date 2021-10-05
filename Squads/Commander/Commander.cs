using Cinemachine;
using Kkachi;
using Squads.CharacterElements;
using Squads.Inputs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Squads.CommanderElements
{
    public class Commander : MonoBehaviour
    {
        #region Variables

            [SerializeField] private List<Character> characters;
            [SerializeField] private CinemachineVirtualCamera commanderCamera;


        #endregion

        private void Awake()
        {
            commanderCamera.Priority = 1;

            RemoveNullCharacters();

            foreach(var character in characters)
            {
                character.Deactivate();
            }

            MatchInputManager.Inputs.Commander.Enable();


        }

        private void OnEnable()
        {
            MatchInputManager.Inputs.Commander.SwitchCharacter.performed += OnSwitchCharacter;
            MatchInputManager.Inputs.Commander.Quit.performed += _ => { Application.Quit(); };
        }

        private void OnDisable()
        {
            MatchInputManager.Inputs.Commander.SwitchCharacter.performed -= OnSwitchCharacter;
            MatchInputManager.Inputs.Commander.Quit.performed -= _ => { Application.Quit(); };
        }

        public void OnSwitchCharacter(InputAction.CallbackContext ctx)
        {
            var nextCharacter = characters.FirstFromPool();
            nextCharacter.Activate();

        }

        public void AddCharacter(Character character)
        {
            if(characters.Contains(character)) return;

            characters.Add(character);
        }

        private void RemoveNullCharacters()
        {
            for (int i = characters.Count - 1; i >= 0; i--)
            {
                if(characters[i] == null) characters.RemoveAt(i); 
            }

        }

    }
}
