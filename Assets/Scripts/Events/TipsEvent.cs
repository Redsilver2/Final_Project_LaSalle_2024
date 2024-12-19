using UnityEngine;

namespace Redsilver2.Core.Events
{
    [CreateAssetMenu(fileName = "New Tips Event", menuName = "Events/Tip")]
    public class TipsEvent : GameEvent
    {
        [SerializeField] private string[] messages;

        public override void Execute()
        {
            TipsManager tipsManager = TipsManager.Instance;

            if (tipsManager != null) 
            {
                tipsManager.PlayTip(messages);
            }
        }
    }
}
