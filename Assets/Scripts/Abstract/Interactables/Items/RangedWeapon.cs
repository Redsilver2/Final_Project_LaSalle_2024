using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Items
{
    public abstract class RangedWeapon : EquippableItem
    {
        [SerializeField] protected Transform bulletSpawnTransform;

        [Space]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int amountOfBulletsToSpawn;

        [Space]
        [SerializeField] private Vector3 bulletSize;

        [Space]
        [SerializeField] private int maxAmmoAmountInStash = 3;
        [SerializeField] private int maxAmmoAmountInMag = 1;

        [Space]
        [SerializeField] protected AudioClip fireClip;
        [SerializeField] protected AudioClip reloadClip;



        private Queue<Bullet> bulletsPool;

        private int ammoAmountInStash;
        private int ammoAmountInMag;

        private static UnityEvent<RangedWeapon> onFiredWeapon  = new UnityEvent<RangedWeapon>();
        private static UnityEvent<RangedWeapon> onReloadWeapon = new UnityEvent<RangedWeapon>();

        public Vector3 BulletSize => bulletSize;

        protected override void Awake()
        {
            base.Awake();

            bulletsPool = new Queue<Bullet>();
            InstantiateBulletsInPool();

            ammoAmountInStash = maxAmmoAmountInStash;
            ammoAmountInMag   = maxAmmoAmountInMag;
        }

        public override void Equip()
        {
            base.Equip();
            EnableControls();
        }

        public override void UnEquip()
        {
            base.UnEquip();
            DisableControls();
        }

        protected abstract void OnReloadEvent(RangedWeapon rangedWeapon);
        protected abstract void FireAnimationEvent();
        protected virtual void ReloadAnimationEvent()
        {
            onReloadWeapon.Invoke(this);
        }

        protected virtual void Fire(InputAction.CallbackContext context)
        {
            onFiredWeapon.Invoke(this);
        }
        protected abstract void Reload(InputAction.CallbackContext context);

        public void AddAmmoInStash(int ammoAmount)
        {
            ammoAmount = Mathf.Abs(ammoAmount);
            ammoAmountInStash += ammoAmount;
        }
        public void RemoveAmmoInStash(int ammoAmount)
        {
            if (ammoAmount > 0)
            {
                ammoAmount *= -1;
            }

            ammoAmountInStash += ammoAmount;
        }

        protected void AddAmmoInMag(int amount)
        {
            if (ammoAmountInMag < maxAmmoAmountInMag)
            {
                ammoAmountInMag += amount;

                if (ammoAmountInMag >= maxAmmoAmountInMag)
                {
                    ammoAmountInMag = maxAmmoAmountInMag;
                }
            }
        }

        protected void RemoveAmmoInMag(int amount)
        {
            if (ammoAmountInMag - amount >= 0)
            {
                ammoAmountInMag -= amount;
            }
        }

        private async void InstantiateBulletsInPool()
        {
            if (bulletPrefab != null)
            {
                while (!bulletPrefab.didAwake) await Awaitable.NextFrameAsync();

                for (int i = 0; i < amountOfBulletsToSpawn; i++)
                {
                    Bullet bullet = Instantiate(bulletPrefab);
                    bullet.gameObject.name += " " + i;

                    bullet.Init(this);
                    bullet.transform.localScale = Vector3.one;
                    ReturnBulletToPool(bullet);
                }


                bulletPrefab.gameObject.SetActive(false);
            }
        }
        public void ReturnBulletToPool(Bullet bullet)
        {
            if (bullet != null && !bulletsPool.Contains(bullet))
            {
                Transform transform = bullet.transform;
                bullet.StopBulletDrop();
                transform.SetParent(this.transform);

                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                bulletsPool.Enqueue(bullet);
                bullet.gameObject.SetActive(false);
            }
        }
        protected Bullet GetBulletFromPool()
        {
            Bullet result = null;

            if (bulletsPool.Count > 0)
            {
                result = bulletsPool.Dequeue();
            }

            return result;
        }
        protected bool IsMagEmpty()
        {
            return ammoAmountInMag < 1;
        }
        protected bool IsStashEmpty()
        {
            return ammoAmountInStash < 1;
        }

        public bool IsStashFull()
        {
            return ammoAmountInStash >= maxAmmoAmountInStash;
        }

        protected override void SetControls(PlayerControls controls, bool isSettingControls)
        {
            if (isSettingControls)
            {
                Debug.LogWarning("HOLY SHIT MAN");
                controls.Item.LeftClickAction.performed += Fire;
                controls.Item.Reload.performed          += Reload;
                controls.Item.Enable();
            }
            else
            {
                controls.Item.LeftClickAction.performed -= Fire;
                controls.Item.Reload.performed          -= Reload;
                controls.Item.Disable();
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            RangedWeapon.AddOnReloadWeaponEvent(OnReloadEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RangedWeapon.RemoveOnReloadWeaponEvent(OnReloadEvent);
        }

        public static void AddOnFireWeaponEvent(UnityAction<RangedWeapon> action)
        {
            onFiredWeapon.AddListener(action);
        }
        public static void RemoveOnFireWeaponEvent(UnityAction<RangedWeapon> action)
        {
            onFiredWeapon.RemoveListener(action);
        }

        public static void AddOnReloadWeaponEvent(UnityAction<RangedWeapon> action)
        {
            onReloadWeapon.AddListener(action); 
        }
        public static void RemoveOnReloadWeaponEvent(UnityAction<RangedWeapon> action)
        {
            onReloadWeapon.RemoveListener(action);
        }
    }
}
