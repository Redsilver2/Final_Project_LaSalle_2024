using UnityEngine;

namespace Redsilver2.Core.Stats
{
    public class Stamina : RecoverableStat
    {
        [Space]
        [SerializeField] private float defaultValueDecreaseSpeed = 1.0f;

        private float valueDecreaseSpeed;
        public float DefaultValueDecreaseSpeed => defaultValueDecreaseSpeed;

        protected override void Start()
        {
            base.Start();
            valueDecreaseSpeed = defaultValueDecreaseSpeed;
        }

        public void Decrease()
        {
            Decrease(valueDecreaseSpeed * Time.deltaTime);
        }

        public void SetDecreaseSpeed(float DecreaseSpeed)
        {
            valueDecreaseSpeed = DecreaseSpeed;
        }

    }
}
