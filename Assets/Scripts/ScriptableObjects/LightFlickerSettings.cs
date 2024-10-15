using UnityEngine;

namespace Redsilver2.Core.Generator
{
    [CreateAssetMenu(fileName = "New Light Flicker Settings", menuName = "Generator/Light Flicker Settings")]
    public class LightFlickerSettings : ScriptableObject
    {
        [SerializeField] private float[] intervals;
        [SerializeField] private AudioClip clip;

        public float[] Intervals => intervals;
        public AudioClip Clip => clip;
    }
}