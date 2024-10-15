using System.Collections;
using UnityEngine;

public abstract class RecoverableStat : StatHandler
{
    [Space]
    [Header("Recovery Settings")]
    [SerializeField] private bool isRecoveryAllowed = true;

    [Space]
    [SerializeField] protected float recoveryDelay;
    [SerializeField] protected float recoverySpeed;

    protected bool isRecoveringStat = false;
    private IEnumerator recoveryCoroutine;


    public virtual void StartRecovery()
    {
        if (!isRecoveringStat && isRecoveryAllowed)
        {
            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
            }

            recoveryCoroutine = RecoveryCoroutine();
            StartCoroutine(recoveryCoroutine);

            isRecoveringStat = true;
        }
    }
    public virtual void CancelRecovery()
    {
        if (isRecoveringStat && isRecoveryAllowed)
        {
            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
            }

            isRecoveringStat = false;
        }
    }

    protected abstract IEnumerator RecoveryCoroutine();
}
