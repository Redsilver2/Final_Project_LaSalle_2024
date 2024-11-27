using Redsilver2.Core.Counters;
using Redsilver2.Core.Events;
using Redsilver2.Core.Player;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Audio
{
    public abstract class AudioSystem : GameObjectEvents
    {
        [SerializeField] private AudioType audioType;

        [Space]
        [SerializeField][Range(1f, 20f)] private float maxCheckDistance;

        [Space]
        [SerializeField][Range(0.1f, 20f)] private float minPlayDistance;
        [SerializeField][Range(0.1f, 20f)] private float maxPlayDistance;

        [Space]
        [SerializeField] private bool is3DAudio    = true;
        [SerializeField] private bool isLooping    = false;
        [SerializeField] private bool canStopMusic = true;

        private bool isActive = false;
        private float timeElapse = 0f;

        private Transform    playerTransform = null;
        private AudioSource  source          = null;
        private AudioClip    currentClip     = null;
        private AudioManager audioManager    = null;

        private float distanceToPlayer;
        public float DistanceToPlayer => distanceToPlayer;
        public float MaxCheckDistance => maxCheckDistance;

        protected override void Awake()
        {
            base.Awake();

            playerTransform = PlayerController.Instance.transform;
            audioManager    = AudioManager.Instance;

            audioManager.AddAudioSystem(this);
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleLevelEvent);

            AddOnStateChangedEvent(isEnabled =>
            {
                if (audioManager != null)
                {
                    if (isEnabled)
                    {
                        audioManager.AddAudioSystem(this);
                    }
                    else
                    {
                        if (!SceneLoaderManager.IsLoadingSingleScene)
                        {
                            if (source != null)
                            {
                                audioManager.RemoveAudioSystem(this);
                            }

                            audioManager.ReturnWorldSource(audioType, source);
                        }
                    }
                }
            });
        }

        private IEnumerator ReturnSourceToWorldCoroutine(float waitTime)
        {
            if (source != null)
            {
                yield return Counter.WaitForSeconds(waitTime);
                audioManager.RemoveAudioSystem(this);
                audioManager.ReturnWorldSource(audioType, source);
            }
        }

        private void OnLoadSingleLevelEvent(int levelIndex) 
        {
            StartCoroutine(ReturnSourceToWorldCoroutine(SceneLoaderManager.Instance.LoadingScreenAlphaLerpDuration - 0.1f));
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleLevelEvent);
        }
 
        public void Set(bool isActive)
        {
            SetIsActive(isActive);
        }


        public bool CheckDistance()
        {
            if(playerTransform != null)
            {
                distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

                if(distanceToPlayer >= 0f && distanceToPlayer <= maxCheckDistance)
                {
                    return true;
                }
            }
            else
            {
                distanceToPlayer = Mathf.Infinity;
            }

            return false;
        }

        public void SetCurrentClip(AudioClip currentClip)
        {
            this.currentClip = currentClip;
        }
        public void Play(AudioClip clip)
        {
            Play(timeElapse, clip);
        }
     
        public void Play(float time, AudioClip clip)
        {
            if (clip != null && source != null)
            {
                if (time < 0f || time > clip.length || clip != currentClip)
                {
                    time = 0f;
                }

                source.Stop();

                source.spatialBlend = is3DAudio ? 1f : 0f;
                source.minDistance = minPlayDistance;
                source.maxDistance = maxPlayDistance;

                source.loop = true;
                source.clip = clip;
                source.time = time;

                timeElapse = time; 
                source.Play();
            }
        }
        private void SetIsActive(bool isActive)
        {
            if (this.isActive != isActive)
            {
                this.isActive = isActive;

                if (isActive)
                {
                    this.source = audioManager.GetAvailableWorldSource(audioType);

                    if (source != null)
                    {
                        Transform transform = source.transform;

                        source.maxDistance = maxPlayDistance;
                        source.minDistance = minPlayDistance;
                        source.loop = isLooping;

                        transform.SetParent(this.transform, false);
                        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                        Play(currentClip);
                    }
                    else
                    {
                        this.isActive = false;
                    }
                }
                else
                {
                    if (source != null)
                    {
                        audioManager.ReturnWorldSource(audioType, source);
                        timeElapse = source.time;
                    }

                    source = null;
                }
            }
        }
    }
}
