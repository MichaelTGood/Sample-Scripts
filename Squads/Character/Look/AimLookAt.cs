using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Squads.CharacterElements
{
    public class AimLookAt : MonoBehaviour
    {
        #region Variables

            [SerializeField] private Transform aimLookAtMaster;
            [SerializeField] private Character ownerCharacter;

            private string aimLookAtMaster_ObjectName = "AimLookAtPoint-Master";

        #endregion

        private void Awake()
        {
            if(ownerCharacter == null) ownerCharacter = transform.parent.GetComponentInChildren<Character>(); 

            if(aimLookAtMaster == null)
            {
                Transform[] children = Camera.main.GetComponentsInChildren<Transform>();

                foreach(var child in children)
                {
                    if(child.name.Contains(aimLookAtMaster_ObjectName))
                    {
                        aimLookAtMaster = child;
                        break;
                    }
                }
            }

        }

        private void Update()
        {
            if(ownerCharacter.IsCharacterActive) transform.position = aimLookAtMaster.position;
        }



    }
}
