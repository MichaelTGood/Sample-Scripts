using System;
using UnityEngine;

namespace Squads.Environment
{
    public abstract class Interactable : MonoBehaviour
    {
        #region Variables

        public abstract InteractAbilities InteractType { get; }
        
        [SerializeField] private InteractAction action;

        public bool RequiresTwoHands;
        public Transform InteractPoint;
        public Transform OffHandPoint;

        #endregion

        private void Awake()
        {
            VerifySingleInteractType();

            if(action == null) action = GetComponent<InteractAction>();
        }

        public virtual void Interact()
        {
            action.TriggerAction();
        }

        private void VerifySingleInteractType()
        {
            int count = 0;
            
            foreach(InteractAbilities interaction in Enum.GetValues(typeof(InteractAbilities)))
            {

                if(interaction.ToString() == "None") continue;

                if(InteractType.HasFlag(interaction))
                {
                    count++;

                    if(count > 1)
                    {
                        Debug.LogError($"Interactable \"{this.name}\" has too many InteractTypes!");
                        break;
                    }
                }
            }
        }

    }
}
