// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class PopupMenu : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private TestButton cancelButton = null;

        [SerializeField]
        private Animator rootAnimator = null;

        [SerializeField]
        private bool isModal = false;

        [SerializeField]
        private bool closeOnNonTargetedTap = false;

        /// <summary>
        /// Called when 'place' is selected
        /// </summary>
        private Action activatedCallback;

        /// <summary>
        /// Called when 'back' or 'hide' is selected
        /// </summary>
        private Action cancelledCallback;

        /// <summary>
        /// Called when the user clicks outside of the menu
        /// </summary>
        private Action deactivatedCallback;

        public PopupState CurrentPopupState = PopupState.Closed; 

        public enum PopupState { Open, Closed }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (cancelButton != null)
            {
                cancelButton.Activated += OnCancelPressed;
            }
        }

        private void OnDisable()
        {
            if (cancelButton != null)
            {
                cancelButton.Activated -= OnCancelPressed;
            }
        }

        public void Show(Action _activatedCallback = null, Action _cancelledCallback = null, Action _deactivatedCallback = null)
        {
            activatedCallback = _activatedCallback;
            cancelledCallback = _cancelledCallback;
            deactivatedCallback = _deactivatedCallback;

            gameObject.SetActive(true);
            //it will put this one to the right position
            float yAdjust = gameObject.transform.parent.gameObject.GetComponent<BoxCollider>().bounds.size.y;
            float xAdjust = gameObject.transform.parent.gameObject.GetComponent<BoxCollider>().bounds.size.z / 2f;

            Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.position - gameObject.transform.position);
            rotation.x = 0f; rotation.y = 0;
            Transform parent = gameObject.transform.parent;
            gameObject.transform.parent = null;
            gameObject.transform.rotation = rotation;
            gameObject.transform.parent = parent;
            gameObject.transform.position = gameObject.transform.parent.position + new Vector3(0, yAdjust, 0);
            //gameObject.transform.rotation = Quaternion.Euler(0, -90f, 0);
            
            CurrentPopupState = PopupState.Open;

            if (isModal)
            {
                InputManager.Instance.PushModalInputHandler(gameObject);
            }

            if (closeOnNonTargetedTap)
            {
                InputManager.Instance.PushFallbackInputHandler(gameObject);
            }

            // the visual was activated via an interaction outside of the menu, let anyone who cares know
            if (activatedCallback != null)
            {
                activatedCallback();
            }
        }

        /// <summary>
        /// Dismiss the details pane
        /// </summary>
        public void Dismiss()
        {
            if (deactivatedCallback != null)
            {
                deactivatedCallback();
            }

            if (isModal)
            {
                InputManager.Instance.PopModalInputHandler();
            }

            if (closeOnNonTargetedTap)
            {
                InputManager.Instance.PopFallbackInputHandler();
            }

            CurrentPopupState = PopupState.Closed;

            activatedCallback = null;
            cancelledCallback = null;
            deactivatedCallback = null;

            if (cancelButton)
            {
                cancelButton.Selected = false;
            }

            /*
            // Deactivates the game object
            if (rootAnimator.isInitialized)
            {
                rootAnimator.SetTrigger("Dehydrate");
            }
            else
            {
                gameObject.SetActive(false);
            }
            */
            gameObject.SetActive(false);
        }

        private void OnCancelPressed(TestButton source)
        {
            if (cancelledCallback != null)
            {
                cancelledCallback();
            }

            Dismiss();
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            if (closeOnNonTargetedTap)
            {
                Dismiss();
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (closeOnNonTargetedTap)
            {
                Dismiss();
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}