using Redsilver2.Core.Controls;
using System;
using UnityEngine;

namespace Redsilver2.Core.Player
{
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerCameraController : CameraController
    {
        private Transform defaultParent;
        private Quaternion lastLocalRotation;

        protected override void Awake()
        {
            Transform transform = this.transform.parent;
            PlayerController player = PlayerController.Instance;

            base.Awake();

            if(player !=  null)
            {
                playerBody = player.transform;

                player.AddOnStateChangedEvent(isEnabled =>
                {
                    if(isEnabled)
                    {
                        GameManager.SetCursorVisibility(false);
                        controls.Enable();
                    }
                    else
                    {
                        lastLocalRotation = transform.localRotation;
                        controls.Disable();
                    }

                    enabled = isEnabled;
                });
            }

            defaultParent = transform;
            Camera.main.nearClipPlane = 0.01f;

            controls.Enable();
        }

        protected override void RotateBody()
        {
            if (playerBody != null)
            {
                playerBody?.Rotate(Vector3.up * inputMotion.x);
            }
        }

        public void FollowPathToDefaultParent(CameraPath[] paths, float duration)
        {
            CameraPath[] newPaths  = new CameraPath[paths.Length + 1];

            for (int i = 0; i < paths.Length; i++)
            {
                newPaths[i] = paths[i];
            }

            Array.Reverse(newPaths, 0, paths.Length);
            newPaths[paths.Length - 1] = new CameraPath(Vector3.zero, Quaternion.identity);

            Debug.Log(newPaths);

            FollowPath(defaultParent, newPaths, duration);
        }
    }
}
