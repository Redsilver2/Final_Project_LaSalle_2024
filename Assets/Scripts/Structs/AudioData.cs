using UnityEngine;

namespace Redsilver2.Core
{
    [System.Serializable]
    public struct AudioData
    {
        [SerializeField] private AudioClip   clip;
        [SerializeField] private AudioSource source;

        [Space]
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        [Space]
        [SerializeField] private bool canRandomizePitch;
        [SerializeField] private bool isLooping;

        public void Play(AudioSource source)
        {
            float currentPitch = canRandomizePitch ? Random.Range(minPitch, maxPitch) : 1f;

            if(source != null && clip != null)
            {
                source.pitch = currentPitch;
                source.clip  = clip;
                source.loop  = isLooping;
                source.Play();
            }
        }

        public bool CompareClipName(string clipName) 
        { 
            if(clipName != string.Empty || clip != null)
            {
                return false;
            }

            return clip.name.ToLower().Contains(clipName.ToLower()); 
        }

        public float GetClipLenght()
        {
            if(clip == null)
            {
                return 0f;
            }

            return clip.length;
        }
    }
}
