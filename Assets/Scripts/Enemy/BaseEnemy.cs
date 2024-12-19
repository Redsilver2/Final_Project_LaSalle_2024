using UnityEngine;
using UnityEngine.AI;

namespace Redsilver2.Core.Enemy
{
    public class BaseEnemy : EnemyStateController
    {
        protected override void ChaseMove(NavMeshAgent agent, Vector3 position)
        {
            if (agent != null) 
            {
                if (Vector3.Distance(transform.position, position) >= 1.5f)
                {
                    agent.SetDestination(position);
                }
            }
        }

        protected override void PatrolMove(NavMeshAgent agent, Vector3 position)
        {
            if (agent != null)
            {
                agent.SetDestination(position);

                if (Vector3.Distance(transform.position, position) <= 0.5f) 
                {
                    SetEnemyState(EnemyState.Idol);
                }
            }
        }

        protected override void SearchMove(NavMeshAgent agent, Vector3 position)
        {
            if (agent != null)
            {
                agent.SetDestination(position);
            }
        }
    }
}
