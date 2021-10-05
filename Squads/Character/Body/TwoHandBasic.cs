using System;
using Kkachi;
using UnityEngine;

namespace Squads.CharacterElements
{
    public class TwoHandBasic : TwoHandBehaviour
    {
        #region Variables

            [Header("Physical Points")]
            [SerializeField] private Transform attachedInteractPoint;
            [SerializeField] private Transform rightHandIK;
            [SerializeField] private Transform leftHandIK;

            [SerializeField] private float defaultLerpTime = 1f;
            private float timerLength;
            private float timer;

            // Hand Management
            [SerializeField] private Transform rightHandSync;
            [SerializeField] private Transform leftHandSync;
            private Transform rightHandPrevious;
            private Transform leftHandPrevious;

            // States
            private bool hasHandPlacements = true;
            private bool isLerping;

        #endregion

        private void Awake()
        {
            if(rightHandSync == null || leftHandSync == null) hasHandPlacements = false;
        }

        private void Update()
        {
            if(!hasHandPlacements) return;

            if(!isLerping)
            {
                MaintainHandPosition();
            }
            else
            {
                LerpHandsToNewPosition(Time.deltaTime);
            }
        }

		private void LerpHandsToNewPosition(float deltaTime)
		{
            timer += deltaTime;
            float progress = timer/timerLength;

            rightHandIK.LerpPositionAndRotation(rightHandSync, progress);
            leftHandIK.LerpPositionAndRotation(leftHandSync, progress);

            // To experiment with removing jittery hands on Interact
            // attachedInteractPoint.LerpPositionAndRotation(leftHandSync, progress);
            // leftHandIK.MatchExact(attachedInteractPoint);

            if(progress >= 1) isLerping = false;
		}

		private void MaintainHandPosition()
        {
            rightHandIK.MatchExact(rightHandSync);
            leftHandIK.MatchExact(leftHandSync);

            // To experiment with removing jittery hands on Interact
            // leftHandIK.MatchExact(attachedInteractPoint);
        }

        public override void NewHandsPlacement(Transform newRightHand, Transform newLeftHand, float transitionLength)
        {
            rightHandPrevious = rightHandSync;
            leftHandPrevious = leftHandSync;

            rightHandSync = newRightHand;
            leftHandSync = newLeftHand;

            timer = 0;
            timerLength = transitionLength;
            hasHandPlacements = true;
            isLerping = true;
        }

		public override void NewHandsPlacement(Transform newRightHand, Transform newLeftHand) => NewHandsPlacement(newRightHand, newLeftHand, defaultLerpTime);


		public override void NewRightHandPlacement(Transform newRightHand, float transitionLength) => NewHandsPlacement(newRightHand, leftHandSync, transitionLength);

		public override void NewRightHandPlacement(Transform newRightHand) => NewHandsPlacement(newRightHand, leftHandSync, defaultLerpTime);


		public override void NewLeftHandPlacement(Transform newLeftHand, float transitionLength) => NewHandsPlacement(rightHandSync, newLeftHand, transitionLength);

		public override void NewLeftHandPlacement(Transform newLeftHand) => NewHandsPlacement(rightHandSync, newLeftHand, defaultLerpTime);


		public override void NewOffHandPlacement(Transform newOffHand, float transitionLength) => NewHandsPlacement(rightHandSync, newOffHand, transitionLength);

		public override void NewOffHandPlacement(Transform newOffHand) => NewHandsPlacement(rightHandSync, newOffHand, defaultLerpTime);


		public override void ReturnToPreviousHandsPlacement(float transitionLength)
        {
            Transform tempRight = rightHandSync;
            Transform tempLeft = leftHandSync;

            rightHandSync = rightHandPrevious;
            leftHandSync = leftHandPrevious;

            rightHandPrevious = tempRight;
            leftHandPrevious = tempLeft;

            timer = 0;
            timerLength = transitionLength;
            isLerping = true;
        }

		public override void ReturnToPreviousHandsPlacement() => ReturnToPreviousHandsPlacement(defaultLerpTime);

	}
}
