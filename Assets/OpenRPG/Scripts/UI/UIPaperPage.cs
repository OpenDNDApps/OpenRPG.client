using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VGDevs;

namespace ORC
{
    public class UIPaperPage : UIWindow
    {
        [SerializeField] private ScriptableData m_data;

        [SerializeField] private TMP_Text m_content;

        protected override void OnInit()
        {
            base.OnInit();
            m_content.text = m_data.DataByEdition[0].Content;
        }

        // TODO: Add a way to set the page style
        // TODO: Add a way to set the page data
    }
}
