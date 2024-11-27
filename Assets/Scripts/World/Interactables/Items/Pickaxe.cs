using Redsilver2.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Redsilver2.Core
{
    public class Pickaxe : MeleeWeapon
    {
        [Space]
        [SerializeField] private float pickaxePower;
        private static Dictionary<Collider, IMinable> minableInstances = new Dictionary<Collider, IMinable>();

        public override void OnHitColliderEvent(Collider collider, float damage)
        {
            if (minableInstances.TryGetValue(collider, out IMinable minable))
            {
                Debug.Log(minable);
                minable.Mine(damage, pickaxePower);
            }
        }

        public static void AddMinableInstance(Collider collider, IMinable minable)
        {
            if (!minableInstances.ContainsKey(collider))
            {
                minableInstances.Add(collider, minable);
            }
        }
    }
}
