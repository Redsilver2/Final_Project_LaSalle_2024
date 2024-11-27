using System.Collections;
using UnityEngine;
using Redsilver2.Core.Player;
using Redsilver2.Core.Counters;
using UnityEngine.Rendering;
using Redsilver2.Core.Lights;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(LensFlareComponentSRP))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(LightSystem))]
    [RequireComponent(typeof(Light))]

    [RequireComponent(typeof(AudioSource))]
    public class FlareBullet : Bullet
    {
        [SerializeField] private AudioClip flareNoisesClip;

        [Space]
        [SerializeField] private float lightFadeDuration;
        [SerializeField] private float defaultBulletLifeTime;

        private LensFlareComponentSRP flare;
        private Rigidbody bulletRigidbody;
        private Light light;

        private float defaultLightIntensity;
        private float defaultLensFlareIntensity;

        protected override void Awake()
        {
            base.Awake();

            flare = GetComponent<LensFlareComponentSRP>();
            light = GetComponent<Light>();
            bulletRigidbody = GetComponent<Rigidbody>();

            defaultLightIntensity = light.intensity;
            defaultLensFlareIntensity = flare.intensity;
            Physics.IgnoreCollision(GetComponent<Collider>(), PlayerController.Instance.GetComponent<Collider>(), true);
        }

        public override void Interact()
        {
            if(ownerRangedWeapon == null && TryGetFlareGunInInventory(out FlareGun flareGun))
            {
                Debug.LogWarning($"Found a flare gun in inventory");
                SetOwnerRangedWeapon(flareGun);

                StopBulletDrop();
                source.Stop();
                bulletRigidbody.useGravity = false;
                bulletRigidbody.mass = 0.001f;

                ownerRangedWeapon.AddAmmoInStash(1);
                ownerRangedWeapon.ReturnBulletToPool(this);
            }
        }
        private bool TryGetFlareGunInInventory(out FlareGun flareGun)
        {
            EquippableItem[] equippableItems = inventory.GetEquippableItems();
            flareGun = null;

            if (equippableItems != null) 
            {
                foreach (EquippableItem item in equippableItems)
                {
                    FlareGun _flareGun = (FlareGun)item;

                    if (_flareGun != null)
                    {
                        if (!_flareGun.IsStashFull())
                        {
                            flareGun = _flareGun;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public override void Init(RangedWeapon rangedWeapon)
        {
            base.Init(rangedWeapon);
            bulletRigidbody.useGravity = false;
            bulletRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        protected override IEnumerator BulletDropCoroutine()
        {
            if (light != null && source != null && flare != null)
            {
                StartFire();
                yield return Counter.WaitForSeconds(defaultBulletLifeTime);

                float t = 0f;
                float currentVolume = source.volume;
                float currentFlareIntensity = flare.intensity;
                float currentLightIntensity = light.intensity;

                Color currentLightColor = light.color;
                Color nextLightColor = new Color(currentLightColor.r, currentLightColor.g, currentLightColor.b, 0f);

                while (t < lightFadeDuration)
                {
                    light.color = Color.Lerp(currentLightColor, nextLightColor, t / lightFadeDuration);
                    light.intensity = Mathf.Lerp(currentLightIntensity, 2f, t / lightFadeDuration);
                    source.volume = Mathf.Lerp(currentVolume, 0f, t / lightFadeDuration);
                    flare.intensity = Mathf.Lerp(currentFlareIntensity, 0f, t / lightFadeDuration);

                    if (!PauseManager.IsGamePaused)
                    {
                        t += Time.deltaTime;
                    }

                    yield return null;
                }

                light.enabled = false;

                if (t > lightFadeDuration)
                {
                    light.color     = nextLightColor;
                    light.intensity = 2f;
                    flare.intensity = 0f;
                    source.volume   = 0f;
                    source.Stop();
                }
            }
        }

        private void StartFire()
        {
            ownerRangedWeapon = null;

            light.color = new Color(light.color.r, light.color.g, light.color.b, 1f);
            light.intensity = defaultLightIntensity;
            flare.intensity = defaultLensFlareIntensity;

            source.volume = 1f;
            source.clip = flareNoisesClip;
            source.Play();

            light.enabled = true;
            bulletRigidbody.useGravity = true;

            bulletRigidbody.AddForce(transform.forward * forwardVelocityPower +
                                     transform.up      * verticalVelocityPower);
        }

        //protected void OnCollisionEnter(Collision collision)
        //{
        //    if (bulletRigidbody != null)
        //    {
        //        bulletRigidbody.mass = 5f;
        //    }
        //}
    }
}
