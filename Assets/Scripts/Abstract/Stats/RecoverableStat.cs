using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Stats
{
    public abstract class RecoverableStat : StatHandler
    {
        [Space]
        [Header("Base Recovery Settings")]
        [SerializeField] private bool allowRecovery = false;

        [Space]
        [SerializeField]                    protected float defaultRecoveryWaitTime = 1.0f;
        [SerializeField]                    private float   defaultRecoverySpeed    = 1.0f;

        protected float recoveryWaitTime = 1.0f;
        protected float recoverySpeed    = 1.0f;
        private IEnumerator recoveryCoroutine;

        public float DefaultRecoverySpeed => defaultRecoverySpeed;
        public float DefaultRecoveryWaitTime => defaultRecoveryWaitTime;

        protected override void Awake()
        {
            base.Awake();   
            recoveryWaitTime = defaultRecoveryWaitTime;
            recoverySpeed    = defaultRecoverySpeed;
        }

        public void SetRecoverySpeed(float recoverySpeed)
        {
            this.recoverySpeed = recoverySpeed;
        }

        public void SetRecoveryWaitTime(float recoveryWaitTime)
        {
            this.recoveryWaitTime = recoveryWaitTime;
        }

        public void StartRecovery()
        {
            if (allowRecovery)
            {
                StopRecovery();

                if (recoveryCoroutine == null)
                {
                    recoveryCoroutine = RecoveryCoroutine();
                    StartCoroutine(recoveryCoroutine);
                }
            }
        }
        public void StopRecovery()
        {
            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
                recoveryCoroutine = null;
            }
        }

        protected virtual IEnumerator RecoveryCoroutine()
        {
            yield return new WaitForSeconds(defaultRecoveryWaitTime);

            while (currentValue < maxValue)
            {
                Increase(recoverySpeed * Time.deltaTime);
                yield return null;
            }

            if(currentValue > maxValue)
            {
                currentValue = maxValue;
            }
        }
    }
}
