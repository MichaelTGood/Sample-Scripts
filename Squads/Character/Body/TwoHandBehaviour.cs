using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.CharacterElements
{
    public abstract class TwoHandBehaviour : MonoBehaviour
    {
        #region Variables

        

        #endregion



        public abstract void NewHandsPlacement(Transform newRightHand, Transform newLeftHand, float transitionLength);

        public abstract void NewHandsPlacement(Transform newRightHand, Transform newLeftHand);

        public abstract void ReturnToPreviousHandsPlacement(float transitionLength);

        public abstract void ReturnToPreviousHandsPlacement();

        public abstract void NewRightHandPlacement(Transform newRightHand, float transitionLength);

        public abstract void NewRightHandPlacement(Transform newRightHand);

        public abstract void NewLeftHandPlacement(Transform newLeftHand, float transitionLength);

        public abstract void NewLeftHandPlacement(Transform newLeftHand);

        public abstract void NewOffHandPlacement(Transform newOffHand, float transitionLength);

        public abstract void NewOffHandPlacement(Transform newOffHand);

    }
}
