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

        private bool isRecovering = false;
        private IEnumerator recoveryCoroutine;

        public float DefaultRecoverySpeed => defaultRecoverySpeed;
        public float DefaultRecoveryWaitTime => defaultRecoveryWaitTime;

        protected override void Start()
        {
            base.Start();   
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
            if (!isRecovering && allowRecovery)
            {
                if (recoveryCoroutine == null)
                {
                    recoveryCoroutine = RecoveryCoroutine();
                    StartCoroutine(recoveryCoroutine);
                }

                isRecovering = true;
            }
        }
        public void StopRecovery()
        {
            if (isRecovering && allowRecovery)
            {
                if (recoveryCoroutine != null)
                {
                    StopCoroutine(recoveryCoroutine);
                    recoveryCoroutine = null;
                }

                isRecovering = false;
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
