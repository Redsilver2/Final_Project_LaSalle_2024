using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Stats
{
    public abstract class LimitedRecoverableStat : RecoverableStat
    {
        [Space]
        [SerializeField][Range(0.1f, 1f)] private float maxRecoveryThreshold;

        protected override void Start()
        {
            base.Start();
            AddOnValueChangedEvent((handler, isIncreasingValue) =>
            {
                if (!isIncreasingValue)
                {
                    StopRecovery();
                    StartRecovery();
                }
            });
        }

        protected override IEnumerator RecoveryCoroutine()
        {
            float maxCurrentValue = currentValue * maxRecoveryThreshold;
            yield return new WaitForSeconds(defaultRecoveryWaitTime);
            
            while(currentValue < maxCurrentValue)
            {
                Increase(recoverySpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
