using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public class Door : Interactable
    {
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

        public override void Interact()
        {
            if (isUnlocked && canRotate)
            {
                base.Interact();
            }
        }

        private IEnumerator Rotate(Quaternion rotation, float duration)
        {
            canRotate = false;
            yield return transform.LerpLocalRotation(rotation, duration);
            canRotate = true;
        }
    }
}
