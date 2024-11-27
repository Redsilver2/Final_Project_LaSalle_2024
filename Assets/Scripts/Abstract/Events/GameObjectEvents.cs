using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class GameObjectEvents : MonoBehaviour
    {
        private UnityEvent<bool> onStateChanged;
        private UnityEvent onDestroy;

        protected virtual void Awake()
        {
            onDestroy      = new UnityEvent();
            onStateChanged = new UnityEvent<bool>();
        }

        public void AddOnStateChangedEvent(UnityAction<bool> action)
        {
            onStateChanged?.AddListener(action);
        }
        public void RemoveOnStateChangedEvent(UnityAction<bool> action)
        {
            onStateChanged?.RemoveListener(action);
        }

        public void AddOnDestroyEvent(UnityAction action)
        {
            onDestroy.AddListener(action);
        }
        public void RemoveOnDestoryEvent(UnityAction action)
        {
            onDestroy.RemoveListener(action);
        }

        protected virtual void OnEnable()
        {
            onStateChanged?.Invoke(true);
        }
        protected virtual void OnDisable()
        {
            onStateChanged?.Invoke(false);
        }
        protected virtual void OnDestroy()
        {
            //onDestroy.Invoke();
        }
    }
}
