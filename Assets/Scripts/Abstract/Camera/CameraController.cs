using Redsilver2.Core.Counters;
using Redsilver2.Core.Events;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using System.Timers;
using UnityEngine;
using Timer = Redsilver2.Core.Counters.Timer;

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

        private bool lastEnabledState;

        private   float minClampedRotationX;
        private   float maxClampedRotationX;
        protected float rotationTrackerX;

        private Transform cameraBody;
        protected PlayerControls.CameraActions controls;
        private bool isFollowingPath = false;


        protected Vector2 inputMotion;
        private static IEnumerator followPathCoroutine;     
        public Vector2 InputMotion => inputMotion;

        protected override void Awake()
        {
            base.Awake();
            Camera camera             = Camera.main;

            cameraBody                = camera.transform;
            controls                  = GameManager.Instance.GetComponent<InputManager>().PlayerControls.Camera;

            AddOnPathFollowStartedEvent(() => { isFollowingPath = true; });
            AddOnPathFollowCompletedEvent(() => { isFollowingPath = false; });
            ResetClampedRotationValue();

            base.OnEnable();
            PauseManager.AddOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadLevel);
        }

        protected virtual void Update()
        {
            inputMotion = controls.Move.ReadValue<Vector2>();
        }

        protected virtual void LateUpdate()
        {
            inputMotion = (Vector2.right * inputMotion.x * sensitivityX + Vector2.up * inputMotion.y * sensitivityY) * Time.deltaTime;

            if (!isFollowingPath)
            {
                RotateBody();
                RotateHead();
                InvokeOnCameraMotionChanged(this);
            }
        }

        private void OnLoadLevel(int Index)
        {
            PauseManager.RemoveOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadLevel);
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

        public void EnabledControls(bool isEnabled)
        {
            if(isEnabled)
            {
                controls.Enable();
            }
            else
            {
                controls.Disable();
            }
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
                    closestDistance = distance;
                }
            }

            duration /= Mathf.Abs(index + 1);
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

                    Vector3 currentPosition    = cameraBody.localPosition;
                    Quaternion currentRotation = cameraBody.localRotation;
                    InvokeOnPathIndexChanged(i, paths.Length);

                    yield return Counter.WaitForSeconds(duration, value =>
                    {
                        cameraBody.localPosition = Vector3.Lerp(currentPosition, path.Position, value);
                        cameraBody.localRotation = Quaternion.Lerp(currentRotation, path.Rotation, value);
                    },
                    () =>
                    {
                        cameraBody.localPosition = path.Position;
                        cameraBody.localRotation = path.Rotation;
                    });
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

        private void OnGamePausedEvent(bool isGamePaused)
        {
            if (!isGamePaused)
            {
                if (lastEnabledState)
                {
                    enabled = true;
                }
            }
            else
            {
                lastEnabledState = enabled;
                enabled = false;
            }
        }
    }
}
