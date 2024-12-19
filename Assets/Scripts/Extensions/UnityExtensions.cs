using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core {
    public static class UnityExtensions
    {
        public static IEnumerator LerpLocalRotation(this Transform transform, Quaternion desiredRotation, float duration)
        {
            Quaternion currentRotation = transform.localRotation;

            yield return Counter.WaitForSeconds(duration, 
            value =>
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, desiredRotation, value);
            },
            () =>
            {
                transform.localRotation = desiredRotation;
            });
        }

        public static IEnumerator LerpLocalPosition(this Transform transform, Vector3 desiredPosition, float duration)
        {
            Vector3 currentPosition = transform.localPosition;

            yield return Counter.WaitForSeconds(duration,
            value =>
            {
                transform.localPosition = Vector3.Lerp(currentPosition, desiredPosition, value);
            },
            () =>
            {
                transform.localPosition = desiredPosition;
            });
        }

        public static IEnumerator Fade(this CanvasRenderer renderer, bool isVisible, float duration)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            float currentAlpha = renderer.GetAlpha();

            yield return Counter.WaitForSeconds(duration, value =>
            {
                renderer.SetAlpha(Mathf.Lerp(currentAlpha, targetAlpha, value));
            },
            () =>
            {
                renderer.SetAlpha(targetAlpha);
            });
        }

        public static IEnumerator Fade(this CanvasGroup canvasGroup, bool isVisible, float duration)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            float currentAlpha = canvasGroup.alpha;

            yield return Counter.WaitForSeconds(duration, value =>
            {
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, value);
            },
            () =>
            {
                canvasGroup.alpha = targetAlpha;
            });
        }

        public static void StartCoroutines(this MonoBehaviour monoBehaviour, IEnumerator[] enumerators)
        {
            foreach (IEnumerator enumerator in enumerators) 
            {
                monoBehaviour.StartCoroutine(enumerator);
            }
        }
        public static void StopCoroutines(this MonoBehaviour monoBehaviour, IEnumerator[] enumerators)
        {
            foreach (IEnumerator enumerator in enumerators)
            {
                monoBehaviour.StopCoroutine(enumerator);
            }
        }
    }
}
