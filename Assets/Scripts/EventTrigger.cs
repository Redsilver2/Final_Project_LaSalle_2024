using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnterOnce;
    [SerializeField] private UnityEvent onTriggerEnter;
    private bool hasEnteredOnce = false;

    private void Awake()
    {
        if(TryGetComponent(out Collider collider))
        {
            collider.isTrigger = true;
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasEnteredOnce)
            {
                hasEnteredOnce     = true;
                onTriggerEnterOnce.Invoke(); 
            }

            onTriggerEnter.Invoke();
        }
    }
}
