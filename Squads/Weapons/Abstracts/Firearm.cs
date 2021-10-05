using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kkachi;

namespace Squads.Weapons
{
    public abstract class Firearm : Weapon
    {
        #region Variables

            [Header("Firearm Options")]
            [SerializeField] private bool canAutoFire;
            [Range(1,25)]
            [SerializeField] private int shotsPerSecond = 1;
            [Range(0.001f, 0.1f)]
            [SerializeField] private float muzzleFlashLightSpeed = 0.05f;

            [Header("Physical Points")]
            [SerializeField] private Transform gripTrigger;
            [SerializeField] private Transform gripSupport;
            [SerializeField] private Transform barrelTip;
            [SerializeField] private Transform raycastDestination;

            [Header("Fire Effects")]
            [SerializeField] private ParticleSystem muzzleFlashParticle;
            [SerializeField] private Light[] muzzleFlashLights;
            [SerializeField] private TrailRenderer tracer;

            [Header("Audio")]
            [SerializeField] private AudioSource audioSource;
            [SerializeField] private AudioClip shotSound;

            [Header("Hit Effects")]
            [SerializeField] private float hitDistanceCheck = 0.5f;
            [SerializeField] private ParticleSystem metalHitEffect;
            [SerializeField] private ParticleSystem fleshHitEffect;


            Ray ray;
            RaycastHit raycastHitInfo;
            ParticleSystem impact = new ParticleSystem();
            
            // Accessors
            public bool CanAutoFire { get => canAutoFire; }
            public int ShotsPerSecond { get => shotsPerSecond; }
            public Transform GripTrigger { get => gripTrigger; }
            public Transform GripSupport { get => gripSupport; }

		#endregion

		public virtual void Fire(Team team)
        {
            audioSource.Play();
            StartCoroutine(MuzzleFlash());

            ray.origin = barrelTip.position;
            ray.direction = raycastDestination.position - barrelTip.position;

            if(Physics.Raycast(ray, out raycastHitInfo))
            {
                IHitable hitObject = raycastHitInfo.transform.GetComponent<IHitable>();
                if(hitObject != null)
				{
					hitObject.Hit(damage, out ImpactMaterial impactMaterial, team);

                    SetImpactParticleEffect(raycastHitInfo, impactMaterial);
				}
				else
                {
                    SetImpactParticleEffect(raycastHitInfo);
                }


            }
            else // Hit nothing (i.e. empty space)
            {
                // Ensures that the tracer doesn't fire backwards.
                raycastHitInfo.point = ray.origin + (ray.direction * 100f); 
            }
            
            FireTracer();
        }

		private IEnumerator MuzzleFlash()
        {
            muzzleFlashParticle.Emit(1);
            foreach(var light in muzzleFlashLights)
            {
                light.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(muzzleFlashLightSpeed);

            foreach(var light in muzzleFlashLights)
            {
                light.gameObject.SetActive(false);
            }
        }

        private void FireTracer()
        {
            var tracerInstance = Instantiate(tracer, barrelTip.position, Quaternion.identity);
            tracerInstance.AddPosition(barrelTip.position);
            tracerInstance.transform.position = raycastHitInfo.point;

        }

		private void SetImpactParticleEffect(RaycastHit hitInfo, ImpactMaterial impactMaterial = ImpactMaterial.None)
		{
			switch (impactMaterial)
			{
				case ImpactMaterial.Metal:
					impact = metalHitEffect;
					break;
				case ImpactMaterial.Flesh:
					impact = fleshHitEffect;
					break;
				default:
					impact = metalHitEffect;
					break;
			}

			impact.transform.position = hitInfo.point;
			impact.transform.forward = hitInfo.normal;
			impact.Emit(1);
		}



    }



}
