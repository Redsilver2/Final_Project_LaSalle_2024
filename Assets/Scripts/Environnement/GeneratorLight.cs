using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redsilver2.Core.Generator
{
    public class GeneratorLight : MonoBehaviour
    {
        [SerializeField] private Light _light;

        private bool isFlickering;
        private float flickeringTimeElapsed;

        private IEnumerator[] iterators;
        private AudioSource source;

        public float FlickerTimeElpased => flickeringTimeElapsed; 
        public bool IsFlickering => isFlickering;
        public AudioSource Source => source;

        private void Start()
        {
            Generator generator = Generator.Instance;
            generator.AddGeneratorLight(this);

            isFlickering          = false;
            flickeringTimeElapsed = 0;
           
            iterators             = new IEnumerator[2];
            iterators[0]          = FlickerCounterCoroutine();

        }


        public void Flicker(float[] intervals, bool finalVisibility)
        {
            StopIterators();
            iterators[1] = FlickerCoroutine(intervals, finalVisibility);

            isFlickering = true;

            foreach (IEnumerator iterator in iterators)
            {
                if (iterator != null)
                {
                   StartCoroutine(iterator);
                }
            }
        }
        public void SetSource(AudioSource source, AudioClip flickeringClip, AudioClip buzzingClip)
        {
            this.source = source;

            if (source != null)
            {
                Transform transform = source.transform;

                source?.Stop();
                transform.parent = transform;
                transform.localPosition = Vector3.zero;

                if (isFlickering)
                {
                    source.clip = flickeringClip;
                    source.time = flickeringTimeElapsed;
                }
                else
                {
                    float startingPoint = Random.Range(0f, buzzingClip.length);
                    source.clip = buzzingClip;
                    source.time = startingPoint;
                }

                source?.Play();
            }
        }

        public void SetVisibility(bool isVisible)
        {
            StopIterators();

            if (_light != null)
            {
                _light.enabled = isVisible;
            }
        }
        private void StopIterators()
        {
            foreach (IEnumerator iterator in iterators)
            {
                if (iterator != null)
                {
                   StopCoroutine(iterator);
                }
            }

            isFlickering = false;
            flickeringTimeElapsed = 0f;
        }

        private IEnumerator FlickerCounterCoroutine()
        {
            while (isFlickering)
            {
                flickeringTimeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        private IEnumerator FlickerCoroutine(float[] intervals, bool finalVisibility)
        {
            if (_light != null)
            {
                bool isDoneLooping          = false;
                bool currentLightVisibility = finalVisibility;

                for (int i = 0; i < intervals.Length; i++)
                {
                    currentLightVisibility = !currentLightVisibility;
                    _light.enabled = currentLightVisibility;

                    if (i == intervals.Length - 1)
                    {
                        isDoneLooping = true;
                        Debug.Log(i);
                    }
                    else
                    {
                        yield return new WaitForSeconds(intervals[i]);
                    }
                }

                if (_light.enabled != finalVisibility && isDoneLooping)
                {
                    yield return new WaitForSeconds(intervals[intervals.Length - 1]);
                    _light.enabled = finalVisibility;
                    StopAllCoroutines();
                }
            }
        }

        private void OnDisable()
        {
            StopIterators();
        }
    }
}

