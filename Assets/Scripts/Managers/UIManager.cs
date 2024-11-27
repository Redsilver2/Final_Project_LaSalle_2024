using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.UI
{
    public class UIManager { 
        public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float duration, bool isVisible)
        {
            if (canvasGroup != null)
            {
                Timer timer        = new Timer();
                float targetAlpha  = isVisible ? 1f : 0f; 
                float currentAlpha = canvasGroup.alpha;

                timer.AddOnValueChangedEvent(value =>
                {
                    if (value >= duration)
                    {
                        timer.Stop();
                    }
                    else
                    {
                        canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, value /duration);
                    }
                });
                timer.AddOnStoppedEvent(() =>
                {
                    if(timer.TimeElapsed >= duration)
                    {
                       canvasGroup.alpha = targetAlpha;
                    }
                });
                timer.Start();

                while (timer.IsTimeElapsing)
                {
                    yield return null;
                }
            }
        }
    }

}
