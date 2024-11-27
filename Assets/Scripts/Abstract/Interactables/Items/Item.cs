using Redsilver2.Core.Interactables;
using Redsilver2.Core.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Animator))]
    public abstract class Item : Interactable, IWeighable
    {
        [Space]
        [SerializeField] protected float weight;

        [Space]
        [SerializeField] private Sprite pickupSpriteIcon;

        [Space]
        [SerializeField] private AudioClip pickupClip;

        private   MeshFilter   meshFilter;
        private   MeshRenderer renderer;
        protected Collider     collider;

        protected PlayerInventory inventory;
        protected AudioSource  source;

        private static UnityEvent<Item> onItemInteractOnce = new UnityEvent<Item>();
        private static UnityEvent<Item> onItemInteract     = new UnityEvent<Item>();

        public MeshFilter MeshFilter => meshFilter;

        protected override void Awake()
        {
            base.Awake();

            inventory  = Camera.main.GetComponent<PlayerInventory>();
            meshFilter = GetComponent<MeshFilter>();
            renderer   = GetComponent<MeshRenderer>();
            collider   = GetComponent<Collider>();

            AddOnInteractOnceEvent(isInteracting =>
            {
                onItemInteractOnce.Invoke(this);
            });

            AddOnInteractEvent(isInteracting =>
            {
                onItemInteract.Invoke(this);
            });
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
            Debug.Log("Is visible:" + isVisible);
            SetMeshRendererState(isVisible);
            SetColliderState(false);
        }

        public void SetSource(AudioSource source)
        {
            this.source = source;
        }
        protected void PlaySound(AudioClip clip)
        {
            if(clip != null && source != null)
            {
                source.clip = clip;
                source.Play();
            }
        }

        public override Sprite GetIcon()
        {
            return pickupSpriteIcon;
        }
        public float GetWeight()
        {
            return weight;
        }

        public static void AddOnItemInteractOnceEvent(UnityAction<Item> action)
        {
            onItemInteractOnce.AddListener(action); 
        }
        public static void RemoveOnItemInteractOnceEvent(UnityAction<Item> action)
        {
            onItemInteractOnce.RemoveListener(action);
        }
    }
}
