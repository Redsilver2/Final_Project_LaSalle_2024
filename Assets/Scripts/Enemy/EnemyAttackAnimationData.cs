using UnityEngine;

namespace Redsilver2.Core.Enemy
{
    [System.Serializable]
    public class EnemyAttackAnimationData : AnimationData
    {
        [Space]
        [SerializeField] private float minAttackRange;
        [SerializeField] private float maxAttackRange;

        [Space]
        [SerializeField] private float waitCooldown;

        public float MinAttackRange => minAttackRange;
        public float MaxAttackRange => maxAttackRange;
        public float WaitCooldown => waitCooldown;

    }
}
