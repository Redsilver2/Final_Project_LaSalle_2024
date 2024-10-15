using Redsilver2.Core.Economy;
using Redsilver2.Core.Interactables;
using Redsilver2.Core.Motion;
using Redsilver2.Core.Player;
using UnityEngine;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class Item : Interactable, IWeighable
    {
        [SerializeField] protected Vector3 positionInHand;
        [SerializeField] private   Vector3 rotationInHand;

        [Space]
        [SerializeField] protected float weight;

        [Space]
        [SerializeField] protected RuntimeAnimatorController animatorController;

        [Space]
        [SerializeField] private PlayerHandMotionSetting motionSetting;

        [Space]
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip equipClip;
        [SerializeField] private AudioClip unequipClip;
        [SerializeField] private AudioClip landClip;

        private MeshFilter   meshFilter;
        private MeshRenderer renderer;
        private Collider     collider;

        protected PlayerInventory inventory;

        private Animator     animator;
        private AudioSource  source;

        public MeshFilter MeshFilter => meshFilter;
        public PlayerHandMotionSetting MotionSetting => motionSetting;
        public RuntimeAnimatorController AnimatorController => animatorController;


        protected override void Start()
        {
            base.Start();

            meshFilter = GetComponent<MeshFilter>();
            renderer   = GetComponent<MeshRenderer>();
            collider   = GetComponent<Collider>();

            if(Camera.main.TryGetComponent(out PlayerInventory inventory))
            {
                this.inventory = inventory;
            }

           // AddOnInteract();
           // AddOnInteractOnce();
        }

        public void SetRotationInHand()
        {
            transform.localEulerAngles = rotationInHand;
        }

        protected void SetMeshRendererState(bool isEnabled)
        {
            if(renderer != null)
            {
                renderer.enabled = isEnabled;
            }
        }
        protected void SetColliderState(bool isEnabled)
        {
            if (collider != null)
            {
                collider.enabled = isEnabled;
            }
        }

        public void SetVisibility(bool isVisible)
        {
            SetMeshRendererState(isVisible);
            SetColliderState(false);
        }

        public void SetAnimator(Animator animator)
        {
            this.animator = animator;
        }

        public void SetSource(AudioSource source)
        {
            this.source = source;
        }

        protected void ChangeAnimation(string stateName, float crossFade)
        {
            if(crossFade < 0.2f) { crossFade = 0.2f; }
            animator.CrossFade(stateName, crossFade);
        }

        public void PlaySound(AudioClip clip)
        {
            if(clip == null && source != null)
            {
                source.clip = clip;
                source.Play();
            }
        }
        
        public float GetWeight()
        {
            return weight;
        }
    }
}
