using UnityEngine;

namespace Redsilver2.Core.Motion
{
    public class PlayerHandMotionHandler : PlayerMotionHandler
    {
        [SerializeField] private PlayerHandMotionSetting handMotionSetting;
        private float rotationTrackerY;

        protected override void Start()
        {
            this.motionSetting = handMotionSetting;
            base.Start();
        }

        public void SethandMotionSetting(PlayerHandMotionSetting handMotionSetting)
        {
            this.handMotionSetting = handMotionSetting;
            this.motionSetting     = handMotionSetting;
        }

       

        protected override void SetRotationValue(Vector2 inputMotion)
        {
            rotationTrackerY += inputMotion.x;
            rotationTrackerY = Mathf.Clamp(rotationTrackerY, handMotionSetting.MinRotationY, handMotionSetting.MaxRotationY);
            base.SetRotationValue(inputMotion);
        }

        protected override void ResetRotationValue(Vector3 defaultRotation, float lerpRotationSpeed)
        {
            rotationTrackerY = Mathf.Lerp(rotationTrackerY, defaultRotation.y, lerpRotationSpeed * Time.deltaTime);
            base.ResetRotationValue(defaultRotation, lerpRotationSpeed);
        }

        protected override Vector3 GetDesiredRotation()
        {
            return Vector3.up * rotationTrackerY;
        }
    }
}
