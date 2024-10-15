using UnityEngine;

namespace Redsilver2.Core.Motion
{
    [CreateAssetMenu(menuName = "Player/Camera Motion", fileName = "New Player Camera Motion")]
    public class PlayerCameraMotionSetting : PlayerMotionSetting
    {
        [Space]
        [SerializeField] private float minRotationZ;
        [SerializeField] private float maxRotationZ;

        public float MinRotationZ => minRotationZ;
        public float MaxRotationZ => maxRotationZ;
    }
}
