using Redsilver2.Core.Events;
using Redsilver2.Core.Interactables;
using Redsilver2.Core.Items;
using Redsilver2.Core.Player;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Redsilver2.Core {
    public class InteractionManager : GameObjectEvents
    {
        [SerializeField] private float interactionRayLenght = 5f;

        [Space]
        [SerializeField] private TextMeshProUGUI describableText;
        [SerializeField] private Image crosshair;


        [Space]
        [SerializeField] private Sprite defaultCrosshairIcon;

        private IEnumerator castingRayCoroutine;
        private PlayerControls.InteractionActions controls;

        private Dictionary<Collider, IDescribable> describableInstances;
        private Dictionary<Collider, Interactable> interactableInstances;


        public static InteractionManager Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            PlayerController player = PlayerController.Instance;
            InputManager inputManager = GameManager.Instance.GetComponent<InputManager>();
            base.Awake();

            controls = inputManager.PlayerControls.Interaction;


            describableInstances = new Dictionary<Collider, IDescribable>();
            interactableInstances = new Dictionary<Collider, Interactable>();

            AddOnStateChangedEvent(isEnabled =>
            {
                if (isEnabled)
                {
                    StartCastingInteractionRay();
                }
                else
                {
                    StopCastingInteractionRay();
                }
            });

            if (player != null)
            {
                player.AddOnStateChangedEvent(isEnabled =>
                {
                    enabled = isEnabled;

                    if(crosshair != null) crosshair.enabled = isEnabled;
                    if(describableText != null) describableText.enabled = isEnabled;
                });
            }


            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleLevelEvent);
            StartCastingInteractionRay();
        }

        private void StartCastingInteractionRay()
        {
            StopCastingInteractionRay();
            castingRayCoroutine = InteractionRayCoroutine();
            StartCoroutine(castingRayCoroutine);
        }

        private void StopCastingInteractionRay()
        {
            if (castingRayCoroutine != null)
            {
                StopCoroutine(castingRayCoroutine);
            }
        }

        private IEnumerator InteractionRayCoroutine()
        {
            while (true)
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactionRayLenght) && hitInfo.collider != null && !PauseManager.IsGamePaused)
                {
                    Collider collider = hitInfo.collider;
                    SetDescribableUI(collider);

                    if (controls.Interact.WasPressedThisFrame())
                    {
                        Interact(collider);
                    }
                }

                Debug.DrawRay(transform.position, transform.forward, Color.blue, 0.1f);
                yield return null;
            }
        }

        public IEnumerator MouseInteractionRayCoroutine()
        {
            controls.Enable();

            while (true)
            {
                Vector2Control position =  Mouse.current.position;
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(position.x.value, position.y.value));

                if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionRayLenght) && hitInfo.collider != null && !PauseManager.IsGamePaused)
                {
                    Collider collider = hitInfo.collider;
                    Debug.DrawRay(ray.origin, ray.direction, Color.blue);


                    if (controls.LeftClickInteract.WasPressedThisFrame())
                    {
                        Debug.Log("Left Click");
                        Interact(collider);
                    }
                }

                yield return null;
            }
        }


        private void SetDescribableUI(Collider collider)
        {
            string describableName = string.Empty;
            Sprite describableIcon = null;

            if (TryGetDescribable(collider, out IDescribable describable))
            {
                describableName = describable.GetName();
                describableIcon = describable.GetIcon();
            }

            if (describableText != null)
            {
                describableText.text = describableName;
            }

            if (crosshair != null)
            {
                Sprite sprite = describableIcon;

                if (sprite == null)
                {
                    sprite = defaultCrosshairIcon;

                    if (defaultCrosshairIcon == null)
                    {
                        crosshair.enabled = false;
                    }
                }
                else
                {
                    crosshair.enabled = true;
                }

                crosshair.sprite = sprite;
            }
        }

        private void Interact(Collider collider) 
        {
            if (TryGetInteractable(collider, out Interactable interactable))
            {
                interactable?.Interact();
                Debug.Log(interactable);
            }
        }

        public void AddDescribableInstance(Collider collider, IDescribable describable)
        {
            if (!describableInstances.ContainsKey(collider) && collider != null && describable != null)
            {
                describableInstances.Add(collider, describable);
            }
        }

        public void RemoveDescribableInstance(Collider collider)
        {
            if (describableInstances.ContainsKey(collider))
            {
                describableInstances.Remove(collider);
            }
        }

        public void AddInteractableInstance(Collider collider, Interactable interactable)
        {
            if (!interactableInstances.ContainsKey(collider) && collider != null && interactable != null)
            {
                interactableInstances.Add(collider, interactable);
                AddDescribableInstance(collider, interactable);
            }
        }

        public void RemoveInteractableInstance(Collider collider)
        {
            if (interactableInstances.ContainsKey(collider))
            {
                interactableInstances.Remove(collider);
                RemoveDescribableInstance(collider);
            }
        }

        public bool TryGetDescribable(Collider collider, out IDescribable describable)
        {
            if (collider != null && describableInstances.TryGetValue(collider, out describable))
            {
                return true;
            }

            describable = null;
            return false;
        }

        public bool TryGetInteractable(Collider collider, out Interactable interactable)
        {
            if (collider != null && interactableInstances.TryGetValue(collider, out interactable))
            {
                return true;
            }

            interactable = null;
            return false;
        }

        public Interactable[] GetInteractables()
        {
            List<Interactable> result = new List<Interactable>();

            foreach (KeyValuePair<Collider, Interactable> keyValuePair  in interactableInstances)
            {
                Debug.LogWarning(keyValuePair.Value);

                if (keyValuePair.Value != null)
                {
                    result.Add(keyValuePair.Value);
                }
            }

            return result.ToArray();
        }

        public Item[] GetItems()
        {
            List<Item> result = new List<Item>();

            Debug.Log(GetInteractables());

            foreach (Interactable interactable in GetInteractables())
            {
                Item item = interactable.GetComponent<Item>();

                if (item != null)
                {
                    result.Add(item);
                }
            }

            //Debug.LogWarning("Items: " + result.Count); 

            return result.ToArray();
        }

        public void ClearInstancesDictonnaries()
        {
            foreach (KeyValuePair<Collider, IDescribable> keyValue in describableInstances)
            {
                if(keyValue.Key == null)
                {
                    describableInstances.Remove(keyValue.Key);
                }
            }

            foreach (KeyValuePair<Collider, Interactable> keyValue in interactableInstances)
            {
                if (keyValue.Key == null)
                {
                    describableInstances.Remove(keyValue.Key);
                }
            }
        }

        private void OnLoadSingleLevelEvent(int levelIndex)
        {
            ClearInstancesDictonnaries();
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleLevelEvent);
        }
    }
}
