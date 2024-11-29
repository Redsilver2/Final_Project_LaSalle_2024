using Redsilver2.Core.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Counters;

namespace Redsilver2.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource01;
        [SerializeField] private AudioSource musicSource02;


        [SerializeField] private AudioSource worldSourcePrefab;
        [SerializeField] private int maxWorldSourcesPoolSize = 5;

        private Dictionary<AudioType, List<AudioSource>> worldSourcesPool;
        private List<AudioSystem> audioSystems;

        private AudioSource mainMusicSource = null;
        private IEnumerator musicLerpCoroutine;

        private IEnumerator audioSystemUpdateCoroutine;

        private static CancellationTokenSource lerpListenerTokenSource;

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            mainMusicSource = musicSource01;
            worldSourcesPool = new Dictionary<AudioType, List<AudioSource>>();
            audioSystems = new List<AudioSystem>();

            SetWorlSourcesPool();
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleLevelEvent);
            SceneLoaderManager.AddOnSingleLevelLoadedEvent(levelIndex =>
            {
                if (musicLerpCoroutine != null)
                {
                    StopCoroutine(musicLerpCoroutine);
                }

                musicSource01.volume = 0f;
                musicSource02.volume = 0f;

                Debug.LogWarning("????");
            });
        }

        private void OnLoadSingleLevelEvent(int levelIndex)
        {     
            audioSystems.Clear();

            if (levelIndex != 0)
            {
                Debug.LogWarning("Enabling Audio Manager");

                if (audioSystemUpdateCoroutine == null)
                {
                    audioSystemUpdateCoroutine = AudioSystemsUpdate();
                    StartCoroutine(audioSystemUpdateCoroutine);
                }
            }
            else
            {
                Debug.LogWarning("Disabling Audio Manager");

                if (audioSystemUpdateCoroutine != null)
                {
                    StopCoroutine(audioSystemUpdateCoroutine);
                }
            }
        }

        private void OnSingleLevelLoadedEvent(int levelIndex)
        {
            if (musicLerpCoroutine != null)
            {
                StopCoroutine(musicLerpCoroutine);
            }

            musicSource01.volume = 0f;
            musicSource02.volume = 0f;

            Debug.LogWarning("????");
        }


        public void LerpMusicSources(float time, AudioClip clip)
        {
            AudioSource nextMainSource;
            AudioSource muteMainSource;


            if (mainMusicSource == musicSource01)
            {
                nextMainSource = musicSource02;
                muteMainSource = musicSource01;
            }
            else
            {
                nextMainSource = musicSource01;
                muteMainSource = musicSource02;
            }


            if (musicLerpCoroutine != null)
            {
                StopCoroutine(musicLerpCoroutine);
            }

            Debug.Log(nextMainSource);

            if (nextMainSource != null)
            {
                if (nextMainSource.clip != clip)
                {
                    if (time <= 0f || time >= clip.length)
                    {
                        time = 0f;
                    }

                    nextMainSource.clip = clip;
                    nextMainSource.time = time;
                    nextMainSource.Play();
                }

                mainMusicSource = nextMainSource;
                musicLerpCoroutine = LerpSourcesVolume(mainMusicSource, muteMainSource, 5f);
                StartCoroutine(musicLerpCoroutine);
            }
        }

        private IEnumerator LerpSourcesVolume(AudioSource mainSource, AudioSource muteSource, float duration)
        {
            if (mainSource != null && muteSource != null)
            {
                float originalMuteSourceVolume = muteSource.volume;
                float originalMainSourceVolume = mainSource.volume;

                yield return Counter.WaitForSeconds(duration, value =>
                {
                    muteSource.volume = Mathf.Lerp(originalMuteSourceVolume, 0f, value);
                    mainSource.volume = Mathf.Lerp(originalMainSourceVolume, 1f, value);

                }, 
                () =>
                {
                    muteSource.volume = 0f;
                    mainSource.volume = 1f;
                });
            }

        }

        public AudioSource GetAvailableWorldSource(AudioType audioType)
        {
            AudioSource result = null;

            if (worldSourcesPool.TryGetValue(audioType, out List<AudioSource> sources))
            {
                if (sources.Count > 0)
                {
                    result = sources[0];
                    sources.RemoveAt(0);
                }
            }

            return result;
        }
        public void ReturnWorldSource(AudioType audioType, AudioSource source)
        {
            if (worldSourcesPool.TryGetValue(audioType, out List<AudioSource> sources) && source != null && !sources.Contains(source))
            {
                Transform transform = source.transform;
                source.Stop();
                sources.Add(source);

                transform.SetParent(this.transform);
                transform.localPosition = Vector3.zero;
            }
        }

        public void AddAudioSystem(AudioSystem audioSystem)
        {
            if (audioSystem != null && !audioSystems.Contains(audioSystem))
            {
                audioSystems.Add(audioSystem);
            }
        }
        public void RemoveAudioSystem(AudioSystem audioSystem)
        {
            if (audioSystems.Contains(audioSystem))
            {
                audioSystems.Remove(audioSystem);
            }
        }

        private void SetWorlSourcesPool()
        {
            if (worldSourcePrefab != null)
            {
                AudioType[] audioTypes = (AudioType[])Enum.GetValues(typeof(AudioType));

                foreach (AudioType audioType in audioTypes)
                {
                    List<AudioSource> sources = new List<AudioSource>();

                    for (int i = 0; i < maxWorldSourcesPoolSize; i++)
                    {
                        AudioSource source     = Instantiate(worldSourcePrefab);
                        GameObject  gameObject = source.gameObject;
                        Transform   transform  = source.transform;

                        sources.Add(source);
                        transform.SetParent(this.transform);
                       
                        transform.localPosition = Vector3.zero;
                        gameObject.name = $"Audio Source ({audioType}) " + (i + 1);
                    }

                    worldSourcesPool.Add(audioType, sources);
                }
            }
        }

        private IEnumerator AudioSystemsUpdate()
        {
            AudioSystem[]  systems;

            while (true)
            {
                systems = GetClosestAudioSystemsToPlayer(30);
                DisableInactiveAudioSystems();

                PlayerController player = PlayerController.Instance;

                if (systems != null && player != null)
                {
                    foreach (AudioSystem audioSystem in systems)
                    {
                        if (audioSystem != null)
                        {
                            Debug.DrawLine(audioSystem.transform.position, player.transform.position, Color.red);
                            audioSystem.Set(true);
                        }
                    }
                }

                yield return Counter.WaitForSeconds(0.25f);
            }
        }
        private AudioSystem[] GetClosestAudioSystemsToPlayer(float distance)
        {
            if (audioSystems.Count > 0)
            {
                return audioSystems.FindAll(system => system.CheckDistance() && system.DistanceToPlayer <= distance).ToArray();
            }

            return null;
        }

        private void DisableInactiveAudioSystems()
        {
            if (audioSystems != null)
            {
                AudioSystem[] inactiveSystems = audioSystems.FindAll(system => !system.CheckDistance()).ToArray();
                Debug.Log("Inactive Audio Systems: " + inactiveSystems.Length);


                foreach (AudioSystem inactiveSystem in inactiveSystems)
                {
                    inactiveSystem.Set(false);
                }
            }

           
        }

        public static IEnumerator LerpAudioListenerVolume(bool isMuted, float duration)
        {
            float targetedVolume = isMuted ? 0f : 1f;
            float currentVolume = AudioListener.volume;

            yield return Counter.WaitForSeconds(duration, value =>
            {
                AudioListener.volume = Mathf.Lerp(currentVolume, targetedVolume, value);
            },
            () =>
            {
                AudioListener.volume = targetedVolume;
            });
        }


    }
}
