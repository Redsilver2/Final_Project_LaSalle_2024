using Redsilver2.Core.Audio;
using Redsilver2.Core.Enemy;
using UnityEngine;

namespace Redsilver2.Core.Events
{
    [CreateAssetMenu(menuName = "Events/Music Change", fileName = "New Music Changed Event")]
    public class MusicAudioChangeEvent : GameEvent
    {
        [SerializeField] private AudioClip clip;

        public override void Execute()
        {
            AudioManager audioManager = AudioManager.Instance;

            if (audioManager != null) 
            {
                audioManager.SetMainMusicClip(clip);

                if (EnemyStateController.HighestEnemyState < EnemyState.Search)
                {
                    audioManager.LerpMusicSources(0.0f, clip);
                }
            }
        }
    }
}
