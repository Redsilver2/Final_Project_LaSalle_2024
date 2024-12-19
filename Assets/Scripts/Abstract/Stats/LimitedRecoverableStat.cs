using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Stats
{
    public abstract class LimitedRecoverableStat : RecoverableStat
    {
        [Space]
        [SerializeField][Range(0.1f, 1f)] private float maxRecoveryThreshold;

        protected sealed override IEnumerator RecoveryCoroutine()
        {
            float maxCurrentValue = maxValue * maxRecoveryThreshold;
            Debug.LogWarning("What the fuck... " + maxCurrentValue);
            yield return new WaitForSeconds(defaultRecoveryWaitTime);
            Debug.LogWarning("ok");
            while (currentValue < maxCurrentValue)
            {
                Increase(recoverySpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
