using UnityEngine;

namespace Redsilver2.Core
{
    [System.Serializable]
    public class AnimationController
    {

        [Space]
        [SerializeField] protected RuntimeAnimatorController animatorController;
        [SerializeField] private AnimationData[] animationDatas;

        private string currentAnimationState;
        private Animator animator;

        public void Init(GameObject item)
        {
            this.animator = item.GetComponent<Animator>();
            Disable();
            animator.runtimeAnimatorController = animatorController;
        }

        protected void PlayAnimation(string stateName, float crossFade)
        {
            if (animator != null && stateName != string.Empty && stateName != currentAnimationState)
            {
                if (crossFade < 0.2f) { crossFade = 0.2f; }
                animator.enabled = true;
                currentAnimationState = stateName;
                animator.CrossFade(stateName, crossFade);
            }
        }

        public void ResetAnimationState()
        {
            currentAnimationState = string.Empty;
        }

        public void PlayAnimation(string keyword)
        {
            if(TryGetAnimationDataByKeyword(keyword, out AnimationData animationData))
            {
                PlayAnimation(animationData.Animation.name, animationData.CrossFadeTime);
            }
        }
        public float GetAnimationLenght(string keyword)
        {
            if (TryGetAnimationDataByKeyword(keyword, out AnimationData animationData))
            {
                return animationData.Animation.length;
            }

            return 0f;
        }
        private bool TryGetAnimationDataByKeyword(string keyword, out AnimationData animationData)
        {
            animationData = new AnimationData();

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
