using Redsilver2.Core.Events;
using Redsilver2.Core.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Controls
{
    public abstract class CameraController : CameraControllerEvents
    {
        [Space]
        [SerializeField] protected float sensitivityY = 5f;
        [SerializeField] protected float sensitivityX = 5f;

        [Space]
        [SerializeField] private float defaultMinClampedRotationX = -90f;
        [SerializeField] private float defaultMaxClampedRotationX = 90f;

        [Space]
        [SerializeField] protected Transform playerBody;

        private   float minClampedRotationX;
        private   float maxClampedRotationX;
        protected float rotationTrackerX;

        private Transform cameraBody;

        private PlayerCameraControls controls;
        private bool isFollowingPath = false;


        protected bool enableControlsOnStart = false;
        protected Vector2 inputMotion;

        private static IEnumerator followPathCoroutine;     
        public Vector2 InputMotion => inputMotion;

        protected override void Start()
        {
            base.Start();

            cameraBody      = Camera.main.transform;
            controls        = new PlayerCameraControls();

            AddOnPathFollowStartedEvent(() => { isFollowingPath = true; });
            AddOnPathFollowCompletedEvent(() => { isFollowingPath = false; });

            AddOnStateChangedEvent(isEnabled =>
            {
                if (controls != null)
                {
                    if (isEnabled)
                    {
                        controls.Enable();
                    }
                    else
                    {
                        controls.Disable();
                    }
                }
            });

            ResetClampedRotationValue();

            if (enableControlsOnStart)
            {
                controls.Enable();
            }
            else
            {
                enabled = false; 
            }
        }

        protected virtual void Update()
        {
            inputMotion = controls.Movement.Move.ReadValue<Vector2>();
        }

        protected virtual void LateUpdate()
        {
            inputMotion =(Vector2.right * inputMotion.x * sensitivityX + Vector2.up * inputMotion.y * sensitivityY) * Time.deltaTime;

            if (!isFollowingPath)
            {
                RotateBody();
                RotateHead();
                InvokeOnCameraMotionChanged(this);
            }
        }

        protected void OnEnable()
        {
           InvokeStateChangedEvent(true);
        }
        protected void OnDisable()
        {
           InvokeStateChangedEvent(false);
        }

        protected abstract void RotateBody();
        protected void RotateHead()
        {
            rotationTrackerX -= inputMotion.y;
            rotationTrackerX = Mathf.Clamp(rotationTrackerX, minClampedRotationX, maxClampedRotationX);
            cameraBody.localEulerAngles = Vector3.right * rotationTrackerX;
        }

        public void SetSensitvityX(float sensitvityX)
        {
            this.sensitivityX = sensitvityX;
        }
        public void SetSensitivityY(float sensitvityY)
        {
            this.sensitivityY = sensitvityY;
        }

        public void FollowPath(Transform parent, CameraPath[] paths, float duration)
        {
            StopFollowingPath();

            followPathCoroutine = FollowPathCoroutine(parent, paths, duration);

            StartCoroutine(followPathCoroutine);
        }
        public void StopFollowingPath()
        {
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }

            isFollowingPath = false;
        }

        private void CaculateShortestPath(CameraPath[] paths, out int index, ref float duration)
        {
            float closestDistance = Mathf.Infinity;
            index = 0;

            for (int i = 0; i < paths.Length; i++)
            {
                float distance = Vector3.Distance(cameraBody.position, paths[i].Position);

                if (distance < closestDistance)
                {
                    index = i;
                }
            }

            duration /= Mathf.Abs(paths.Length - index);
        }
        private IEnumerator FollowPathCoroutine(Transform parent, CameraPath[] paths, float duration)
        {
            Debug.Log(cameraBody + " " + this.name);

            if (cameraBody != null)
            {
                CaculateShortestPath(paths, out int starterIndex, ref duration);

                cameraBody?.SetParent(parent);
                InvokeOnPathFollowStarted();

                for (int i = starterIndex; i < paths.Length; i++)
                {
                    CameraPath path = paths[i];

                    Vector3 desiredPosition    = path.Position;
                    Quaternion desiredRotation = path.Rotation;

                    Vector3 currentPosition    = cameraBody.localPosition;
                    Quaternion currentRotation = cameraBody.localRotation;

                    float t = 0f;
                    InvokeOnPathIndexChanged(i, paths.Length);

                    while (t < duration)
                    {
                        float progress = t / duration;
                        cameraBody.localPosition = Vector3.Lerp(currentPosition, desiredPosition, progress);
                        cameraBody.localRotation = Quaternion.Lerp(currentRotation, desiredRotation, progress);

                        t += Time.deltaTime;
                        yield return null;
                    }

                    if (t >= duration)
                    {
                        cameraBody.localPosition = desiredPosition;
                        cameraBody.localRotation = desiredRotation;
                    }
                }

                InvokeOnPathFollowCompleted();
            }
        }

        public void SetMinClampedRotationX(float minClampedRotationX) 
        {
            this.minClampedRotationX = minClampedRotationX;
        }
        public void SetMaxClampedRotationX(float maxClampedRotationX)
        {
            this.maxClampedRotationX = maxClampedRotationX;
        }

        public virtual void ResetClampedRotationValue()
        {
            minClampedRotationX = defaultMinClampedRotationX;
            maxClampedRotationX = defaultMaxClampedRotationX;
        }

        public virtual void ResetRotationTrackers()
        {
            rotationTrackerX = 0f;
        }
    }
}
