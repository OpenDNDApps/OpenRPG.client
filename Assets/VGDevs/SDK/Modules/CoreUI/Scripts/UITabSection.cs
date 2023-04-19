using System;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    [Serializable]
    public class UITabSection
    {
        [SerializeField] protected string m_id;
        [SerializeField] protected List<UIButton> m_tabButtons = new List<UIButton>();
        [SerializeField] protected List<UIVisualRoot> m_visualRoots = new List<UIVisualRoot>();
        
        private UITabWindow m_tabWindow;
        
        public string ID => m_id;
        public List<UIButton> TabButtons => m_tabButtons;
        public UITabWindow TabWindow
        {
            get => m_tabWindow;
            set => m_tabWindow = value;
        }
        public List<UIVisualRoot> VisualRoots => m_visualRoots;
        
        public event Action OnShow;
        public event Action OnAnimatedShowStart;
        public event Action OnAnimatedHideStart;
        public event Action OnHide;

        public void Init(UITabWindow owner, Action<UITabSection> onTabButtonClick)
        {
            m_tabWindow = owner;
            foreach (UIButton button in m_tabButtons)
            {
                button.SetupAsTabButton(this, onTabButtonClick);
            }
        }

        public void AnimatedShow()
        {
            m_visualRoots.Activate();
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.AnimatedShow).StartAnimation(VisualRootAnimTriggerType.AnimatedShow, Show);
            
            foreach (UIButton button in m_tabButtons)
            {
                button.IsHighlighted = true;
            }
            OnAnimatedShowStart?.Invoke();
            m_tabWindow.OnTabAnimatedShowStart?.Invoke(this);
        }

        public void Show()
        {
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.OnShowOrEnable).StartAnimation(VisualRootAnimTriggerType.OnShowOrEnable);

            foreach (UIButton button in m_tabButtons)
            {
                button.IsHighlighted = true;
            }
            OnShow?.Invoke();
            m_tabWindow.OnTabShow?.Invoke(this);
        }
        
        public void AnimatedHide()
        {
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.AnimatedHide).StartAnimation(VisualRootAnimTriggerType.AnimatedHide, Hide);
            
            foreach (UIButton button in m_tabButtons)
            {
                button.IsHighlighted = false;
            }
            OnAnimatedHideStart?.Invoke();
            m_tabWindow.OnTabAnimatedHideStart?.Invoke(this);
        }
        
        public void Hide()
        {
            foreach (UIButton button in m_tabButtons)
            {
                button.IsHighlighted = false;
            }
            OnHide?.Invoke();
            m_tabWindow.OnTabHide?.Invoke(this);
            
            m_visualRoots.Deactivate();
        }

        public void SetupSelectableSection(IUITabSelectableSection selectableSection)
        {
            OnAnimatedShowStart += selectableSection.OnSectionSelected;
            OnHide += selectableSection.OnSectionDeselected;
        }
    }
    
    public interface IUITabSelectableSection
    {
        public void OnSectionSelected();
        public void OnSectionDeselected();
    }
}