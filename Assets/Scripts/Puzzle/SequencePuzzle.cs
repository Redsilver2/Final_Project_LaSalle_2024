using Redsilver2.Core.Counters;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Interactables
{
    public abstract class SequencePuzzle : Puzzle
    {
        [SerializeField] private float sequenceDuration;
        private bool isSequenceStarted = false;
        private IEnumerator sequenceCoroutine;

        public void StartCountdown(UnityAction<float> onValueChanged, UnityAction onFinished)
        {
            if (!isSequenceStarted)
            {
                isSequenceStarted = true;
                StopSequence();
               
                sequenceCoroutine = SequenceCoroutine(onValueChanged, onFinished);
                StartCoroutine(sequenceCoroutine);
            }
        }

        public void StopSequence()
        {
            if (isSequenceStarted)
            {
                StopCoroutine(sequenceCoroutine);
                isSequenceStarted = false;
            }
        }

        private IEnumerator SequenceCoroutine(UnityAction<float> onValueChanged, UnityAction onFinished)
        {
            yield return Counter.WaitForSeconds(sequenceDuration, onValueChanged, onFinished);
        }
    }
}
