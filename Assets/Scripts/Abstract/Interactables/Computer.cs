using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Interactables
{
    public class Computer : Lookable
    {
        [Space]
        [SerializeField] private   GraphicRaycaster interactionRaycaster;
        [SerializeField] private   Button exitButton;

        protected override void Awake()
        {
            base.Awake();

            if (interactionRaycaster != null)
            {
                AddOnInteractEvent(isInteracting => 
                { if (!isInteracting) 
                  { 
                    interactionRaycaster.enabled = false; 
                  } 
                });

                if (clampedController)
                {
                    clampedController.AddOnPathFollowCompletedEvent(() => 
                    {
                        if (isInteracting)
                        { 
                            interactionRaycaster.enabled = true; 
                        }
                    });
                }
            }

            if(exitButton != null) 
            {
                exitButton.onClick.AddListener(() => { Interact(false); });
            }
        }

        protected virtual void Start()
        { 
            interactionRaycaster.ignoreReversedGraphics = false;
            interactionRaycaster.enabled = false;
        }
    }
}
