using UnityEngine;
using UnityEngine.Rendering;

namespace Redsilver2.Core.Lights
{
    [CreateAssetMenu(fileName = "New  Light Quality Level Settings", menuName = "Lights/Quality Level")]
    public class LightQualityLevelSettings : ScriptableObject
    {
        [SerializeField] private float minCheckDistance;
        [SerializeField] private float maxCheckDistance;

        [Space]
        [SerializeField] private LightShadows shadows;
        [SerializeField] private LightShadowResolution shadowResolution;

        public float MinCheckDistance => minCheckDistance;
        public float MaxCheckDistance => maxCheckDistance;

        public void SetLightSettings(Light light)
        {
            if(light != null)
            {
                light.shadowResolution = shadowResolution;
                light.shadows          = shadows;
            }
        }
    }
}
