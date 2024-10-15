using UnityEngine;

namespace Redsilver2.Core.Items
{
    public abstract class RangedWeapon : SellableItem
    {
        [SerializeField] private uint maxAmmoAmountInStash = 3;
        [SerializeField] private uint maxAmmoAmountInMag   = 1;

        [Space]
        [SerializeField] protected AudioClip fireClip;
        [SerializeField] protected AudioClip reloadClip;

        private uint ammoAmountInStash;
        private uint ammoAmountInMag;

        protected abstract void Fire();
        protected abstract void Reload();

        public void AddAmmoInStash(uint ammoAmount)
        {
            ammoAmountInStash += ammoAmount;
        }

        protected void AddAmmoInMag()
        {
            if(ammoAmountInMag < maxAmmoAmountInMag)
            {
                ammoAmountInMag++;
            }
        }
    }
}
