using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class InteractableEvents : GameObjectEvents
    {
        [SerializeField] private UnityEvent<bool> onInteractOnce;
        [SerializeField] private UnityEvent<bool> onInteract;

        UnityAction event01;

        protected override void Awake()
        {
            base.Awake();
            event01 = OnMessagedTriggered;
            event01.Invoke();
        }

        public void OnMessagedTriggered()
        {
            Debug.Log("This Object's Event Triggered");
        }

        public void AddOnInteractOnceEvent(UnityAction<bool> action)
        {
            onInteractOnce?.AddListener(action);
        }
        public void RemoveOnInteractOnceEvent(UnityAction<bool> action)
        {
            onInteractOnce?.AddListener(action);
        }

        public void AddOnInteractEvent(UnityAction<bool> action)
        {

            Debug.Log("Interaction Event Added");
            onInteract?.AddListener(action);
        }
        public void RemoveOnInteractEvent(UnityAction<bool> action)
        {
            Debug.Log("Interaction Event Removed");
            onInteract?.RemoveListener(action);
        }

        protected void InvokeOnInteractOnce(bool isInteracting) 
        {
            Debug.Log("Interaction Once Invoked");
            onInteractOnce?.Invoke(isInteracting); 
        }
        protected void InvokeOnInteract(bool isInteracting)
        {
            Debug.Log("Interaction Invoked");
            onInteract?.Invoke(isInteracting);
        }
    }

}
