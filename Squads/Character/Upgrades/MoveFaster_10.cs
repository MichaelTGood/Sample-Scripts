using UnityEngine;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;

namespace Squads.CharacterElements
{
	[System.Serializable]
	public class MoveFaster_10 : Upgrade
	{
		#region Variables

            [SerializeField, ReadOnly] private float locomotionMultiplier = 1.1f;

			[TextArea, ReadOnly]
			[SerializeField] private string description = "Multiplies movement, adding 10%. It does this by setting the animator float \"LocomotionMultiplier\" to 1.1f";

		#endregion

		[OnSerializing]
		public override void SetDefaultValues()
		{
			locomotionMultiplier = 1.1f;
			description = "Multiplies movement, adding 10%. It does this by setting the animator float \"LocomotionMultiplier\" to 1.1f";
		}

		public override void ExecuteAtStartOfMatch(CharacterPrefab character)
		{
			Debug.Log($"Character: {character.name} | Multiplier: {locomotionMultiplier}");

			var animator = character.Animator;

			int anim_locoMultiplier = Animator.StringToHash("LocomotionMultiplier");

            animator.SetFloat(anim_locoMultiplier, locomotionMultiplier);

		}


	}
}

