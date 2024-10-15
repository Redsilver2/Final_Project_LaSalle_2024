using UnityEngine;

namespace Redsilver2.Core.Controls
{
    public class ClampedCameraController : CameraController
    {
        [Space] 
        [SerializeField] private float defaultMinClampedRotationY = -90f;
        [SerializeField] private float defaultMaxClampedRotationY = 90f;

        private float minClampedRotationY;
        private float maxClampedRotationY;

        private float rotationTrackerY = 0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void RotateBody()
        {
            if(playerBody != null)
            {
                rotationTrackerY += inputMotion.x;
                rotationTrackerY = Mathf.Clamp(rotationTrackerY, minClampedRotationY, maxClampedRotationY);
                playerBody.localEulerAngles = Vector3.up * rotationTrackerY;
            }
        }

        public override void ResetClampedRotationValue()
        {
            minClampedRotationY = defaultMinClampedRotationY;
            maxClampedRotationY = defaultMaxClampedRotationY;

            base.ResetClampedRotationValue();
        }

        public void SetRotationTrackerY()
        {
            rotationTrackerY = playerBody.localEulerAngles.y;
        }
    }
}
