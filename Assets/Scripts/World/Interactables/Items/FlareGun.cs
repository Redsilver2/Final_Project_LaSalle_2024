using UnityEngine;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Items
{
    sealed class FlareGun : RangedWeapon
    {
        protected override void Fire(InputAction.CallbackContext context)
        {
            if (context.performed && !IsMagEmpty())
            {
                base.Fire(context);
                RemoveAmmoInMag(1);
                itemAnimationController.PlayAnimation("Shoot");
            }
        }
        protected override void Reload(InputAction.CallbackContext context)
        {
            if (context.performed && IsMagEmpty() && !IsStashEmpty())
            {
                itemAnimationController.PlayAnimation("Reload");
            }
        }

        protected override void OnReloadEvent(RangedWeapon rangedWeapon)
        {
            if (rangedWeapon == this)
            {
                AddAmmoInMag(1);
                RemoveAmmoInStash(1);
                PlaySound(reloadClip);
            }
        }

        protected override void FireAnimationEvent()
        {
            FlareBullet bullet = (FlareBullet)GetBulletFromPool();
            Debug.LogWarning("Bullet: " + bullet);

            if (bullet != null)
            {
                bullet.gameObject.SetActive(true);
                bullet.Fire(bulletSpawnTransform);
                PlaySound(fireClip);
            }
        }
    }
}
