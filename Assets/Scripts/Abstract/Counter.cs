using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Counters
{
    public abstract class Counter
    {
        protected float timeElapsed;
        protected bool  isPaused;
        protected bool  isTimeElapsing;

        protected UnityEvent        onStarted;
        private   UnityEvent        onStopped;

        private   UnityEvent<bool>  onResumed;
        protected UnityEvent<float> onValueChanged;

        public float TimeElapsed => timeElapsed;
        public bool  IsPaused => isPaused;
        public bool  IsTimeElapsing => isTimeElapsing;

        public Counter() 
        {
            onStarted          = new UnityEvent();
            onStopped          = new UnityEvent();

            onResumed          = new UnityEvent<bool>();
            onValueChanged     = new UnityEvent<float>();

            isPaused       = false;
            isTimeElapsing = false;

            onStarted.AddListener(async () =>
            {
                 isTimeElapsing = true;
                 isPaused       = false;
                 await Update();
            });

            onResumed.AddListener(isResuming => 
            {
                isPaused = !isResuming;
            });

            onStopped.AddListener(() =>
            {
                isTimeElapsing = false;
                isPaused       = false;
            });
        }

        public void Resume()
        {
            if (isPaused && isTimeElapsing)
            {
                onResumed.Invoke(true);
            }
        }
        public void Pause()
        {
            if(!isPaused && isTimeElapsing)
            {
                onResumed.Invoke(false);
            }
        }
        public void Stop()
        {
            if (isTimeElapsing)
            {
                onStopped.Invoke();
            }
        }

        protected abstract Awaitable Update();

        public void AddOnStartedEvent(UnityAction action)
        {
            onStarted.AddListener(action);
        }
        public void AddOnStoppedEvent(UnityAction action)
        {
            onStopped.AddListener(action);
        }
        public void AddOnResumedEvent(UnityAction<bool> action)
        {
            onResumed.AddListener(action);
        }
        public void AddOnValueChangedEvent(UnityAction<float> action)
        {
            onValueChanged.AddListener(action);
        }

        public void RemoveOnStartedEvent(UnityAction action)
        {
            onStarted.RemoveListener(action);
        }
        public void RemoveOnStoppedEvent(UnityAction action)
        {
            onStopped.RemoveListener(action);
        }
        public void RemoveOnResumedEvent(UnityAction<bool> action)
        {
            onResumed.AddListener(action);
        }
        public void RemoveOnValueChangedEvent(UnityAction<float> action)
        {
            onValueChanged.RemoveListener(action);
        }

        public static IEnumerator WaitForSeconds(float waitTime)
        {
            float t = 0f;

            while (t < waitTime)
            {
                if (!PauseManager.IsGamePaused)
                {
                    t += Time.deltaTime;
                }

                yield return null;
            }
            
        }

        public static IEnumerator WaitForSeconds(float waitTime, UnityAction<float> onValueChanged, UnityAction onCounterFinished)
        {
            float t = 0f;

            while (t < waitTime)
            {
                if (!PauseManager.IsGamePaused)
                {
                    if (onValueChanged != null)
                    {
                        onValueChanged.Invoke(t / waitTime);
                    }

                    t += Time.deltaTime;
                }

                yield return null;
            }

            if (t >= waitTime && onCounterFinished != null)
            {
                onCounterFinished.Invoke();
            }


        }
    }
}
