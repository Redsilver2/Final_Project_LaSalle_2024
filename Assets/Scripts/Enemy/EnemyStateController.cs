using Redsilver2.Core.Audio;
using Redsilver2.Core.Counters;
using Redsilver2.Core.Player;
using Redsilver2.Core.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Redsilver2.Core.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyStateController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyAnimationController animationController;

        [Space]
        [SerializeField] private FootstepAudioHandler footstepAudioHandler;

        [Space]
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float movementTransitionSpeed;

        [Space]
        [SerializeField] private              Transform fieldOfViewPosition;
        [SerializeField] private              float     fieldOfViewDistance          = 50f;
        [SerializeField] private              float     fieldOfViewRotationIncrement = 50f;
        [SerializeField] private              float     enemyLoopUpdateRate        = 0.25f;
        [SerializeField][Range(1,15)] private uint      maxFieldOfViewRays;

        [Space]
        [SerializeField] private float minIdolTime;
        [SerializeField] private float maxIdolTime;

        [Space]
        [SerializeField] private float searchWaitTime;

        [Space]
        [SerializeField] private float chaseWaitTime;

        [Space]
        [SerializeField] private float maxHearingDistance;

        [Space]
        [SerializeField] private float minAggressionMultiplier;
        [SerializeField] private float maxAggressionMultiplier;

        [Space]
        [SerializeField] private float chasingAggressionValueRequirement;
        [SerializeField] private float searchAggressionValueRequirement;

        [Space]
        [SerializeField] private float attackDamage = 2;

        [Space]
        [SerializeField] private Transform[] waypoints;

        [Space]
        [SerializeField] private bool randomizeSearchPattern    = false;
        [SerializeField] private bool isSensitiveToPlayerNoises = false;
      
        private float stateWaitTime;
        private bool  isPlayerInFieldOfView;

        private float minAttackRange;
        private float maxAttackRange;

        private int   waypointIndex;
        private float aggresionValue = 0f;

        private Vector3 searchSpot;
        private Vector3 originalFieldOfViewRotation;

        private EnemyState currentState = EnemyState.Idol;
        private IEnumerator updateStateCoroutine;

        private AudioManager audioManager;
        private PlayerController controller;
        private NavMeshAgent agent;

        public EnemyState CurrentState => currentState;



        public static EnemyState HighestEnemyState 
        {
            get
            {
                if(enemiesInstances.Count > 0)
                {
                    return enemiesInstances.OrderBy(x => x.currentState).Reverse().First().currentState;
                }

                return EnemyState.Idol;
            }
        }

        private static List<EnemyStateController> enemiesInstances = new List<EnemyStateController>();


        protected virtual void Awake()
        {
            EnemyAttackAnimationData[] attackAnimationDatas = animationController.AttackAnimationDatas;

            agent        = GetComponent<NavMeshAgent>();
            audioManager = AudioManager.Instance;
            controller   = PlayerController.Instance;
         
            originalFieldOfViewRotation = fieldOfViewPosition.localEulerAngles;
            animationController.Init(animator, false);

            if (attackAnimationDatas.Length > 0)
            {
                minAttackRange = attackAnimationDatas.OrderBy(x => x.MinAttackRange).First().MinAttackRange;
                maxAttackRange = attackAnimationDatas.OrderBy(x => x.MaxAttackRange).Reverse().First().MaxAttackRange;
            }

            if (controller != null)
            {
                FootstepAudioHandler footstepAudioHandler = controller.GetComponent<FootstepAudioHandler>();
                Health health = controller.Health;

                if (footstepAudioHandler != null) {
                    footstepAudioHandler.AddOnFootstepSoundPlayedEvent(OnPlayerFootstepAudioPlayed);
                }

                if (health != null) 
                {
                    health.AddOnValueChangedEvent(value =>
                    {
                        if (value <= 0f)
                        {
                            
                            SetEnemyState(EnemyState.Idol);
                            enabled = false;
                        }
                    });
                }
            }

            enemiesInstances.Add(this);

            StartCoroutine(UpdateEnemyLoop());
            SetEnemyState(EnemyState.Patrol);
        }

        private void Update()
        {
            if (!PauseManager.IsGamePaused)
            {
                float desiredMovementSpeed = walkSpeed;
                bool isRunning = false;

                stateWaitTime -= Time.deltaTime;

                if (stateWaitTime <= 0f)
                {
                    stateWaitTime = 0f;
                }

                if (currentState == EnemyState.Chase)
                {
                    desiredMovementSpeed = runSpeed;
                    isRunning = true;
                }

                if (currentState != EnemyState.Idol)
                {
                    footstepAudioHandler.SetPitch(isRunning,movementTransitionSpeed);
                    footstepAudioHandler.PlayFootstepSound(GetGroundTag(), true);
                }

                agent.speed = Mathf.Lerp(agent.speed, desiredMovementSpeed, movementTransitionSpeed * Time.deltaTime);        
            }
        }

        private IEnumerator UpdateEnemyLoop()
        {
            while (true) 
            {
                if (!PauseManager.IsGamePaused)
                {
                    UpdateLoop();
                }
                yield return Counter.WaitForSeconds(enemyLoopUpdateRate);    
            }
        }

        private bool CanSeePlayer()
        {
            fieldOfViewPosition.localEulerAngles = originalFieldOfViewRotation;

            for (uint i = 1; i < maxFieldOfViewRays + 1; i++)
            {
                fieldOfViewPosition.localEulerAngles += Vector3.up * i * fieldOfViewRotationIncrement;

                if (Physics.Raycast(fieldOfViewPosition.position, fieldOfViewPosition.forward, out RaycastHit hit, fieldOfViewDistance) && hit.collider != null)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.DrawRay(fieldOfViewPosition.position, fieldOfViewPosition.forward, Color.green, 0.5f);
                        return true;
                    }
                }

                Debug.DrawRay(fieldOfViewPosition.position, fieldOfViewPosition.forward, Color.red, 0.5f);
            }

            return false;
        }

        private string GetGroundTag()
        {
            if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 5f, LayerMask.GetMask("Ground")) && hitInfo.collider != null){
                return hitInfo.collider.tag;
            }

            return string.Empty;
        }

        protected virtual void SetEnemyState(EnemyState state)
        {
            if (state != currentState)
            {
                if (updateStateCoroutine != null)
                {
                    StopCoroutine(updateStateCoroutine);
                }

                switch (state)
                {
                    case EnemyState.Idol:
                        updateStateCoroutine = IdolUpdate();
                        break;

                    case EnemyState.Patrol:
                        updateStateCoroutine = PatrolUpdate(waypoints[waypointIndex].position);
                        break;

                    case EnemyState.Search:
                        updateStateCoroutine = SearchUpdate();
                        break;

                    case EnemyState.Chase:
                        updateStateCoroutine = ChaseUpdate();
                    break;
                }
                currentState = state;

                audioManager.LerpEnemyMusic();
                StartCoroutine(updateStateCoroutine);
            }
        }

        private void SetEnemyState()
        {
            if (aggresionValue >= chasingAggressionValueRequirement)
            {
                SetEnemyState(EnemyState.Chase);
                aggresionValue = chasingAggressionValueRequirement;
            }
            else if (aggresionValue >= searchAggressionValueRequirement)
            {
                SetEnemyState(EnemyState.Search);
            }
        }

        private IEnumerator IdolUpdate()
        {
            animationController.PlayAnimation("Idle");
            yield return Counter.WaitForSeconds(Random.Range(minIdolTime, maxIdolTime));

            if (randomizeSearchPattern)
            {
                int lastIndex = waypointIndex;

                if (waypoints.Length > 1)
                {
                    while (waypointIndex == lastIndex)
                    {
                        waypointIndex = Random.Range(0, waypoints.Length);
                        yield return null;
                    }
                }
            }
            else
            {
                waypointIndex++;

                if(waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0;
                }
            }

            SetEnemyState(EnemyState.Patrol);
        }
        private IEnumerator PatrolUpdate(Vector3 position)
        {
            animationController.PlayAnimation("Walk");

            while (currentState == EnemyState.Patrol)
            {
                if (!PauseManager.IsGamePaused)
                {
                    if (agent.enabled)
                    {
                        PatrolMove(agent, position);
                    }
                }

                yield return null;
            }
        }
        private IEnumerator SearchUpdate()
        {
            stateWaitTime = searchWaitTime;
            animationController.PlayAnimation("Walk");

            while (currentState == EnemyState.Search)
            {
                if (!PauseManager.IsGamePaused)
                {
                    if (agent.enabled)
                    {
                        SearchMove(agent, searchSpot);
                    }

                    if(stateWaitTime <= 0f)
                    {
                        aggresionValue = 0f;
                        SetEnemyState(EnemyState.Patrol);
                        break;
                    }
                }

                yield return null;
            }
        }
        private IEnumerator ChaseUpdate()
        {
            stateWaitTime = chaseWaitTime;

            while (currentState == EnemyState.Chase)
            {
                if (!PauseManager.IsGamePaused)
                {
                    float distanceToPlayer = Vector3.Distance(controller.transform.position, transform.position);

                    if (agent.enabled)
                    {
                        ChaseMove(agent, controller.transform.position);
                    }

                    if(isPlayerInFieldOfView && distanceToPlayer >= minAttackRange &&  distanceToPlayer <= maxAttackRange)
                    {
                        yield return PlayAttackAnimation(distanceToPlayer);
                    }
                    else
                    {
                        animationController.PlayAnimation("Run");
                    }

                    if (stateWaitTime <= 0f)
                    {
                        aggresionValue = searchAggressionValueRequirement;
                        SetEnemyState(EnemyState.Search);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator PlayAttackAnimation(float distanceToPlayer)
        {
            animationController.PlayAttackAnimation(distanceToPlayer, out float waitCooldown, out bool IsAttacking);

            if (IsAttacking)
            {
                controller.Health.Damage(attackDamage);
                agent.velocity = Vector3.zero;
                agent.speed = 0f;
            }

            yield return Counter.WaitForSeconds(waitCooldown);
        }

        protected virtual void UpdateLoop()
        {
            isPlayerInFieldOfView = CanSeePlayer();

            if (isPlayerInFieldOfView)
            {
                IncreaseAggressionValue(Vector3.Distance(controller.transform.position, transform.position), fieldOfViewDistance);
            }
        }

        protected abstract void PatrolMove(NavMeshAgent agent, Vector3 position);
        protected abstract void SearchMove(NavMeshAgent agent, Vector3 position);
        protected abstract void ChaseMove(NavMeshAgent  agent, Vector3 position);

        private void OnPlayerFootstepAudioPlayed(Transform transform)
        {
            if (isSensitiveToPlayerNoises){
                float maxHearingDistance = this.maxHearingDistance;
                float distanceToPlayer   = Vector3.Distance(transform.position, this.transform.position);

                if (controller.IsRunning)
                {
                    maxHearingDistance *= 1.5f;
                }

                if(aggresionValue >= searchAggressionValueRequirement && currentState == EnemyState.Search)
                {
                    searchSpot = transform.position;
                }

                IncreaseAggressionValue(distanceToPlayer, maxHearingDistance);
            }
        }
        protected void IncreaseAggressionValue(float distanceToPlayer, float maxDistance)
        {
            if (distanceToPlayer >= 0f && distanceToPlayer <= maxDistance)
            {
                float aggressionMultiplier = Mathf.Lerp(maxAggressionMultiplier, minAggressionMultiplier, distanceToPlayer/maxDistance);
                aggresionValue += aggressionMultiplier * Time.deltaTime;

                SetEnemyState();

                if (currentState == EnemyState.Chase)
                {
                    stateWaitTime = chaseWaitTime;
                }
                else if(currentState == EnemyState.Search)
                {
                    stateWaitTime = searchWaitTime;
                }

            }
        }

        private void OnGamePausedEvent(bool isGamePaused)
        {
            agent.enabled = !isGamePaused;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                aggresionValue = chasingAggressionValueRequirement;
            }
        }

        protected virtual void OnEnable()
        {
            PauseManager.AddOnGamePausedEvent(OnGamePausedEvent);
        }
        protected virtual void OnDisable()
        {
            PauseManager.RemoveOnGamePausedEvent(OnGamePausedEvent);
        }

        public static void SetAllEnemyStates(EnemyState enemyState)
        {
            foreach(EnemyStateController stateController in enemiesInstances)
            {
                if (stateController != null)
                {
                    stateController.SetEnemyState(enemyState);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (isSensitiveToPlayerNoises)
            {
                Gizmos.DrawWireSphere(transform.position, maxHearingDistance);
            }
        }
    }
}
