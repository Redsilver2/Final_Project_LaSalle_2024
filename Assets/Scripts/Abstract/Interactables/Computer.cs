using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Interactables
{
    public class Computer : Lookable
    {
        [Space]
        [SerializeField] private GraphicRaycaster interactionRaycaster;
        [SerializeField] private Button exitButton;

        protected override void Start()
        {
            base.Start();

            if (interactionRaycaster != null)
            {
                interactionRaycaster.ignoreReversedGraphics = true;
                AddOnInteractEvent(isInteracting => { if (!isInteracting) { interactionRaycaster.ignoreReversedGraphics = true; } });

                if (clampedController)
                {
                    clampedController.AddOnPathFollowCompletedEvent(() => { if (isInteracting) { interactionRaycaster.ignoreReversedGraphics = false; } });
                }
            }

            if(exitButton != null) 
            {
                exitButton.onClick.AddListener(() => { Interact(false); });
            }
        }
    }
}
