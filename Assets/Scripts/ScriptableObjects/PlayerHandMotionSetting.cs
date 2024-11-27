using UnityEngine;

namespace Redsilver2.Core.Motion
{
    [CreateAssetMenu(menuName = "Player/Hand Motion", fileName = "New Player Hand Motion")]
    public class PlayerHandMotionSetting : PlayerMotionSetting
    {
        [Space]
        [SerializeField] private float minRotationY;
        [SerializeField] private float maxRotationY;

        public float MinRotationY => minRotationY;
        public float MaxRotationY => maxRotationY;
    }
}
