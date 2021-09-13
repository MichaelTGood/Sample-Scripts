using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Squads.Weapons
{
	public class Tracer : Poolable
	{
		#region Variables

            [SerializeField] private SpriteRenderer spriteRenderer;

			private Vector3 startingLocation;
			public override Vector3 StartingLocation { set => startingLocation = value; }

			private Vector3 destination;
			public Vector3 SetDestination { set => destination = value; }

			private float travelTime = 0;

			private bool isActive;

            public override event Action<Poolable> Done;

		#endregion

		private void Awake()
		{
			if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if(!isActive) return;

			transform.position = Vector3.Lerp(startingLocation, destination, travelTime);
			travelTime += Time.deltaTime;

			if(travelTime >= 1) IsDone();
			
		}
        

		public override void Activate()
		{
			spriteRenderer.enabled = true;
			isActive = true;			
        }

		public override void Prepare()
		{
			isActive = false;
            spriteRenderer.enabled = false;
			travelTime = 0;
			transform.position = startingLocation;
        }

		public override void Prepare(Vector3 newStartingLocation)
		{
			startingLocation = newStartingLocation;
			Prepare();
		}

		public override void IsDone()
		{
			isActive = false;
			Done?.Invoke(this);
		}
	}
}
