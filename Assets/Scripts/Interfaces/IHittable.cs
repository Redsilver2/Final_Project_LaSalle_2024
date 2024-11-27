using UnityEngine;


namespace Redsilver2.Core.Hittable
{
    public interface IHittable 
    {
        public void Hit(float damageAmount, Vector3 hitPosition);
    }
}
