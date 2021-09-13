using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.CharacterElements
{
    public class AimLookAt : MonoBehaviour
    {
        #region Variables

            [SerializeField] private Transform aimLookAtMaster;
            [SerializeField] private Character ownerCharacter;

        #endregion

        private void Update()
        {
            if(ownerCharacter.IsCharacterActive) transform.position = aimLookAtMaster.position;
        }

    }
}
