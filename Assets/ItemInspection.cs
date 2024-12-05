using Redsilver2.Core.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Player
{
    [System.Serializable]
    public class ItemInspection
    {
        [SerializeField] private GameObject  camera;

        [Space]
        [SerializeField] private CanvasGroup     canvasGroup;
        [SerializeField] private TextMeshProUGUI itemNameDisplayer;
        [SerializeField] private Button          closeButton;

        [Space]
        [SerializeField] private Transform    rotationXBody;
        [SerializeField] private Transform    rotationYBody;

        private PlayerControls.CameraActions controls;
        private IEnumerator  uiCoroutine;

        private List<GameObject> inspectableItems;

        private bool isEnabled = false;
        private bool isInitialized = false;
        public bool IsEnabled => isEnabled;

        public void Init()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                inspectableItems = new List<GameObject>();
                controls = GameManager.Instance.GetComponent<InputManager>().PlayerControls.Camera;

                if (closeButton) 
                {
                    closeButton.onClick.AddListener(() =>
                    {
                        if (isEnabled)
                        {
                            Disable();
                        }
                    });
                }

                if (itemNameDisplayer != null) 
                {
                    itemNameDisplayer.text = string.Empty;
                }

                SetUIVisibility(false);
            }
        }

        public void SetObjectInspectionParent(GameObject gameObject) 
        {
            if(gameObject != null && !inspectableItems.Contains(gameObject))
            {
                gameObject.transform.SetParent(rotationYBody);
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.SetActive(false);
                inspectableItems.Add(gameObject);
            }
        }


        public void Enable(MonoBehaviour monoBehaviour, EquippableItem equippableItem)
        {
            if (!isEnabled && equippableItem != null)
            {
                foreach(GameObject gameObject in inspectableItems)
                {
                    if(gameObject == null)
                    {
                        continue;
                    }

                    gameObject.SetActive(false);

                    if(gameObject == equippableItem.ShowcaseClone)
                    {
                        gameObject.SetActive(true);
                    }
                }

                if (itemNameDisplayer != null)
                {
                    itemNameDisplayer.text = equippableItem.InteractableName;
                }

                isEnabled = true;
                uiCoroutine = UpdateItemRotation();
                monoBehaviour.StartCoroutine(uiCoroutine);
            }
        }

        public void Disable() 
        {
            if (isEnabled) 
            {
                isEnabled = false;
                controls.Disable();
            }
        }

        public void SetUIVisibility(bool isVisible)
        {
            if(camera != null) camera.SetActive(isVisible);
        }

        private IEnumerator UpdateItemRotation()
        {
            PlayerController playerController = PlayerController.Instance;

            if (playerController != null && canvasGroup != null)
            {
                canvasGroup.enabled      = true;
                canvasGroup.alpha        = 0f;
                playerController.enabled = false;

                SetUIVisibility(true);
                GameManager.SetCursorVisibility(true);

                rotationXBody.localEulerAngles = Vector3.zero;
                rotationYBody.localEulerAngles = Vector3.zero;

                yield return canvasGroup.FadeCanvasGroup(true, 1.5f);
                canvasGroup.enabled = false;

                controls.Enable();

                while (isEnabled)
                {
                    if (!PauseManager.IsGamePaused)
                    {
                        Vector2 motion = controls.Move.ReadValue<Vector2>() * 10f;
                        rotationXBody.Rotate(Vector3.up    * -motion.x * Time.deltaTime);
                        rotationYBody.Rotate(Vector3.right * -motion.y * Time.deltaTime);
                    }

                    yield return null;
                }

                canvasGroup.enabled = true;
                yield return canvasGroup.FadeCanvasGroup(false, 1.5f);
                canvasGroup.enabled = false;

                SetUIVisibility(false);
                GameManager.SetCursorVisibility(false);

                playerController.enabled = true;
            }

        }
    }
}
