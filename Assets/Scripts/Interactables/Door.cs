using Redsilver2.Core.Items;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public class Door : Interactable
    {
        [Space]
        [SerializeField] private Quaternion desiredRotation;

        [SerializeField] private string keyName;
        [SerializeField] private float rotationDuration;

        private bool canRotate =  true;
        private bool isUnlocked = true;
        private Quaternion originalRotation;
        private IEnumerator rotationCoroutine;

        protected override void Awake()
        {
            base.Awake();
            originalRotation = transform.localRotation;

            if (keyName != string.Empty)
            {
                isUnlocked = false;
            }

            AddOnInteractEvent(isInteracting =>
            {
                if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
                rotationCoroutine = Rotate(isInteracting ? desiredRotation : originalRotation, rotationDuration);
                StartCoroutine(rotationCoroutine);
            });
        }

        public void SetLockState(Key key)
        {
            if (key != null && keyName != string.Empty)
            {
                if (keyName.ToLower().Contains(key.InteractableName.ToLower()))
                {
                    isUnlocked = !isUnlocked;
                }
            }
        }

        public override void Interact()
        {
            if (isUnlocked && canRotate)
            {
                base.Interact();
            }
        }

        public override string GetName()
        {
            return base.GetName() + (isUnlocked ? "Unlocked" : "Locked");
        }

        private IEnumerator Rotate(Quaternion rotation, float duration)
        {
            canRotate = false;
            yield return transform.LerpLocalRotation(rotation, duration);
            canRotate = true;
        }
    }
}
