using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Audio 
{
    public class FootstepAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private Footstep[] footsteps;

        [Space]
        [SerializeField] private float walkPitch = 1.0f;
        [SerializeField] private float runPitch = 1.0f;

        private  UnityEvent<Transform> onFootstepSoundPlayed = new UnityEvent<Transform>();

        private void OnValidate()
        {
            Footstep.SetArray(ref footsteps);
        }

        private void Start()
        {
            source.pitch = walkPitch;
        }

        public void SetPitch(bool isRunning, float lerpSpeed)
        {
            float desiredPitch = isRunning ? runPitch : walkPitch;
            source.pitch = Mathf.Lerp(source.pitch, desiredPitch, lerpSpeed * Time.deltaTime);
        }

        public void PlayFootstepSound(string groundTag, bool playGroundClip)
        {
            if (source != null && footsteps != null)
            {
                if (!playGroundClip)
                {
                    source?.Stop();
                    source.pitch = 1f;
                }

                if (!source.isPlaying)
                {
                    AudioClip clip = GetFootstepClip(groundTag, playGroundClip);

                    if (clip != null)
                    {
                        source.clip = clip;
                        source.Play();
                        onFootstepSoundPlayed.Invoke(transform);
                    }
                }
            }
        }

        private AudioClip GetFootstepClip(string groundTag, bool isGettingGroundClip)
        {
            AudioClip result = null;

            for (int i = 0; i < footsteps.Length; i++)
            {
                Footstep footstep = footsteps[i];

                if (groundTag.Contains(footstep.groundTag))
                {
                    result = footstep.GetUniqueRandomClip(isGettingGroundClip);
                    break;
                }
            }

            return result;
        }

        public void AddOnFootstepSoundPlayedEvent(UnityAction<Transform> action)
        {
            onFootstepSoundPlayed?.AddListener(action);
        }

        public void RemoveOnFootstepSoundPlayedEvent(UnityAction<Transform> action)
        {
            onFootstepSoundPlayed?.RemoveListener(action);
        }
    }
}
