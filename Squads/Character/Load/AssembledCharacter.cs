using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Squads.CharacterElements
{
    /// <summary> This is a model of a character, assembled from the CharacterSO and UpgradeSOs.
    /// </summary>
    [System.Serializable]
    public class AssembledCharacter : MonoBehaviour
    {
        #region Variables

        /* 
            REMEMBER: Don't add variables for things that won't change by upgrade! The attached CharacterSO can store these.
            ie: ImpactMaterial

            OR: Things that won't need to change for pre-match preview. ie: Hand Control
        */

            public CharacterSO characterSO;

            private string characterName;
            private int cost;
            private int startingHealth;

            private Mesh characterMesh;
            private Material characterMeshMaterial;
            private Avatar avatar;

            private LocomotionTypes locomotion;
            private CameraControlTypes cameraControl;
            private HandControlTypes handControl;

            private WeaponControlTypes weaponControl;

            private int maximumCarriableWeapons;
            private WeaponTypes usableWeaponTypes;

            private InteractorTypes interactor;

            private UpgradeSO[] upgrades;

            public UpgradeSO[] Upgrades { get => upgrades; }

            public List<UpgradeSO> UpgradesListToTest = new List<UpgradeSO>();


		#endregion

        [Button]
		public void LoadCharacterSO(CharacterSO newCharacterSO)
        {
            if(characterSO != null) ClearAllData();
            
            characterSO = newCharacterSO;
            characterName = characterSO.CharacterName;
            cost = characterSO.Cost;
            startingHealth = characterSO.StartingHealth;

            characterMesh = characterSO.CharacterMesh;
            characterMeshMaterial = characterSO.CharacterMeshMaterial;
            avatar = characterSO.Avatar;

            locomotion = characterSO.Locomotion;
            cameraControl = characterSO.CameraControl;
            handControl = characterSO.HandControl;

            weaponControl = characterSO.WeaponControl;
            maximumCarriableWeapons = characterSO.MaximumCarriableWeapons;
            usableWeaponTypes = characterSO.UsableWeaponTypes;

            interactor = characterSO.Interactor;

            int upgradeCount = 0;
            foreach(UpgradeTypes upgradeType in Enum.GetValues(typeof(UpgradeTypes)))
            {
                if(characterSO.AllowedUpgradeTypes.HasFlag(upgradeType)) upgradeCount++;
            }

            upgrades = new UpgradeSO[upgradeCount];

        }

        private void ClearAllData()
        {
            characterSO = null;
            characterName = null;
            cost = 0;
            startingHealth = 0;

            characterMesh = null;
            characterMeshMaterial = null;
            avatar = null;

            locomotion = LocomotionTypes.None;
            cameraControl = CameraControlTypes.None;
            handControl = HandControlTypes.None;

            weaponControl = WeaponControlTypes.None;
            maximumCarriableWeapons = characterSO.MaximumCarriableWeapons;
            usableWeaponTypes = characterSO.UsableWeaponTypes;

            interactor = InteractorTypes.None;
        }

        /// <summary> Adds an upgrade to character. The slot parameter should be passed from the GUI.
        /// </summary>
        public void AddUpgrade(UpgradeSO newUpgrade, int slot)
        {
            if(!characterSO.AllowedUpgradeTypes.HasFlag(newUpgrade.UpgradeType)) return;
            
            if(upgrades.Contains(newUpgrade)) return;

            if(slot >= upgrades.Length | slot < 0) return;

            upgrades[slot] = newUpgrade;

        }


        #region Data Accessors
            
            public CharacterDataModel GetCharacterData()
            {
                return new CharacterDataModel(characterName, characterSO, startingHealth);
            }

            public List<string> GetCharacterComponents()
            {
                List<string> components = new List<string>();

                components.Add(locomotion.ToString());
                components.Add(cameraControl.ToString());
                components.Add(handControl.ToString());
                components.Add(weaponControl.ToString());
                components.Add(interactor.ToString());

                return components;
            }
        
        #endregion

    
        #region Debug

            [Button("Load Upgrades")]
            public void LoadUpgrades_DebugOnly()  
            {
                upgrades = new UpgradeSO[UpgradesListToTest.Count];

                int i = 0;
                foreach(var upgrade in UpgradesListToTest)
                {
                    upgrades[i] = upgrade;
                    i++;
                }
            }

        #endregion
    
    }

    public class CharacterDataModel
    {
        public string CharacterName;
        public CharacterSO CharacterSO;
        public int StartingHealth;

		public CharacterDataModel(string characterName, CharacterSO characterSO, int startingHealth)
		{
			CharacterName = characterName;
			CharacterSO = characterSO;
			StartingHealth = startingHealth;
		}
	}
}
