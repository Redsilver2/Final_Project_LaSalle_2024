using UnityEngine;
using System.Linq;

namespace Redsilver2.Core.Enemy
{
    [System.Serializable]
    public class EnemyAnimationController : AnimationController
    {
        [Space]
        [SerializeField] private EnemyAttackAnimationData[] attackAnimationDatas;
        public EnemyAttackAnimationData[] AttackAnimationDatas => attackAnimationDatas;

        public void PlayAttackAnimation(float distanceToPlayer, out float waitCooldown, out bool isAttaking)
        {
            EnemyAttackAnimationData[] validAttackAnimationDatas = GetAttackAnimationDatas(distanceToPlayer);

            isAttaking = false;
            waitCooldown = 0f;

            if (attackAnimationDatas.Length > 0) 
            {
                int index = Random.Range(0, validAttackAnimationDatas.Length);
                EnemyAttackAnimationData randomAttackAnimationData = attackAnimationDatas[index];
                AnimationClip clip = randomAttackAnimationData.Animation;

                if (clip != null)
                {
                    PlayAnimation(clip.name,
                                  randomAttackAnimationData.CrossFadeTime);

                    isAttaking   = true;
                    waitCooldown = clip.length;
                }
            }
        }

        private EnemyAttackAnimationData[] GetAttackAnimationDatas(float distanceToPlayer)
        {
            if (attackAnimationDatas.Length > 0)
            {
                return attackAnimationDatas.Where(x => distanceToPlayer >= x.MinAttackRange && distanceToPlayer <= x.MaxAttackRange).ToArray();
            }

            return new EnemyAttackAnimationData[0];
        }
    }
}
