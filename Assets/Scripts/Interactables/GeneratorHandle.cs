
using Redsilver2.Core.Interactables;
using Redsilver2.Core.Items;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Generator {
    [RequireComponent(typeof(BoxCollider))]
    public class GeneratorHandle : Interactable
    {
        [Space]
        [SerializeField] private Transform handle;

        [Space]
        [SerializeField] private Quaternion generatorOnRotation;
        [SerializeField] private Quaternion generatorOffRotation;

        private bool      canInteract = true;
        private Generator generator;
        private Collider  collider;

        protected override void Awake()
        {
            base.Awake();
            collider = GetComponent<Collider>();
            generator = Generator.Instance;

            AddOnInteractEvent(isInteracting =>
            {
                if (isInteracting && generator.Durability.CurrentValue >= 0f)
                {
                    StartCoroutine(RotateHandle(generatorOnRotation, 0.5f));
                    generator.Activate();
                }
                else
                {
                    StartCoroutine(RotateHandle(generatorOffRotation, 0.5f));
                    generator.Desactivate();
                }
            });

            generator.AddOnInteractEvent(isInteracting =>
            {
                collider.enabled = isInteracting;
            });

            generator.Durability.AddOnValueChangedEvent(value =>
            {
                if(value <= 0f)
                {
                    isInteracting = false;
                    Interact();
                }
            });

            collider.enabled = false;
        }

        public override void Interact()
        {
            if (canInteract)
            {
                base.Interact();
            }
        }

        private IEnumerator RotateHandle(Quaternion rotation, float duration)
        {
            canInteract = false;
            yield return handle.LerpLocalRotation(rotation, duration);
            canInteract = true;
        }
    }
}
