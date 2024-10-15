using UnityEngine;

namespace Redsilver2.Core.Motion
{
    public class PlayerCameraMotionHandler : PlayerMotionHandler
    {
        [SerializeField] private PlayerCameraMotionSetting cameraMotionSetting;
        private float rotationTrackerZ;

        protected override void Start()
        {
            this.motionSetting = cameraMotionSetting;
            base.Start();
        }

        public void SetCameraMotionSetting(PlayerCameraMotionSetting cameraMotionSetting)
        {
            this.cameraMotionSetting = cameraMotionSetting;
            this.motionSetting       = cameraMotionSetting;
        }

        protected override void SetRotationValue(Vector2 inputMotion)
        {
            rotationTrackerZ -= inputMotion.x;
            rotationTrackerZ = Mathf.Clamp(rotationTrackerZ, cameraMotionSetting.MinRotationZ, cameraMotionSetting.MaxRotationZ);
            base.SetRotationValue(inputMotion);
        }

        protected override void ResetRotationValue(Vector3 defaultRotation, float lerpRotationSpeed)
        {
            rotationTrackerZ = Mathf.Lerp(rotationTrackerZ, defaultRotation.z, lerpRotationSpeed * Time.deltaTime);
            base.ResetRotationValue(defaultRotation, lerpRotationSpeed);
        }

        protected override Vector3 GetDesiredRotation()
        {
            return Vector3.forward * rotationTrackerZ;
        }
    }
}
