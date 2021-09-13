using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.CharacterElements
{
    public class AimRaycastAt : MonoBehaviour
    {
        #region Variables

            [SerializeField] private Character ownerCharacter;

            [SerializeField] private Transform mainCameraTransform;

            Ray ray;
            RaycastHit hitInfo;

        #endregion

        private void Awake()
        {
            if(ownerCharacter == null) ownerCharacter = transform.parent.GetComponentInChildren<Character>(); 
        }

        private void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        private void Update()
        {

            if(ownerCharacter.IsCharacterActive)
            {
                ray.origin = mainCameraTransform.position;
                ray.direction = mainCameraTransform.forward;
                if(Physics.Raycast(ray, out hitInfo, 1000, 12, QueryTriggerInteraction.Ignore))
                {
                    transform.position = hitInfo.point;
                }
                else
                {
                    transform.position = ray.origin + (ray.direction * 1000f);
                }
            }
        }

    }
}
