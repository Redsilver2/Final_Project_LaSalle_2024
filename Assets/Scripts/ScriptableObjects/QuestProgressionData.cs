using Redsilver2.Core.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Quests
{
    public abstract class QuestProgressionData : ScriptableObject
    {
        [SerializeField] protected string questProgressionObjectName;
        [SerializeField] private   string description;
        [SerializeField] private   uint   maxProgressionValue;

        protected QuestProgression questProgression;

        public string Description       => description;
        public uint MaxProgressionValue => maxProgressionValue;

        public virtual void Enable(QuestProgression progression)
        {
            this.questProgression = progression;
        }

        public virtual void Disable()
        {
            this.questProgression = null;
        }

        public virtual string GetDescription(int progressValue)
        {
            string progress = string.Empty;

            if (maxProgressionValue > 1)
            {
                progress = $"({progressValue}/{maxProgressionValue})";
            }

            return $"{description} {progress}"; 
        }    
    }
}
