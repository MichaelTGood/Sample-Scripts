using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.CharacterElements
{
    public class CharacterPrefab : MonoBehaviour
    {
        #region Variables

            [SerializeField] private GameObject characterObj;

            [SerializeField] private SkinnedMeshRenderer meshRenderer;

            [SerializeField] private Character character;

            [SerializeField] private Rigidbody rb;
            
            [SerializeField] private Animator animator;

            public Character Character { get => character;  }
            public GameObject CharacterObj { get => characterObj; }
            public Animator Animator { get => animator; }


		#endregion

		private void Awake()
        {
            if(characterObj == null) characterObj = GetComponentInChildren<Character>().gameObject;

            if(character == null) character = characterObj.GetComponent<Character>();

            if(rb == null) rb = characterObj.GetComponent<Rigidbody>();

            if(animator == null) animator = characterObj.GetComponent<Animator>();
        }


	}
}
