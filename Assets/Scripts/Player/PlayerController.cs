using Redsilver2.Core.Audio;
using Redsilver2.Core.Events;
using Redsilver2.Core.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Stamina))]
    [RequireComponent(typeof(FootstepAudioHandler))]

    [RequireComponent(typeof(PlayerWeight))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : GameObjectEvents
    {
        [SerializeField] private float defaultWalkSpeed;
        [SerializeField] private float defaultRunSpeed;

        [Space]
        [SerializeField] private float defaultGravityForce;
        [SerializeField] private float fallingGravityForce;

        [Space]
        [SerializeField] private float maxGroundCheckRayLenght;

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
        private FootstepAudioHandler footstepAudio;
     
        private PlayerWeight        weight;
        private CharacterController character;
        private PlayerControls      controls;

        private UnityEvent<PlayerController> onMovementMotionChanged = new UnityEvent<PlayerController>();
        private UnityEvent<PlayerController> onFootstepSoundPlayed   = new UnityEvent<PlayerController>();

        public float DefaultWalkSpeed => defaultWalkSpeed;
        public float DefaultRunSpeed => defaultRunSpeed;
        public bool IsGrounded => isGrounded;
        public bool IsRunning => isRunning;
        public Vector2 InputMotion => inputMotion;
        public Health Health   => health;
        public Stamina Stamina => stamina; 
        public PlayerWeight Weight => weight;
        public PlayerControls Controls => controls;
        public CharacterController Character => character;  
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        protected override void Start()
        {
            base.Start();

            health         = GetComponent<Health>();
            stamina        = GetComponent<Stamina>();

            character      = GetComponent<CharacterController>();
            weight         = GetComponent<PlayerWeight>();
            footstepAudio  = GetComponent<FootstepAudioHandler>();

            controls = new PlayerControls();
           
            if (health != null)
            {
                health.AddOnDeathEvent(health =>
                {
                    enabled = false;
                });
            }
            if (stamina != null)
            {
                stamina.AddOnValueChangedEvent((handler, isValueIncreasing) =>
                {
                    float percentage = handler.PercentageValue;

                    if (percentage <= 0f)
                    {
                        canRun = false;
                    }
                    else if (percentage >= 0.2f && !canRun)
                    {
                        canRun = true;
                    }
                });
            }

            AddOnFootstepSoundPlayedEvent(Player =>
            {
                if (Player.isRunning)
                {
                    Debug.Log("Player is running and making loud noises");
                }
                else
                {
                    Debug.Log("Player is slow and making small noises");
                }

                Debug.DrawRay(Player.transform.position, Vector3.down, Color.red, 10f);
            });
            AddOnStateChangedEvent(isEnabled =>
            {
                if(controls != null)
                {
                    if (isEnabled)
                    {
                        controls?.Enable();
                    }
                    else
                    {
                        controls?.Disable();
                    }
                }
            });

            currentGravityForce = defaultGravityForce;

            isRunning          = false;
            isGrounded         = false;
            canPlayLandingClip = false;

            controls.Enable();
            GameManager.SetCursorVisibility(false);
        }

        private void Update()
        {
            inputMotion = controls.Movement.Move.ReadValue<Vector2>();
            isGrounded  = GroundCheck(out currentGroundTag);

            if (isGrounded)
            {
                if (inputMotion.magnitude > 0.5f && canRun)
                {
                    isRunning = controls.Movement.Run.IsPressed();
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
                weight.AddWeight(10f);
            }
        }
        private void LateUpdate()
        {
            float targetedGravityForce  = fallingGravityForce;
            Vector3 characterMotion;

            if(isGrounded)
            {
                bool playGroundSound = false;
                bool canPlaySound    = false;

                targetedGravityForce = defaultGravityForce;

                if (isRunning)
                {
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
               
                if (inputMotion.magnitude > 0.5f)
                {
                    if(!canPlaySound)
                    {
                        canPlaySound = true;
                        playGroundSound = true;
                    }
                }

                if (canPlaySound) 
                {
                   footstepAudio.SetPitch(isRunning, 1f);
                   footstepAudio.PlayFootstepSound(currentGroundTag, playGroundSound, out bool isSoundTriggered);

                   if (isSoundTriggered)
                   {
                       onFootstepSoundPlayed?.Invoke(this);
                   }
                }
            }
            else
            {
                canPlayLandingClip = true;
            }

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, weight.GetDesiredMovementSpeed(this), Time.deltaTime);

            currentGravityForce  = Mathf.Lerp(currentGravityForce, targetedGravityForce, Time.deltaTime);
            currentGravityForce  = Mathf.Clamp(currentGravityForce, defaultGravityForce, fallingGravityForce);

            characterMotion = ( transform.forward * inputMotion.y * currentMovementSpeed 
                              + transform.right   * inputMotion.x * currentMovementSpeed 
                              + transform.up      * -currentGravityForce) * Time.deltaTime;

            character.Move(characterMotion);
            onMovementMotionChanged?.Invoke(this);
        }
        private bool GroundCheck(out string groundTag)
        {
            Ray ray = new Ray(transform.position, -transform.up);
            int targetMask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxGroundCheckRayLenght, targetMask) && hitInfo.collider.gameObject != null)
            {
                groundTag = hitInfo.collider.gameObject.tag;
                return true;
            }

            groundTag = string.Empty;
            return false;
        }

        public void AddOnMovementMotionChangedEvent(UnityAction<PlayerController> action)
        {
            onMovementMotionChanged?.AddListener(action);
        }
        public void RemoveOnMovementMotionChangedEvent(UnityAction<PlayerController> action)
        {
            onMovementMotionChanged?.RemoveListener(action);
        }

        public void AddOnFootstepSoundPlayedEvent(UnityAction<PlayerController> action)
        {
            onFootstepSoundPlayed?.AddListener(action);
        }
        public void RemoveOnFootstepSoundPlayedEvent(UnityAction<PlayerController> action)
        {
            onFootstepSoundPlayed?.RemoveListener(action);
        }


        private void OnEnable()
        {
            InvokeStateChangedEvent(true);
        }
        private void OnDisable()
        {
            InvokeStateChangedEvent(false);
        }
    }
}
