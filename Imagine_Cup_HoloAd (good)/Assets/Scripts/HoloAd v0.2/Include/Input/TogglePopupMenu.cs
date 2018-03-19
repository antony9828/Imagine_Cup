// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TogglePopupMenu : MonoBehaviour
    {

        [SerializeField]
        public PopupMenu popupMenu = null;

        [SerializeField]
        public TestButton button = null;

        private void Awake()
        {
            if (button)
            {
                button.Activated += ShowPopup;
            }
        }

        private void OnDisable()
        {
            if (button)
            {
                button.Activated -= ShowPopup;
            }
        }


        private int togglePopup = 0;
        private void ShowPopup(TestButton source)
        {
            if (popupMenu != null)
            {
                togglePopup++;
                switch(togglePopup)
                {
                    case 1:
                        if (popupMenu.CurrentPopupState == PopupMenu.PopupState.Closed)
                        {
                            popupMenu.Show();
                            Debug.Log("dickdickdick");
                            StartCoroutine(WaitForPopupToClose());
                        }
                        break;
                    case 2:
                        //СЮДА ДОБАВИТЬ КОД ДЛЯ ПЕРЕХОДА НА САЙТ
                        popupMenu.Dismiss();
                        togglePopup = 0;
                        break;                    
                }
            }
        }

        private IEnumerator WaitForPopupToClose()
        {
            if (popupMenu)
            {
                while (popupMenu.CurrentPopupState == PopupMenu.PopupState.Open)
                {
                    yield return null;
                }
            }

            if (button)
            {
                button.Selected = false;
            }
        }
    }
}