using Redsilver2.Core.Interactables;
using Redsilver2.Core.Player;
using System.Collections;
using UnityEngine;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SphereCollider))]
    public abstract class Bullet : Interactable
    {
        [Space]
        [SerializeField] protected float forwardVelocityPower;
        [SerializeField] protected float verticalVelocityPower;

        protected RangedWeapon ownerRangedWeapon;
        private IEnumerator bulletDropCoroutine;

        protected AudioSource source;
        protected PlayerInventory inventory;

        protected override void Awake()
        {
            base.Awake();
            inventory = Camera.main.GetComponent<PlayerInventory>();
        }

        public void Fire(Transform transform)
        {
            if(transform != null)
            {
                this.transform.parent     = null;
                this.transform.position   = transform.position;
                this.transform.rotation   = transform.rotation;
                this.transform.localScale = ownerRangedWeapon.BulletSize;

                StopBulletDrop();
                bulletDropCoroutine = BulletDropCoroutine();
                StartCoroutine(bulletDropCoroutine);
            }
        }

        public void StopBulletDrop()
        {
            if (bulletDropCoroutine != null)
            {
                StopCoroutine(bulletDropCoroutine);
            }
        }

        public void SetOwnerRangedWeapon(RangedWeapon rangedWeapon)
        {
            if(ownerRangedWeapon == null && rangedWeapon != null)
            {
                ownerRangedWeapon = rangedWeapon;
            }
        }

        public virtual void Init(RangedWeapon rangedWeapon)
        {
            source = GetComponent<AudioSource>();
            SetOwnerRangedWeapon(rangedWeapon);
        }
        protected abstract IEnumerator BulletDropCoroutine();
    }
}
