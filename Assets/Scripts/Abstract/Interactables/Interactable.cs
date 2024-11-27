using Redsilver2.Core.Events;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public abstract class Interactable : InteractableEvents, IInteractable, IDescribable
    {
        [SerializeField] protected string interactableName;

        private bool hasInteractedOnce = false;
        protected bool isInteracting = false;

        public string InteractableName => interactableName;
        public bool IsInteracting => isInteracting;

        protected override void Awake()
        {
            base.Awake();
            InteractionManager.Instance.AddInteractableInstance(GetComponent<Collider>(), this);
        }

        public virtual void Interact()
        {
            isInteracting = !isInteracting;
            InvokeInteractableEvents();
        }

        protected void InvokeInteractableEvents()
        {
            if (!hasInteractedOnce)
            {
                InvokeOnInteractOnce(isInteracting);
                hasInteractedOnce = true;
            }


            Debug.Log("Interact (1)");
            InvokeOnInteract(isInteracting);
        }

        public void Interact(bool isInteracting)
        {
            if(this.isInteracting != isInteracting)
            {
                this.isInteracting = !isInteracting;
                Interact(); 
            }
        }

        public virtual string GetName()
        {
            return interactableName;
        }

        public virtual Sprite GetIcon()
        {
            return null;
        }
    }
}
