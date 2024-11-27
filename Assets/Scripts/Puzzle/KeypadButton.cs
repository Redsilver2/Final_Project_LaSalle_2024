using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class KeypadButton : Interactable
    {
        [SerializeField] private Keypad    keypad;
        [SerializeField] private AudioClip audioClip;

        [Space]
        [SerializeField] private float maxPositionY;
        [SerializeField] private char  inputCode;

        private bool     canClickButton = true;
        private Collider collider;
        private Vector3  originalPosition;

        protected override void Awake()
        {
            base.Awake();
            collider = GetComponent<Collider>();
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
            keypad.InputCode(inputCode);
        }

        private IEnumerator PlayButtonAnimation()
        {
            canClickButton = false;
            yield return transform.LerpLocalPosition(transform.localPosition + Vector3.down * maxPositionY, 0.5f);
            yield return transform.LerpLocalPosition(originalPosition, 0.5f);
            canClickButton = true;
        }

    }
}
