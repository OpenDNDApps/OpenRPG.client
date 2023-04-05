using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VGDevs
{
    public class UIButton : UIItem, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Button")] 
        [SerializeField] private Button m_button;
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private Sprite m_icon;
        
        [SerializeField] private string m_localizationKey;

        public Action OnClick;
        public Button Button => m_button;

        protected override void OnInit()
        {
            if (m_button == null)
            {
                m_button = GetComponent<Button>();
            }

            if (m_button != null)
            {
                m_button.onClick.AddListener(ClickBehaviour);
            }

            if (!string.IsNullOrEmpty(m_localizationKey))
            {
                SetLabel(m_localizationKey);
            }
        }

        public void SetLabel(string key)
        {
            if (m_label == null || string.IsNullOrEmpty(key))
                return;

            m_label.SetLocalizedText(key);
        }

        public virtual void ClickBehaviour()
        {
            OnClick?.Invoke();
        }

        public void TriggerClick()
        {
            m_button.onClick.Invoke();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_visualRoots.StartAnimation(VisualRootAnimTriggerType.OnPointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_visualRoots.StartAnimation(VisualRootAnimTriggerType.OnPointerExit);
        }
    }
}