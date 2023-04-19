using System;
using UnityEngine;

namespace VGDevs
{
    public class UIContentSection : UIItem
    {
        [Header("Content Settings")]
        [SerializeField] private UIContentSectionBehaviours m_contentSectionBehaviours = UIContentSectionBehaviours.HideOnAwake;
        
        public UIContentSectionBehaviours ContentSectionBehaviours => m_contentSectionBehaviours;

        protected override void OnInit()
        {
            if (m_contentSectionBehaviours.HasFlag(UIContentSectionBehaviours.HideOnAwake))
            {
                m_visualRoots.Disable();
            }
            base.OnInit();
        }
    }

    [Flags]
    public enum UIContentSectionBehaviours
    {
        None = 0,
        HideOnAwake = 1 << 3,
    }
}
