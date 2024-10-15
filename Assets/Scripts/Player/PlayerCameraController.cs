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

        protected override void Start()
        {
            Transform transform = this.transform.parent;
            PlayerController player = PlayerController.Instance;

            enableControlsOnStart = true;
            base.Start();

            if(player !=  null)
            {
                playerBody = player.transform;

                player.AddOnStateChangedEvent(isEnabled =>
                {
                    if(isEnabled)
                    {
                        GameManager.SetCursorVisibility(false);
                    }
                    else
                    {
                        lastLocalRotation = transform.localRotation;
                    }

                    enabled = isEnabled;
                });
            }

            defaultParent = transform;
            GetComponent<Camera>().nearClipPlane = 0.01f;
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
            newPaths[paths.Length - 1] = new CameraPath(Vector3.zero, lastLocalRotation);

            Debug.Log(newPaths);

            FollowPath(defaultParent, newPaths, duration);
        }
    }
}
