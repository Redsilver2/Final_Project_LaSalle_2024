using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Interactables {

    [RequireComponent(typeof(MeshRenderer))]
    public class ColorSequenceButton : Interactable
    {
        [SerializeField] private ColorSequencePuzzle sequencePuzzle;

        [Space]
        [SerializeField] private Material clickMaterial;

        private Material defaultMaterial;
        private MeshRenderer meshRenderer;


        private bool canPushButton = false;

        protected override void Awake()
        {
            base.Awake();
            meshRenderer = GetComponent<MeshRenderer>();
            defaultMaterial = meshRenderer.material;

            if (sequencePuzzle != null)
            {
                sequencePuzzle.AddButton(this);

                AddOnInteractEvent(isInteracting =>
                {
                    sequencePuzzle.VerifyPattern(this);
                });
            }

            canPushButton = true;
        }

        public void ResetColor()
        {
            meshRenderer.material = defaultMaterial;
        }

        public IEnumerator ShowColors(float duration)
        {
            meshRenderer.material = clickMaterial;
            yield return Counter.WaitForSeconds(duration);
            ResetColor();
        }



        public override void Interact()
        {
            if (canPushButton)
            {
                base.Interact();
            }
        }
    }
}
