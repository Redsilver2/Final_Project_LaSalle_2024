using Redsilver2.Core.Controls;
using Redsilver2.Core.Player;
using UnityEngine;

namespace Redsilver2.Core.Motion
{
    public abstract class PlayerMotionHandler : MonoBehaviour
    {
        protected PlayerMotionSetting motionSetting;
        private   float currentPositionLerpSpeed;
        private   float rotationTrackerX;

        protected virtual void Start()
        {
            PlayerController player = PlayerController.Instance;

            if (motionSetting != null)
            {
                currentPositionLerpSpeed = motionSetting.MinPositionLerpSpeed;
            }

            if (player != null)
            {
                player.AddOnMovementMotionChangedEvent(MovementMotionEvent);
            }

            if(Camera.main.TryGetComponent(out PlayerCameraController cameraController))
            {
                Debug.Log(cameraController);
                cameraController.AddOnCameraMotionChangedEvent(CameraMotionEvent);
            }
        }

        private void MovementMotionEvent(PlayerController controller)
        {
            if (motionSetting != null)
            {
                Vector3 result;
                Vector3 desiredPosition;

                Vector3 defaultPosition = motionSetting.DefaultPosition;
                float desiredPositionLerpSpeed = motionSetting.MinPositionLerpSpeed;

                if (controller.IsRunning)
                {
                    desiredPositionLerpSpeed = motionSetting.MaxPositionLerpSpeed;
                }


                currentPositionLerpSpeed = Mathf.Clamp(currentPositionLerpSpeed, desiredPositionLerpSpeed, Time.deltaTime);

                if (controller.InputMotion.magnitude > 0f)
                {
                    desiredPosition = GetDesiredPosition(defaultPosition);
                }
                else
                {
                    desiredPosition = defaultPosition;
                }

                result = Vector3.Lerp(transform.localPosition, desiredPosition, currentPositionLerpSpeed * Time.deltaTime);
                transform.localPosition = result;
            }
        }

        private void CameraMotionEvent(CameraController controller)
        {
            if (motionSetting != null)
            {
                Vector2 inputMotion = controller.InputMotion;
                Vector3 defaultRotation = motionSetting.DefaultRotation;

                float lerpRotationSpeed = motionSetting.DefaultRotationLerpSpeed;

                if (inputMotion.magnitude > 0f)
                {
                    SetRotationValue(inputMotion);
                }
                else
                {
                    ResetRotationValue(defaultRotation, lerpRotationSpeed);
                }

                transform.localEulerAngles = Vector3.right * rotationTrackerX + GetDesiredRotation();
            }
        }

        protected virtual void SetRotationValue(Vector2 inputMotion)
        {
            rotationTrackerX -= inputMotion.y;
            rotationTrackerX = Mathf.Clamp(rotationTrackerX, motionSetting.MinRotationX, motionSetting.MaxRotationX);
        }

        protected virtual void ResetRotationValue(Vector3 defaultRotation, float lerpRotationSpeed)
        {
            rotationTrackerX = Mathf.Lerp(rotationTrackerX, defaultRotation.x, lerpRotationSpeed * Time.deltaTime);
        }

        private Vector3 GetDesiredPosition(Vector3 defaultPosition)
        {
            float sin = Mathf.Abs(Mathf.Sin(Time.time * currentPositionLerpSpeed));

            float x = Mathf.Lerp(motionSetting.MinPositionX, motionSetting.MaxPositionX, sin);
            float y = Mathf.Lerp(motionSetting.MinPositionY, motionSetting.MaxPositionY, sin);

            return new Vector3(x, y, defaultPosition.z);
        }
        protected abstract Vector3 GetDesiredRotation();
    }
}
