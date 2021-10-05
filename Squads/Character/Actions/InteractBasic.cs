using Squads.Inputs;
using Squads.Environment;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using System;
using System.Collections;

namespace Squads.CharacterElements
{
    public class InteractBasic : Interactor
    {
        #region Variables

            [Header("Character Properties")]
            [SerializeField] private InteractAbilities allowedInteractions;

            [SerializeField] private TwoBoneIKConstraint rightHandConstraint;
            [SerializeField] private TwoBoneIKConstraint leftHandConstraint;

            [Header("Animation")]
            [Range(0,1)]
            [SerializeField] private float fadeOutAnimationSpeed = 0.5f;

            [Header("Interactable Properties")]
            [SerializeField] private Transform interactSpot;
            [SerializeField] private Interactable currentInteractable;

            // Animator
            private Animator animator;
            private int anim_UpperBodyActionsLayer;

            // Animations
            private int animationFramerate = 60;
            private Dictionary<string, int> animations = new Dictionary<string, int>();
            private string currentAnimation;

            private AnimationState interactAnimation = AnimationState.Finished;



        #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        anim_UpperBodyActionsLayer = animator.GetLayerIndex("Upper Body Actions");
        animator.SetLayerWeight(anim_UpperBodyActionsLayer, 1);
        
        // Collect allowed Interactions, and connect them to animation triggers.
        foreach(InteractAbilities interaction in Enum.GetValues(typeof(InteractAbilities)))
        {
            if(interaction.ToString() == "None" || interaction.ToString() == "All") continue;
            if(allowedInteractions.HasFlag(interaction))
            {
                animations.Add(interaction.ToString(), Animator.StringToHash(interaction.ToString()));
            }
        }

        
    }

    private void OnEnable()
    {
        MatchInputManager.Inputs.Character.Interact.started += TriggerInteract;
    }

    private void OnDisable()
    {
        MatchInputManager.Inputs.Character.Interact.started -= TriggerInteract;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if(interactable != null) currentInteractable = interactable;

    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if(interactable != null && interactable == currentInteractable) currentInteractable = null;

    }


    private void TriggerInteract(InputAction.CallbackContext ctx)
    {
        if(interactAnimation == AnimationState.Finished && currentInteractable != null)
            TriggerAnimation();
    }

    private void TriggerAnimation()
    {
        currentAnimation = currentInteractable.InteractType.ToString();
        animator.SetTrigger(currentAnimation);

        StartCoroutine(FadeInAnimation_Coro(currentInteractable.RequiresTwoHands));

        interactAnimation = AnimationState.InProgress;
    }


    #region Animation Events

        public void TriggerInteractable()
        {
            currentInteractable.Interact();
        }
        
        private IEnumerator FadeInAnimation_Coro(bool bothHands)
        {
            float animationLength = animationFramerate;
            float frameCounter = animationLength;
            float adjustmentAmount;

            while(frameCounter >= 0)
            {
                frameCounter--;
                
                adjustmentAmount = (float)(frameCounter / animationLength);
                rightHandConstraint.weight = adjustmentAmount;
                if(bothHands) leftHandConstraint.weight = adjustmentAmount;
                
                yield return new WaitForSeconds(1/animationFramerate);
            }

            interactAnimation = AnimationState.Finished;
        }

        // Called from animation events
        public void FadeOutAnimation() => StartCoroutine(FadeOutAnimation_Coro());

        private IEnumerator FadeOutAnimation_Coro()
        {
            float animationLength = Convert.ToInt32(animationFramerate * fadeOutAnimationSpeed);
            float frameCounter = 0;
            float adjustmentAmount;

            while(frameCounter <= animationLength)
            {
                frameCounter++;
                
                adjustmentAmount = (float)(frameCounter / animationLength);
                if(rightHandConstraint.weight < 1) rightHandConstraint.weight = adjustmentAmount;
                if(leftHandConstraint.weight < 1) leftHandConstraint.weight = adjustmentAmount;
                
                yield return new WaitForSeconds(1/animationFramerate);
            }

            interactAnimation = AnimationState.Finished;
        }

    #endregion

    }

}
