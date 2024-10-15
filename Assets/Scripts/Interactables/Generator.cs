using Redsilver2.Core.Stats;
using Redsilver2.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Generator
{
    [RequireComponent(typeof(Health))]
    public class Generator : MonoBehaviour
    {
        [SerializeField] private Transform handle;
        [SerializeField] private float maxHandleRotationClamp = 49f;

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
        [SerializeField] Health durability;
        [SerializeField] private float baseDurabilityDPS = 0.05f;

        [Space(4f)]
        [SerializeField] private Image fillBar;

        [Space]
        [SerializeField] private float minFillBarFadeDistance;
        [SerializeField] private float maxFillBarFadeDistance;

        [Space]
        [SerializeField] private float minDelayLightsFlickers;
        [SerializeField] private float maxDelayLightsFlickers;

        private Color targetedFillBarColor; 
        private GeneratorCondition condition = GeneratorCondition.Danger;
       
        private IEnumerator conditionCoroutine;
        private IEnumerator damageCoroutine;

        private bool  isActivated     = true;


        private List<GeneratorLight> generatorLights = new List<GeneratorLight>();
        public static Generator Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {

            durability = GetComponent<Health>();


            StartTakeDamage();
            SetPowerMaterial(true);
            PlaySound(powerOn);
            SetGeneratorCondition(GeneratorCondition.Healthy);

            if (durability != null)
            {
                durability.AddOnValueChangedEvent((handler, isIncreasingValue) =>
                {
                    if (isActivated)
                    {
                        float value = handler.PercentageValue;

                        if (value <= 1f && value > 0.8f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Healthy);
                        }
                        else if (value <= 0.8f && value > 0.4f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Caution);
                        }
                        else if (value <= 0.4f && value > 0f)
                        {
                            SetGeneratorCondition(GeneratorCondition.Danger);
                        }
                        else if (value <= 0f)
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
                    durability.AddOnValueChangedEvent((handler, isIncreasingValue) =>
                    {
                        float rotationY;
                        float value = handler.PercentageValue;

                        if (value >= 1f)
                        {
                            rotationY = maxHandleRotationClamp;
                        }
                        else if (value <= 0f)
                        {
                            rotationY = -maxHandleRotationClamp;
                        }
                        else
                        {
                            rotationY = Mathf.Lerp(-maxHandleRotationClamp, maxHandleRotationClamp, value);
                        }

                        handle.localEulerAngles = Vector3.right * -90 +  Vector3.forward * rotationY;
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
            Transform playerTransform = PlayerController.Instance.transform;

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
            Transform playerTransform = PlayerController.Instance.transform;

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
                lightsFlickerTime += Time.deltaTime;

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


        private void StartTakeDamage()
        {
            StopTakeDamage();
            damageCoroutine = DamageCoroutine();
            StartCoroutine(damageCoroutine);
        }

        private void StopTakeDamage()
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
                durability.Damage(baseDurabilityDPS * Time.deltaTime);
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
