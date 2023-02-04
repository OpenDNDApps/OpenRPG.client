using System;
using TMPro;
using UnityEngine;

namespace VGDevs
{
    public class UIAlertPopup : UIWindow
    {
        [SerializeField] private TMP_Text m_message;
        
        public void Build(string message, Action onClose = null)
        {
            m_message.SetLocalizedText(message);
            if (onClose != null)
            {
                OnCloseTrigger += onClose;
            }
        }
    }
}
