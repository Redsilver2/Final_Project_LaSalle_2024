using Redsilver2.Core;
using UnityEngine;

namespace Redsilver2.Core
{
    [System.Serializable]
    public struct AudioController
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioData[] datas;

        public void PlayAudioData(string dataName)
        {
            for (int i = 0; i < datas.Length; i++) 
            {
                AudioData data = datas[i];

                if (data.CompareClipName(dataName))
                {
                    data.Play(source);
                }
            }
        }

        public float GetAudioDataClipLenght(string dataName)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                AudioData data = datas[i];

                if (data.CompareClipName(dataName))
                {
                    return data.GetClipLenght();
                }
            }

            return 0f;
        }

        public void Play(AudioClip clip)
        {
            Play(clip, false, 1f);
        }

        public void Play(AudioClip clip, bool isLooping)
        {
            Play(clip, isLooping, 1f);
        }

        public void Play(AudioClip clip, bool isLooping, float minPitch, float maxPitch)
        {
            Play(clip, isLooping, Random.Range(minPitch, maxPitch));
        }

        public void Play(AudioClip clip, bool isLooping, float currentPitch)
        {
            if (source != null && clip != null) 
            {
                source.pitch = currentPitch;
                source.loop  = isLooping;
                source.clip  = clip;
                source.Play();
            }
        }
    }
}