using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class KeypadButton : Interactable
    {
        [SerializeField] private Keypad    keypad;

        [Space]
        [SerializeField] private char  inputCode;
        [SerializeField] private float pushPositionY;

        private bool     canClickButton = true;
        private Vector3  originalPosition;

        protected override void Awake()
        {
            base.Awake();
            Collider collider = GetComponent<Collider>();
            originalPosition = transform.localPosition;

            if (keypad != null)
            {
                keypad.ClampedController.AddOnPathFollowStartedEvent(() =>
                {
                    if (!keypad.IsInteracting)
                    {
                        collider.enabled = false;
                    }
                });

                keypad.ClampedController.AddOnPathFollowCompletedEvent(() =>
                {
                    if (keypad.IsInteracting)
                    {
                        collider.enabled = true;
                    }
                });
                AddOnInteractEvent(isInteracting =>
                {
                    if (canClickButton)
                    {
                        StartCoroutine(PlayButtonAnimation());
                        OnInteractKeypadButtonEvent();
                    }
                });
            }
        }

        public virtual void OnInteractKeypadButtonEvent()
        {
            if (keypad != null)
            {
                keypad.InputCode(inputCode);
                keypad.PlaySound();
            }
        }

        private IEnumerator PlayButtonAnimation()
        {
            canClickButton = false;
            yield return transform.LerpLocalPosition(transform.localPosition + Vector3.down * pushPositionY, 0.5f);
            yield return transform.LerpLocalPosition(originalPosition, 0.5f);
            canClickButton = true;
        }

    }
}
