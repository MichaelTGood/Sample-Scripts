using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Squads.CharacterElements;
using UnityEngine;

namespace Squads.CommanderElements
{
    public class CharacterLoader : MonoBehaviour
    {
        #region Variables

            [Header("Commander")]
            [SerializeField] private Commander commander;

            [Header("Character Prefabs")]
            [SerializeField] private CharacterPrefab characterPrefab;

            [Header("Charactets To Load")]
            [InlineEditor][SerializeField] private AssembledCharacter assembledCharacter;

            [Header("Loading Variables")]
            [SerializeField] private Vector3 startingPosition;

            private Dictionary<string, System.Type> characterComponents = new Dictionary<string, System.Type>();

        #endregion

        private void Awake()
        {
            GetAllChildClasses();
        }

        [Button()]
        public void LoadCharacters()
        {
            var characterToLoad = Instantiate(characterPrefab, startingPosition, Quaternion.identity);
            characterToLoad.name = assembledCharacter.characterSO.CharacterName;

            // Loads data into Character component, ie: Name, Health, etc.
            characterToLoad.Character.LoadCharacterData(assembledCharacter.GetCharacterData());

            // Loads components, ie: Locomotion, CameraControl, etc.
            foreach(string component in assembledCharacter.GetCharacterComponents())
            {
                if(characterComponents.TryGetValue(component, out Type componentType))
                {
                    characterToLoad.CharacterObj.AddComponent(componentType);
                }
                else
                {
                    Debug.Log($"Coundn't find component: {component}.");
                }
            }

            // Load all upgrades
            foreach(var upgrade in assembledCharacter.Upgrades)
            {
                foreach(var upgradeItem in upgrade.upgradeList)
                {
                    upgradeItem.ExecuteAtStartOfMatch(characterToLoad);
                }

            }

            commander.AddCharacter(characterToLoad.Character);
            

        }





        /// <summary> Collects all child classes of character components (locomotion, interactors, etc), to be used during character loading.
        /// </summary>
        private void GetAllChildClasses()
        {

            var interactors = Assembly.GetAssembly(typeof(Interactor)).GetTypes().Where(t => t.IsSubclassOf(typeof(Interactor)));

            foreach(var childClass in interactors)
            {
                if(characterComponents.TryGetValue(childClass.Name, out Type existingComponent)) continue;

                characterComponents.Add(childClass.Name, childClass);
            }


            var hands = Assembly.GetAssembly(typeof(TwoHandBehaviour)).GetTypes().Where(t => t.IsSubclassOf(typeof(TwoHandBehaviour)));

            foreach(var childClass in hands)
            {
                if(characterComponents.TryGetValue(childClass.Name, out Type existingComponent)) continue;
                

                characterComponents.Add(childClass.Name, childClass);
            }


            var cameraControls = Assembly.GetAssembly(typeof(CameraControl)).GetTypes().Where(t => t.IsSubclassOf(typeof(CameraControl)));

            foreach(var childClass in cameraControls)
            {
                if(characterComponents.TryGetValue(childClass.Name, out Type existingComponent)) continue;
                

                characterComponents.Add(childClass.Name, childClass);
            }


            var locomotors = Assembly.GetAssembly(typeof(Locomotion)).GetTypes().Where(t => t.IsSubclassOf(typeof(Locomotion)));

            foreach(var childClass in locomotors)
            {
                if(characterComponents.TryGetValue(childClass.Name, out Type existingComponent)) continue;

                characterComponents.Add(childClass.Name, childClass);
            }


            var weaponControls = Assembly.GetAssembly(typeof(WeaponControl)).GetTypes().Where(t => t.IsSubclassOf(typeof(WeaponControl)));

            foreach(var childClass in weaponControls)
            {
                if(characterComponents.TryGetValue(childClass.Name, out Type existingComponent)) continue;

                characterComponents.Add(childClass.Name, childClass);
            }

        }



    }
}
