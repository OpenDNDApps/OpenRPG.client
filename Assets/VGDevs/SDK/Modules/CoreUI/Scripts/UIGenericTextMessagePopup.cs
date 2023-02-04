using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VGDevs
{
    public class UIGenericTextMessagePopup : UIWindow
    {
        [Header("Generic Text Message Settings")] 
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_message;
        [SerializeField] private UIButton m_primaryButton;
        [SerializeField] private UIButton m_secondaryButton;

        protected override void OnInit()
        {
            if (m_closeButton != null)
            {
                m_closeButton.Enable();
                m_closeButton.gameObject.SetActive(true);
            }

            if (m_primaryButton != null)
            {
                m_primaryButton.Disable();
                m_primaryButton.gameObject.SetActive(false);
            }

            if (m_secondaryButton != null)
            {
                m_secondaryButton.Disable();
                m_secondaryButton.gameObject.SetActive(false);
            }

            if (m_title != null)
            {
                m_title.Disable();
            }

            if (m_message != null)
            {
                m_message.Disable();
            }
        }

        public override void AnimatedHide()
        {
            if (m_primaryButton != null)
            {
                m_primaryButton.Disable();
            }

            if (m_secondaryButton != null)
            {
                m_secondaryButton.Disable();
            }

            base.AnimatedHide();
        }

        public void SetTitle(string titleKey)
        {
            if (m_title == null) return;

            m_title.SetLocalizedText(titleKey);
            m_title.Enable();
        }

        public void SetContent(string contentKey)
        {
            if (m_message == null) return;

            m_message.SetLocalizedText(contentKey);
            m_message.Enable();
        }

        public void SetPrimaryButton(string labelKey = "", Action onClick = null, bool cleanCallback = false)
        {
            SetupButton(m_primaryButton, labelKey, onClick);
        }

        public void SetSecondaryButton(string labelKey, Action onClick, bool cleanCallback = false)
        {
            SetupButton(m_secondaryButton, labelKey, onClick);
        }

        private void SetupButton(UIButton button, string labelKey, Action onClick, bool cleanCallback = false)
        {
            if (button == null || onClick == null) return;
            
            if (m_closeButton != null)
            {
                m_closeButton.Disable();
                m_closeButton.gameObject.SetActive(false);
            }
            
            if (cleanCallback)
            {
                m_primaryButton.OnClick = null;
            }
            
            m_secondaryButton.SetLabel(labelKey);
            m_secondaryButton.OnClick += onClick;
            m_secondaryButton.Enable();
            m_secondaryButton.gameObject.SetActive(true);
        }
    }
}
