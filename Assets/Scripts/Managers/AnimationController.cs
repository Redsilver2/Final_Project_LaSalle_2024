using System.Linq;
using UnityEngine;

namespace Redsilver2.Core
{
    [System.Serializable]
    public class AnimationController
    {

        [Space]
        [SerializeField] protected RuntimeAnimatorController animatorController;
        [SerializeField] private   AnimationData[] animationDatas;

        private AnimationClip currentAnimation;
        protected string currentAnimationState = string.Empty;
        protected Animator animator;

        public void Init(Animator animator, bool isDisabled)
        {
            this.animator = animator;

            if (animator != null)
            {
                animator.runtimeAnimatorController = animatorController;

                if (isDisabled)
                {
                    Disable();
                }
            }
        }

        protected void PlayAnimation(string stateName, float crossFade)
        {
            if (animator != null && stateName != string.Empty && stateName != currentAnimationState)
            {
                currentAnimationState = stateName;
                animator.CrossFade(stateName, crossFade);
            }
        }

        public void ResetAnimationState()
        {
            currentAnimationState = string.Empty;
            currentAnimation = null;
        }

        public void PlayAnimation(string keyword)
        {
            if (animator == null)
            {
                Debug.LogWarning("No animator found!");
                return;
            }

            bool canPlayAnimation = true;

            if (currentAnimationState != string.Empty) 
            {
                if (currentAnimationState.ToLower().Contains(keyword.ToLower()))
                {
                    canPlayAnimation = false;
                }
            }

            if (canPlayAnimation && TryGetAnimationDatasByKeyword(keyword, out AnimationData[] animationDatas))
            {
                AnimationData currentAnimationData = GetRandomAnimationData(animationDatas);

                if (currentAnimationData != null)
                {
                    currentAnimation = currentAnimationData.Animation;
                    PlayAnimation(currentAnimation.name, currentAnimationData.CrossFadeTime);
                }
                else
                {
                    Debug.LogWarning("No animation data found!");
                }
            }
        }

        private AnimationData GetRandomAnimationData(AnimationData[] animationDatas)
        {
            int index = Random.Range(0, animationDatas.Length);
            return animationDatas[index];
        }

        public float GetAnimationLenght(string keyword)
        {
            if (currentAnimation != null)
            {
                return currentAnimation.length;
            }

            return 0f;
        }
        private bool TryGetAnimationDataByKeyword(string keyword, out AnimationData animationData)
        {
            animationData = null;

            for (int i = 0; i < animationDatas.Length; i++)
            {
                if (animationDatas[i].Compare(keyword))
                {
                    animationData = animationDatas[i];
                    return true;
                }
            }

            return false;
        }

        private bool TryGetAnimationDatasByKeyword(string keyword, out AnimationData[] animationDatas)
        {
            animationDatas = null;

            if (TryGetAnimationDataByKeyword(keyword, out AnimationData data))
            {
                animationDatas = this.animationDatas.Where(x => x.Compare(keyword)).ToArray();
                Debug.LogError(animationDatas.Length);
                return true;
            }

            return false;
        }

        public void Enable() 
        {
            animator.enabled = true;
            animator.runtimeAnimatorController = animatorController;
        }

        public void Disable() 
        {
            animator.enabled = false;
            animator.runtimeAnimatorController = null;
            currentAnimationState = string.Empty;
        }
    }
}
