using UnityEngine;

namespace Redsilver2.Core.Motion
{
    public abstract class PlayerMotionSetting : ScriptableObject
    {
        [SerializeField] private Vector3 defaultPosition;
        [SerializeField] private Vector3 defaultRotation;

        [Space]
        [SerializeField] private float minPositionLerpSpeed;
        [SerializeField] private float maxPositionLerpSpeed;

        [Space]
        [SerializeField] private float defaultRotationLerpSpeed;


        [Space]
        [SerializeField] private float minPositionX;
        [SerializeField] private float maxPositionX;

        [Space]
        [SerializeField] private float minPositionY;
        [SerializeField] private float maxPositionY;


        [Space]
        [SerializeField] private float minRotationX;
        [SerializeField] private float maxRotationX;

        public float MinPositionLerpSpeed => minPositionLerpSpeed;
        public float MaxPositionLerpSpeed => maxPositionLerpSpeed;
      
        public float DefaultRotationLerpSpeed => defaultRotationLerpSpeed;

        public float MinPositionX => minPositionX;
        public float MaxPositionX => maxPositionX;

        public float MinPositionY => minPositionY;
        public float MaxPositionY => maxPositionY;
        public float MinRotationX => minRotationX;
        public float MaxRotationX => maxRotationX;

        public Vector3 DefaultPosition => defaultPosition;
        public Vector3 DefaultRotation => defaultRotation;
    }
}
