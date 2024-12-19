using Redsilver2.Core.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Items
{
    public class Key : EquippableItem
    {
        protected override void SetControls(PlayerControls controls, bool isSettingControls)
        {
            if (isSettingControls)
            {

            }
            else
            {

            }
        }

        private void OnFireInputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Camera camera = Camera.main;
                InteractionManager interactionManager = InteractionManager.Instance;

                if (camera == null || interactionManager == null)
                {
                    return;
                }

                Transform transform = camera.transform;

                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 2f) && hitInfo.collider != null)
                {
                    if (interactionManager.TryGetInteractable(hitInfo.collider, out Interactable interactable))
                    {
                        Door door = interactable as Door;

                        if (door != null)
                        {
                            door.SetLockState(this);
                        }
                    }
                }
            }
        }
    }
}
