using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VGDevs;
using NaughtyAttributes;

namespace ORC
{
    public class UIPaperPage : UIWindow
    {
        [NaughtyAttributes.Expandable]
        [SerializeField] protected ScriptableData m_data;

        [Header("Paper Page Settings")]
        [SerializeField] protected Image m_solidBackground;
        [SerializeField] protected RawImage m_rawArt;
        [SerializeField] protected Image m_overlayHoledBackground;
        
        [Header("Debug")]
        [SerializeField] protected bool m_getRandomToDebug;
        [SerializeField] protected ScriptableData.OrcDataType m_debugType;
        [SerializeField] protected TMP_Text m_debugContent;

        protected override void OnInit()
        {
            base.OnInit();

            SetupPaperPage();
        }

        protected virtual void SetupPaperPage()
        {
            SetDebugContent();
        }
        
        [Button]
        private void SetDebugContent()
        {
            if (!m_getRandomToDebug) 
                return;

            if (m_data == null)
            {
                switch (m_debugType)
                {
                    case ScriptableData.OrcDataType.Unknown:
                    default:
                        return;
                    case ScriptableData.OrcDataType.Backgrounds:
                        m_data = GameResources.OrcData.Backgrounds.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Books:
                        m_data = GameResources.OrcData.SourceBooks.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Classes:
                        m_data = GameResources.OrcData.Classes.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Feats:
                        m_data = GameResources.OrcData.Feats.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Items:
                        m_data = GameResources.OrcData.Items.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Languages:
                        m_data = GameResources.OrcData.Languages.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Monsters:
                        m_data = GameResources.OrcData.Monsters.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Races:
                        m_data = GameResources.OrcData.Races.GetRandom();
                        break;
                    case ScriptableData.OrcDataType.Spells:
                        m_data = GameResources.OrcData.Spells.GetRandom();
                        break;
                } 
            }
            m_debugContent.text = m_data.DataByEdition.First().Content;
        }

        // TODO: Add a way to set the page style
        // TODO: Add a way to set the page data
    }
}
