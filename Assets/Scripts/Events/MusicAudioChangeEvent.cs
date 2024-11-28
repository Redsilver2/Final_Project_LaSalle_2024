using Redsilver2.Core.Audio;
using UnityEngine;

namespace Redsilver2.Core.Events
{
    [CreateAssetMenu(menuName = "Events/Music Change", fileName = "New Music Changed Event")]
    public class MusicAudioChangeEvent : GameEvent
    {
        [SerializeField] private float lerpDuration;
        [SerializeField] private AudioClip clip;

        public override void Execute()
        {
            AudioManager audioManager = AudioManager.Instance;

            if (audioManager != null) 
            {
                audioManager.LerpMusicSources(lerpDuration, clip);
            }
        }
    }
}
