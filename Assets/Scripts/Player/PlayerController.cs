using Redsilver2.Core.Audio;
using Redsilver2.Core.Events;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Stats;
using Redsilver2.Core.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Stamina))]
    [RequireComponent(typeof(FootstepAudioHandler))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerUI))]
    public class PlayerController : GameObjectEvents
    {
        [SerializeField] private float defaultWalkSpeed;
        [SerializeField] private float defaultRunSpeed;

        [Space]
        [SerializeField] private float defaultGravityForce;
        [SerializeField] private float fallingGravityForce;

        [Space]
        [SerializeField] private float maxGroundCheckRayLenght;

        private PlayerUI uiManager;

        private float currentMovementSpeed;
        private float currentGravityForce;

        private bool canRun = true;
        private bool isGrounded;
        private bool isRunning;
        private bool canPlayLandingClip;

        private string currentGroundTag;
        private Vector2 inputMotion;

        private Health health;
        private Stamina stamina;
        private FootstepAudioHandler footstepAudioHandler;

        private CharacterController character;
        private PlayerControls.MovementActions controls;

        private static UnityEvent<PlayerController> onMovementMotionChanged = new UnityEvent<PlayerController>();
        private bool lastEnabledState = false;

        public float DefaultWalkSpeed => defaultWalkSpeed;
        public float DefaultRunSpeed => defaultRunSpeed;
        public bool IsGrounded => isGrounded;
        public bool IsRunning => isRunning;
        public Vector2 InputMotion => inputMotion;
        public Health Health => health;
        public Stamina Stamina => stamina;

        public FootstepAudioHandler FootstepAudioHandler => footstepAudioHandler;
        public CharacterController Character => character;
        public static PlayerController Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            base.Awake();

            InputManager inputManager = GameManager.Instance.GetComponent<InputManager>();

            health               = GetComponent<Health>();
            stamina              = GetComponent<Stamina>();

            character            = GetComponent<CharacterController>();
            footstepAudioHandler = GetComponent<FootstepAudioHandler>();

            uiManager = GetComponent<PlayerUI>();

            controls = inputManager.PlayerControls.Movement;
            inputManager.SetPlayerControlsState(true);

            stamina.AddOnValueChangedEvent(value =>
            {
                float percentage = stamina.PercentageValue;

                if (percentage <= 0f)
                {
                    canRun = false;
                }
                else if (percentage >= 0.2f && !canRun)
                {
                    canRun = true;
                }
            });

            health.AddOnValueChangedEvent(value =>
            {
                if (value <= 0f)
                {
                    SceneLoaderManager.Instance.ReloadScene();
                }
            });

            health.AddOnDamagedEvent(health =>
            {
                uiManager.BloodSFX(health.CurrentValue / 0.5f, true);
            });

            health.AddOnHealedEvent(health =>
            {
                uiManager.BloodSFX(health.CurrentValue / 0.5f, false);
            });




            footstepAudioHandler.AddOnFootstepSoundPlayedEvent(transform =>
            {
                if (isRunning)
                {
                    Debug.Log("Player is running and making loud noises");
                }
                else
                {
                    Debug.Log("Player is slow and making small noises");
                }

                Debug.DrawRay(transform.position, Vector3.down, Color.red, 10f);
            });

            AddOnStateChangedEvent(isEnabled =>
            {
                inputManager.SetPlayerControlsState(isEnabled);
            });

            currentGravityForce = defaultGravityForce;

            isRunning = false;
            isGrounded = false;
            canPlayLandingClip = false;

            controls.Enable();

            GameManager.SetCursorVisibility(false);
            PauseManager.AddOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        private void Update()
        {
            inputMotion = controls.Move.ReadValue<Vector2>();
            isGrounded = GroundCheck(out currentGroundTag);

            if (isGrounded)
            {
                if (inputMotion.magnitude > 0.5f && canRun)
                {
                    isRunning = controls.Run.IsPressed();
                }
                else
                {
                    isRunning = false;
                }
            }
            else
            {
                isRunning = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                health.Damage(health.MaxValue);
            }
        }

        private void LateUpdate()
        {
            float targetedMovementSpeed = defaultWalkSpeed;
            float targetedGravityForce = fallingGravityForce;
            Vector3 characterMotion;

            if (isGrounded)
            {
                bool playGroundSound = false;
                bool canPlaySound = false;

                targetedGravityForce = defaultGravityForce;

                if (isRunning)
                {
                    targetedMovementSpeed = defaultRunSpeed;
                    stamina?.StopRecovery();
                    stamina?.Decrease();
                }
                else
                {
                    stamina?.StartRecovery();
                }

                if (canPlayLandingClip)
                {
                    playGroundSound = false;
                    canPlaySound = true;
                    canPlayLandingClip = false;
                }

                if (inputMotion.magnitude > 0.5f && !canPlaySound)
                {
                    canPlaySound = true;
                    playGroundSound = true;
                }

                if (canPlaySound)
                {
                    footstepAudioHandler.SetPitch(isRunning, 1f);
                    footstepAudioHandler.PlayFootstepSound(currentGroundTag, playGroundSound);
                }
            }
            else
            {
                canPlayLandingClip = true;
            }

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, targetedMovementSpeed, Time.deltaTime);

            currentGravityForce = Mathf.Lerp(currentGravityForce, targetedGravityForce, Time.deltaTime);
            currentGravityForce = Mathf.Clamp(currentGravityForce, defaultGravityForce, fallingGravityForce);

            characterMotion = (transform.forward * inputMotion.y         * currentMovementSpeed
                              + transform.right  * inputMotion.x         * currentMovementSpeed
                              + transform.up     * -currentGravityForce) * Time.deltaTime;

            character.Move(characterMotion);
            onMovementMotionChanged?.Invoke(this);
        }
        private bool GroundCheck(out string groundTag)
        {
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxGroundCheckRayLenght, LayerMask.GetMask("Ground")) && hitInfo.collider.gameObject != null)
            {
                groundTag = hitInfo.collider.gameObject.tag;
                return true;
            }

            groundTag = string.Empty;
            return false;
        }

        public static void AddOnMovementMotionChangedEvent(UnityAction<PlayerController> action)
        {
            onMovementMotionChanged?.AddListener(action);
        }
        public static void RemoveOnMovementMotionChangedEvent(UnityAction<PlayerController> action)
        {
            onMovementMotionChanged?.RemoveListener(action);
        }

        private void OnLoadSingleSceneEvent(int levelIndex)
        {
            enabled = false;
            PauseManager.RemoveOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        private void OnGamePausedEvent(bool isPaused)
        {
            if (!isPaused)
            {
                if (lastEnabledState)
                {
                    enabled = true;
                }
            }
            else
            {
                lastEnabledState = enabled;
                enabled = false;
            }
        }
    }
}
