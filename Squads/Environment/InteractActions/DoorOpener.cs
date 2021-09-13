using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.Environment
{
	public class DoorOpener : InteractAction
	{
		#region Variables

            [SerializeField] private bool startsOpen;

            [Header("Animation")]
            [SerializeField] private Animator doorAnimator;
            [Tooltip("Assumes that if the animator's bool = true, the door is open.")]
            [SerializeField] private string animationBoolName;

            [Header("Door's Light Components")]
            [SerializeField] private Light lockedLight;
            [SerializeField] private Light unlockedLight;
            

            private int anim_bool;
            private bool isOpen;


		#endregion

        private void Awake()
        {
            anim_bool = Animator.StringToHash(animationBoolName);

            isOpen = startsOpen;
            doorAnimator.SetBool(anim_bool, isOpen);
            
        }

		public override void TriggerAction()
		{
            isOpen = !isOpen;
            SwapLights();
            doorAnimator.SetBool(anim_bool, isOpen);
		}

        private void SwapLights()
        {
            if(lockedLight == null || unlockedLight == null) return;

            unlockedLight.enabled = isOpen;
            lockedLight.enabled = !isOpen;
        }


	}
}
