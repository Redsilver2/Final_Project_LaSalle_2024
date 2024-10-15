using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class InteractableEvents : GameObjectEvents
    {
        private UnityEvent<bool> onInteractOnce;
        private UnityEvent<bool> onInteract;

        protected override void Start()
        {
            onInteractOnce = new UnityEvent<bool>();
            onInteract = new UnityEvent<bool>();

            base.Start();
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
