using Redsilver2.Core.Controls;
using Redsilver2.Core.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Interactables
{

    public abstract class Lookable : Interactable
    {
        [SerializeField] protected ClampedCameraController clampedController;

        [Space]
        [SerializeField] private float pathFollowDuration;

        [Space]
        [SerializeField] private CameraPath[] interactionPaths;

        protected override void Start()
        {
            PlayerController player = PlayerController.Instance;
            base.Start();

            if (Camera.main.TryGetComponent(out PlayerCameraController cameraController))
            {
                cameraController.AddOnPathFollowStartedEvent(() =>
                {
                    GameManager.SetCursorVisibility(false);
                });

                cameraController.AddOnPathFollowCompletedEvent(() =>
                {
                    if(player != null)
                    {
                        player.enabled = true;
                    }
                });
            }

            if (clampedController != null)
            {
                clampedController.SetRotationTrackerY();

                clampedController.AddOnPathFollowStartedEvent(() =>
                {
                    clampedController.ResetRotationTrackers();
                });

                clampedController.AddOnPathFollowCompletedEvent(() =>
                {
                    GameManager.SetCursorVisibility(true);
                });

                clampedController.enabled = false;
            }

            AddOnInteractEvent(isInteracting =>
            {
                if(cameraController && clampedController != null && player != null)
                {
                    if (isInteracting)
                    {

                        cameraController.enabled = false;
                        player.enabled = false;

                        clampedController.enabled = true;
                        clampedController.FollowPath(clampedController.transform, interactionPaths, pathFollowDuration);

                    }
                    else
                    {
                        clampedController.enabled = false;
                        cameraController.FollowPathToDefaultParent(interactionPaths, pathFollowDuration);
                    }
                }
            });
        }
    }
}
