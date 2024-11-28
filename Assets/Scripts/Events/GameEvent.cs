using UnityEngine;

namespace Redsilver2.Core.Events
{
    public abstract class GameEvent : ScriptableObject
    {
        public abstract void Execute();
    }
}
