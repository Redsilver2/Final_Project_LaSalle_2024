using Redsilver2.Core.Controls;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class CameraControllerEvents : GameObjectEvents
    {
        private UnityEvent onPathFollowStarted;
        private UnityEvent onPathFollowCompleted;
       
        private UnityEvent<int> onPathIndexChanged;
        private UnityEvent<CameraController> onCameraMotionChanged;

        protected override void Start()
        {
            onPathFollowStarted = new UnityEvent();
            onPathFollowCompleted = new UnityEvent();

            onPathIndexChanged = new UnityEvent<int>();
            onCameraMotionChanged = new UnityEvent<CameraController>();

            base.Start();
        }

        public void AddOnPathFollowStartedEvent(UnityAction action)
        {
            onPathFollowStarted?.AddListener(action);
        }
        public void RemoveOnPathFollowStartedEvent(UnityAction action)
        {
            onPathFollowStarted?.RemoveListener(action);
        }

        public void AddOnPathFollowCompletedEvent(UnityAction action)
        {
            onPathFollowCompleted?.AddListener(action);
        }
        public void RemoveOnPathFollowCompletedEvent(UnityAction action)
        {
            onPathFollowCompleted?.RemoveListener(action);
        }

        public void AddOnPathIndexChangedEvent(UnityAction<int> action)
        {
            onPathIndexChanged?.AddListener(action);
        }
        public void RemoveOnPathIndexChangedEvent(UnityAction<int> action)
        {
            onPathIndexChanged?.RemoveListener(action);
        }

        public void AddOnCameraMotionChangedEvent(UnityAction<CameraController> action)
        {
            onCameraMotionChanged?.AddListener(action);
        }
        public void RemoveOnCameraMotionChangedEvent(UnityAction<CameraController> action)
        {
            onCameraMotionChanged?.RemoveListener(action);
        }
     
        protected void InvokeOnCameraMotionChanged(CameraController controller)
        {
            if(controller != null)
            {
                onCameraMotionChanged?.Invoke(controller);
            }
        }  
        protected void InvokeOnPathIndexChanged(int index, int lenght)
        {
            if (index > 0 && index < lenght)
            {
                onPathIndexChanged?.Invoke(index);
            }
        }
        protected void InvokeOnPathFollowStarted()
        {
            onPathFollowStarted?.Invoke();
        }
        protected void InvokeOnPathFollowCompleted()
        {
            onPathFollowCompleted?.Invoke();
        }
    }
}
