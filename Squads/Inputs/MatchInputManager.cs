using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Squads.Inputs
{
    public class MatchInputManager : MonoBehaviour
    {
        #region Variables

            public static MainInputs Inputs = new MainInputs();

        #endregion

        public static void SwitchToActionMap(InputActionMap actionMap)
        {
            if(actionMap.enabled) return;

            Inputs.Disable();
            actionMap.Enable();
        }

        public static void DeactivateActionMap(InputActionMap actionMap)
        {
            actionMap.Disable();
        }

    }
}
