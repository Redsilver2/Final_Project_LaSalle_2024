using Redsilver2.Core.Controls;
using Redsilver2.Core.Player;
using System.Collections;
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

        [Space]
        [SerializeField] private bool allowMouseInteraction = false;

        private IEnumerator mouseInteractionCoroutine;
        public ClampedCameraController ClampedController => clampedController;

        protected override void Awake()
        {
            PlayerController player = PlayerController.Instance;
            mouseInteractionCoroutine = InteractionManager.Instance.MouseInteractionRayCoroutine();
            base.Awake();

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
                AddOnInteractEvent(isInteracting =>
                {
                    clampedController.EnabledControls(false);
                });

                clampedController.AddOnPathFollowStartedEvent(() =>
                {
                    if (!isInteracting && allowMouseInteraction)
                    {
                        if (mouseInteractionCoroutine != null) StopCoroutine(mouseInteractionCoroutine);
                    }

                    clampedController.ResetRotationTrackers();
                });

                clampedController.AddOnPathFollowCompletedEvent(() =>
                {
                    if (isInteracting && allowMouseInteraction)
                    {
                        if (mouseInteractionCoroutine != null) StopCoroutine(mouseInteractionCoroutine);
                        StartCoroutine(mouseInteractionCoroutine);
                    }

                    clampedController.EnabledControls(true);
                    GameManager.SetCursorVisibility(true);
                    Debug.LogWarning("Path followed...");
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
