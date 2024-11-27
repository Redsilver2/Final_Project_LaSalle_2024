using UnityEngine;

namespace Redsilver2.Core
{
    [System.Serializable]
    public struct AnimationData 
    {
        [SerializeField] private AnimationClip animation;
        [SerializeField] private float         crossfadeTime;

        public float CrossFadeTime => crossfadeTime;
        public AnimationClip Animation => animation;

        public bool Compare(string keyword)
        {
            if (this.animation.name.ToLower().Contains(keyword.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
