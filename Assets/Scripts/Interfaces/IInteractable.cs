using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public interface IInteractable
    {
        void Interact();
        void Interact(bool isInteracting);
    }
}
