using Redsilver2.Core.Stats;
using Redsilver2.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redsilver2.Core.Interactables;

namespace Redsilver2.Core.Generator
{
    [RequireComponent(typeof(Health))]
    public class Generator : Lookable
    {
        [SerializeField] private Transform handle;

        [Space]
        [SerializeField] private MeshRenderer powerOnRenderer;
        [SerializeField] private MeshRenderer powerOffRenderer;

        [Space]
        [SerializeField] private Material defaultPowerOnMaterial;
        [SerializeField] private Material powerOnMaterial;

        [Space]
        [SerializeField] private Material defaultPowerOffMaterial;
        [SerializeField] private Material powerOffMaterial;

        [Space]
        [SerializeField] private Light powerOnLight;
        [SerializeField] private Light powerOffLight;

        [Space]
        [SerializeField] private AudioSource generatorSource;


        [Space]
        [SerializeField] private AudioClip powerOff;
        [SerializeField] private AudioClip powerOn;

        [Space]
        [SerializeField] private float damagePerSeconds = 0.05f;

        [Space(4f)]
        [SerializeField] private Image fillBar;

        [Space]
        [SerializeField] private float minFillBarFadeDistance;
        [SerializeField] private float maxFillBarFadeDistance;

        [Space]
        [SerializeField] private float minDelayLightsFlickers;
        [SerializeField] private float maxDelayLightsFlickers;


        public Health Durability => durability;
        private float maxHandleRotationClamp = 49f;

        private Color targetedFillBarColor; 
        private GeneratorCondition condition = GeneratorCondition.Danger;
       
        private IEnumerator conditionCoroutine;
        private IEnumerator damageCoroutine;


        private Health    durability;
        private Transform playerTransform;
        private bool      isActivated     = true;

        private float handleLocalRotationW;
        private Collider collider;


        private List<GeneratorLight> generatorLights = new List<GeneratorLight>();
        public static Generator Instance { get; private set; }

        protected override void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            base.Awake();
          
            collider   = GetComponent<Collider>();
            durability = GetComponent<Health>();

            playerTransform        = PlayerController.Instance.transform;
            maxHandleRotationClamp = 0.293232471f;
            handleLocalRotationW   = handle.localRotation.w;

            AddOnInteractEvent(isInteracting =>
            {
                collider.enabled = !isInteracting;
            });
        }

        public void Activate()
        {
            PlaySound(powerOn);
            SetPowerMaterial(true);
            StartTakingDamage();
        }

        public void Desactivate()
        {
            PlaySound(powerOff);
            SetPowerMaterial(false);
            StopTakingDamage();
        }



        private void Start()
        {
            SetGeneratorCondition(GeneratorCondition.Healthy);

            if (durability != null)
            {
                durability.AddOnValueChangedEvent(value =>
                {
                    if (isActivated)
                    {
                        float percentage = durability.PercentageValue;

                        if (percentage <= 1f && percentage > 0.8f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Healthy);
                        }
                        else if (percentage <= 0.8f && percentage > 0.4f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Caution);
                        }
                        else if (percentage <= 0.4f && percentage > 0f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Danger);
                        }
                        else if (percentage <= 0f)
                        {
                            PlaySound(powerOff);
                            SetPowerMaterial(false);

                            SetAllLightsVisibility(false);
                            isActivated = false;
                        }
                    }

                    UpdateFillBar(targetedFillBarColor);
                });

                if(handle != null)
                {
                    durability.AddOnValueChangedEvent(value =>
                    {
                        float rotationY;
                        float percentage = durability.PercentageValue;

                        if (percentage >= 1f)
                        {
                            rotationY = maxHandleRotationClamp;
                        }
                        else if (percentage <= 0f)
                        {
                            rotationY = -maxHandleRotationClamp;
                        }
                        else
                        {
                            rotationY = Mathf.Lerp(-maxHandleRotationClamp, maxHandleRotationClamp, percentage);
                        }

                        // Quaternion(-0.64343977,-0.293232471,-0.293232471,0.64343977)
                        handle.localRotation = new Quaternion(-0.64343977f, rotationY, 0f, handleLocalRotationW);
                    });
                }
            }
        }

        private void SetPowerMaterial(bool isPowerOn)
        {
            Material mat = null;

            if (powerOnRenderer != null)
            {
                mat = isPowerOn ? powerOnMaterial : defaultPowerOnMaterial;

                if (mat != null)
                {
                    powerOnRenderer.material = mat;
                }

                if (powerOnLight != null)
                {
                    powerOnLight.enabled = isPowerOn;
                }
            }

            if(powerOffRenderer != null)
            {
                mat = isPowerOn ? defaultPowerOffMaterial : powerOffMaterial;

                if (mat != null)
                {
                    powerOffRenderer.material = mat;
                }

                if (powerOffLight != null)
                {
                    powerOffLight.enabled = !isPowerOn;
                }
            }
        }
        private void PlaySound(AudioClip clip)
        {
            if (generatorSource != null)
            {
                generatorSource.clip = clip;
                generatorSource.Play();
            }
        }

        private void SetGeneratorCondition(GeneratorCondition condition)
        {
            if(this.condition != condition)
            {
                if(conditionCoroutine != null)
                {
                    StopCoroutine(conditionCoroutine);
                    conditionCoroutine = null;
                }

                this.condition = condition;

                switch (condition)
                {
                    case GeneratorCondition.Healthy:
                        targetedFillBarColor = Color.green;
                        break;

                    case GeneratorCondition.Caution:
                        targetedFillBarColor = Color.yellow;
                        conditionCoroutine = CautionConditionCoroutine();
                        break;

                    case GeneratorCondition.Danger:
                        targetedFillBarColor = Color.red;
                        break;
                }

                if(conditionCoroutine != null)
                {
                    StartCoroutine(conditionCoroutine);
                }

            }
        }
        private void UpdateFillBar(Color targetedFillBarColor)
        {
            PlayerController player = PlayerController.Instance;

            if (fillBar != null)
            {
                float distance     = Vector3.Distance(player.transform.position, transform.position);
                float currentAlpha = 0f;

                if (distance > minFillBarFadeDistance && distance < maxFillBarFadeDistance)
                {
                    currentAlpha = Mathf.Lerp(1f, 0f, distance / maxFillBarFadeDistance);
                }
                else if(distance <= minFillBarFadeDistance)
                {
                    currentAlpha = Mathf.Lerp(fillBar.color.a, 1f, Time.deltaTime);
                }
                else if(distance >= maxFillBarFadeDistance)
                {
                    currentAlpha = Mathf.Lerp(fillBar.color.a, 0f, Time.deltaTime);
                }

                Color newColor = new Color(targetedFillBarColor.r, targetedFillBarColor.g, targetedFillBarColor.b, currentAlpha);
                fillBar.color = Color.Lerp(fillBar.color, newColor, Time.deltaTime);
            }
        }

        private void FlickerLightsByDistanceToPlayer(float maxDistance)
        {
            foreach(GeneratorLight generatorLight in generatorLights)
            {
                Transform transform = generatorLight.transform;
                float distance      = Vector3.Distance(playerTransform.position, transform.position);

                if(distance <= maxDistance && !generatorLight.IsFlickering)
                {
                    generatorLight.Flicker(new float[]
                    {
                       1f, 0.5f, 1f, 0.5f, 1f,0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f
                    }, false);
                }
            }
        }
        private void FlickerLightByDistanceToPlayer(float maxDistance)
        {
            for(int i = 0; i < 9999; i++)
            {
                GeneratorLight generatorLight = generatorLights[Random.Range(0, generatorLights.Count)];

                float distance = Vector3.Distance(playerTransform.position, transform.position);

                if (distance <= maxDistance && !generatorLight.IsFlickering)
                {
                    generatorLight.Flicker(new float[]
                    {
                       1f, 0.5f, 1f, 0.5f, 1f,0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f
                    }, false);
                    break;
                }
            }
        }

        private void SetAllLightsVisibility(bool isVisible)
        {
            foreach(GeneratorLight generatorLight in generatorLights)
            {
                generatorLight?.SetVisibility(isVisible);    
            }
        }

        private IEnumerator CautionConditionCoroutine()
        {
            float lightsFlickerWaitTime = Random.Range(minDelayLightsFlickers, maxDelayLightsFlickers);
            float lightsFlickerTime     = 0f;

            while (this.condition == GeneratorCondition.Caution)
            {
                if (!PauseManager.IsGamePaused)
                {
                    lightsFlickerTime += Time.deltaTime;
                }

                if(lightsFlickerTime > lightsFlickerWaitTime)
                {
                    Debug.Log("Flickering Random Light");
                    FlickerLightsByDistanceToPlayer(50f);

                    lightsFlickerTime     = 0f;
                    lightsFlickerWaitTime = Random.Range(minDelayLightsFlickers, maxDelayLightsFlickers);
                }


                yield return null;
            }
        }


        private void StartTakingDamage()
        {
            StopTakingDamage();
            damageCoroutine = DamageCoroutine();
            StartCoroutine(damageCoroutine);
        }

        private void StopTakingDamage()
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
        }

        private IEnumerator DamageCoroutine()
        {
            while (isActivated)
            {
                if (!PauseManager.IsGamePaused)
                {
                    durability.Damage(damagePerSeconds * Time.deltaTime);
                }

                yield return null;
            }
        }

        public void AddGeneratorLight(GeneratorLight generatorLight) 
        {
            if (!generatorLights.Contains(generatorLight) && generatorLight != null)
            {
                generatorLights?.Add(generatorLight);
            }
        }
        public void RemoveGeneratorLight(GeneratorLight generatorLight)
        {
            if (generatorLights.Contains(generatorLight) && generatorLight != null)
            {
                generatorLights?.Remove(generatorLight);
            }
        }
    }
}
