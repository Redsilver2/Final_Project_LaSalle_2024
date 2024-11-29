using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    public class Radio : Interactable
    {
        [SerializeField] private AudioClip[] clips;

        [Space]
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool playRandomClipOnAwake = true;
        [SerializeField] private bool muteOnAwake    = true;

        private AudioSource source;

        private IEnumerator radioSoundsCoroutine;

        protected override void Awake()
        {
            base.Awake();
            source = GetComponent<AudioSource>();
            source.spatialBlend = 1.0f;
            source.playOnAwake  = false;

            radioSoundsCoroutine = RadioSoundsCoroutine(playRandomClipOnAwake ? Random.Range(0, clips.Length) 
                                                                              : 1);
            StartCoroutine(radioSoundsCoroutine);

            AddOnInteractEvent(isInteracting =>
            {
                if (isInteracting)
                {
                    source.mute = true;
                }
                else
                {
                    source.mute = false;
                }
            });

            if (muteOnAwake) 
            {
                Interact(true);
            }
        }

        public override void Interact()
        {
            if (isInteractable)
            {
                base.Interact();
            }
        }

        private IEnumerator RadioSoundsCoroutine(int index)
        {
            while (true)
            {
                for (int i = index; i < clips.Length; i++)
                {
                    AudioClip clip = clips[i];

                    if (clip != null)
                    {
                        source.clip = clip;
                        source.Play();
                        yield return new WaitForSeconds(clip.length);
                    }
                }

                index = 0;
            }
        }
    }
}
