using Redsilver2.Core.SceneManagement;
using UnityEngine;

namespace Redsilver2.Core.Controls
{
    public class ClampedCameraController : CameraController
    {
        [Space] 
        [SerializeField] private float defaultMinClampedRotationY = -90f;
        [SerializeField] private float defaultMaxClampedRotationY = 90f;

        [Space]
        [SerializeField] private bool  enableControlsOnStart = false;

        private float minClampedRotationY;
        private float maxClampedRotationY;

        private float rotationTrackerY = 0f;
        private Vector2 defaultRotation;

        protected override void Awake()
        {
            base.Awake();
           
            defaultRotation = playerBody.localEulerAngles;
            rotationTrackerY = defaultRotation.y;

            AddOnPathFollowStartedEvent(() =>
            {
                rotationTrackerY =  defaultRotation.y;
                rotationTrackerX =  defaultRotation.x;
            });

            if (!enableControlsOnStart)
            {
                enabled = false;
            }
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

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}
