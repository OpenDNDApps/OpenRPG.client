using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VGDevs
{
    public class UIWelcome : UIWindow
    {
        [Header("Welcome")]
        [SerializeField] private TMP_Text m_message;

        public void Build(string message)
        {
            m_message.SetText(message);
        }
    }  
}
