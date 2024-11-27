using System.Collections;
using System.Threading;
using UnityEngine;

namespace Redsilver2.Core.Counters
{
    public class Timer : Counter
    {
        public Timer() : base()
        {
            AddOnStartedEvent(() =>
            {
                timeElapsed = 0f;
            });
        }

        public void Start()
        {
            if (!isTimeElapsing)
            {
                onStarted.Invoke();
            }
        }

        public void Start(float startingTimeElapsed)
        {
            if (!isTimeElapsing)
            {
                Start();
                timeElapsed = startingTimeElapsed;
            }
        }

        protected override async Awaitable Update()
        {
            while (timeElapsed < Mathf.Infinity && isTimeElapsing)
            {
                if (!isPaused && !PauseManager.IsGamePaused)
                {
                    timeElapsed += Time.deltaTime;
                    onValueChanged.Invoke(timeElapsed);
                }

                await Awaitable.NextFrameAsync();
            }
        }

        public static IEnumerator WaitTimer(Timer timer, float timeToReach)
        {
            if(timer != null)
            {
                while (timer.TimeElapsed < timeToReach)
                {
                    yield return null;
                }
            }
        }
    }
}
