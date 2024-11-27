using Redsilver2.Core.Player;
using UnityEngine;

namespace Redsilver2.Core.Lights
{
    [System.Serializable]
    public class LightQualityLevelData
    {
        [HideInInspector] public string qualityName;

        [Space]
        [SerializeField] private float minCheckDistance;
        [SerializeField] private float maxCheckDistance;

        [Space]
        [SerializeField] private float bounceIntensity;


        [Space]
        [SerializeField] private UnityEngine.Rendering.LightShadowResolution shadowResolution;
        [SerializeField] private LightShadows lightShadows; 

        [Space]
        [SerializeField] private LightRenderMode renderMode;
        [SerializeField] private LightShadowCasterMode shadowCasterMode;

        [Space]
        [SerializeField] private bool isForcedVisible;
        [SerializeField] private bool enableSpotReflector;

        public float MinCheckDistance => minCheckDistance;
        public float MaxCheckDistance => maxCheckDistance;

        public LightQualityLevelData(string qualityName)
        {
            this.qualityName = qualityName;
            minCheckDistance = 0f;
            maxCheckDistance = 0f;
            bounceIntensity  = 0f;

            shadowResolution = UnityEngine.Rendering.LightShadowResolution.Low;

            renderMode = LightRenderMode.Auto;
            shadowCasterMode = LightShadowCasterMode.Default;

            isForcedVisible = false;
            enableSpotReflector = false;
        }

        public void SetLightQuality(Light light)
        {
            if (light != null)
            {
                light.bounceIntensity = bounceIntensity;
                light.shadows = lightShadows;
                light.shadowResolution = shadowResolution;
                light.renderMode = renderMode;
                light.lightShadowCasterMode = shadowCasterMode;
                light.enableSpotReflector = enableSpotReflector;
                light.forceVisible = isForcedVisible;
                light.enabled = true;
            }
        }

        public bool IsCloseToPlayer(float distance)
        {
            return distance >= minCheckDistance && distance < maxCheckDistance;
        }
    }
}
