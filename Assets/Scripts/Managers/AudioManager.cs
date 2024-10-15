using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource01;
    [SerializeField] private AudioSource musicSource02;

    [Space]
    [SerializeField] private AudioClip defaultMusicClip;
    [SerializeField] private AudioClip chaseClip;

    private AudioSource mainMusicSource = null;

    private IEnumerator musicLerpCoroutine;

    private void Start()
    {
        mainMusicSource = musicSource01;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            LerpMusicSources(0f, defaultMusicClip);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LerpMusicSources(0f, chaseClip);
        }
    }

    private void LerpMusicSources(float time, AudioClip clip)
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

        if (clip == null)
        {
            return;
        }

        if (time <= 0f || time >= clip.length)
        {
            time = 0f;
        }

        if (musicLerpCoroutine != null)
        {
            StopCoroutine(musicLerpCoroutine);
        }

        if (nextMainSource != null)
        {
            if (nextMainSource.clip != clip)
            {
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
        float t = 0f;

        if (mainSource != null && muteSource != null)
        {
            float originalMuteSourceVolume = muteSource.volume;
            float originalMainSourceVolume = mainSource.volume;

            while (t < duration)
            {
                float progress = t / duration;
                muteSource.volume = Mathf.Lerp(originalMuteSourceVolume, 0f, progress);
                mainSource.volume = Mathf.Lerp(originalMainSourceVolume, 1f, progress);

                t += Time.deltaTime;
                yield return null;
            }

            muteSource.volume = 0f;
            mainSource.volume = 1f;
        }

    }
}
