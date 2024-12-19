using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.UI
{
    [System.Serializable]
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Image bloodSfx;
        private IEnumerator bloodSFXCoroutine;

        public void BloodSFX(float fillAmount, bool tookDamage)
        {
            if (bloodSfx != null)
            {
                if (tookDamage)
                {
                    if (bloodSFXCoroutine != null) StopCoroutine(bloodSFXCoroutine);
                    bloodSFXCoroutine = BloodSFXCoroutine(0.5f, fillAmount);
                    StartCoroutine(bloodSFXCoroutine);
                }
                else
                {
                    float desiredAlpha = Mathf.Lerp(1f, 0f, fillAmount);

                    if(fillAmount >= 1f)
                    {
                        desiredAlpha = 1f;
                    }

                    bloodSfx.color = new Color(bloodSfx.color.r, bloodSfx.color.g, bloodSfx.color.b,
                                              desiredAlpha);
                }
            }
        }

        private IEnumerator BloodSFXCoroutine(float duration, float fillAmount)
        {
            float currentAlpha = bloodSfx.color.a;
            float desiredAlpha = Mathf.Lerp(1f, 0f, fillAmount);

            yield return Counter.WaitForSeconds(duration, value =>
            {

                bloodSfx.color = new Color(bloodSfx.color.r, bloodSfx.color.g, bloodSfx.color.b,
                                           Mathf.Lerp(currentAlpha, desiredAlpha, value/duration));
            },
            () =>
            {
                bloodSfx.color = new Color(bloodSfx.color.r, bloodSfx.color.g, bloodSfx.color.b,
                                           desiredAlpha);
            });
        }

    }
}
