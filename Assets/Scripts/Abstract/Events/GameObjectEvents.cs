using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class GameObjectEvents : MonoBehaviour
    {
        private UnityEvent<bool> onStateChanged;

        protected virtual void Start()
        {
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

        public void InvokeStateChangedEvent(bool isEnabled)
        {
            onStateChanged?.Invoke(isEnabled);
        }
    }
}
