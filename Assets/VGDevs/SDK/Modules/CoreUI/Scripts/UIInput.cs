using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VGDevs
{
    public class UIInput : UIItem
    {
        [Header("UI Input")] 
        [SerializeField] private TMP_InputField m_inputField;
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private TMP_Text m_placeholder;
        
        [SerializeField] private string m_localizationKey;
        [SerializeField] private string m_placeHolderLocalizationKey;

        public TMP_InputField InputField => m_inputField;

        public string Value
        {
            get => m_inputField != null ? m_inputField.text : string.Empty;
            set
            {
                if (m_inputField == null)
                    return;

                m_inputField.text = value;
            }
        }

        protected override void OnInit()
        {
            SetLabel(m_localizationKey);
            SetPlaceholder(m_placeHolderLocalizationKey);
        }

        public void SetLabel(string newLabel)
        {
            if (m_label == null)
                return;

            m_label.SetText(newLabel);
        }

        public void SetPlaceholder(string newPlaceholder)
        {
            if (m_placeholder == null)
                return;

            m_placeholder.SetText(newPlaceholder);
        }
    }
}
