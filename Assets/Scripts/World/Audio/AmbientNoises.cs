using UnityEngine;

namespace Redsilver2.Core.Audio
{
    public class AmbientNoises : AudioSystem
    {
        [Space]
        [SerializeField] private AudioClip startingClip;

        protected override void Awake()
        {
            base.Awake();
            SetCurrentClip(startingClip);
        }
    }
}
