using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace VGDevs
{
    public class UIWindow : UIItem
    {
        [Header("Generic Window Settings")]
        [SerializeField] private UISectionType m_uiSectionType = UISectionType.Default;
        [SerializeField] protected UIButton m_closeButton;
        [SerializeField] private bool m_hideOnAwake = true;
        [SerializeField] private bool m_autoHideWhenCloseCalled = true;
        
        [Header("Notch Settings")]
        [SerializeField] protected NotchBehaviour m_notchBehaviour = NotchBehaviour.Ignore;
        private Canvas m_canvas;
        
        [Header("Popup Settings")] 
        [SerializeField] protected bool m_autoGenerateInputBlocker = false;
        [SerializeField] protected UIInputBlocker m_inputBlocker;
        [SerializeField] protected InputBlockClickBehaviour m_inputBlockClickBehaviour = InputBlockClickBehaviour.AnimatedHide;

        public UISectionType SectionType => m_uiSectionType;
        public NotchBehaviour NotchBehaviour => m_notchBehaviour;

        public InputBlockClickBehaviour InputBlockerBehaviour => m_inputBlockClickBehaviour;

        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null)
                    m_canvas = UIRuntime.GetCanvasOfType(m_uiSectionType);

                return m_canvas;
            }
        }

        public bool IsTopLevelWindow => this.Canvas.transform == transform.parent;

        public event Action OnCloseTrigger;
        
        protected override void OnInit()
        {
            if (m_hideOnAwake)
            {
                m_visualRoots.Disable();
            }

            if (m_closeButton != null)
            {
                m_closeButton.OnClick = OnCloseButtonClick;
            }

            if (m_autoGenerateInputBlocker && m_inputBlocker == null)
            {
                m_inputBlocker = AddInputBlocker(this);
            }

            this.ApplySafeArea();
        }

        protected void SetButtonAsCloseButton(UIButton closeButton)
        {
            if (closeButton != default)
            {
                closeButton.OnClick += OnCloseButtonClick;
            }
        }

        public virtual void OnCloseButtonClick()
        {
            if(m_autoHideWhenCloseCalled)
                AnimatedHide();
            
            OnCloseTrigger?.Invoke();
        }
        
        public override void Show()
        {
            base.Show();
            if (m_inputBlocker != null)
            {
                m_inputBlocker.Enable();
            }
        }
    
        public override void AnimatedShow()
        {
            if (m_inputBlocker != null)
            {
                m_inputBlocker.AnimatedShow();
            }
            base.AnimatedShow();
        }

        public override void Hide()
        {
            base.Hide();
            m_visualRoots.Disable();
            if (m_inputBlocker != null)
            {
                m_inputBlocker.Disable();
            }
        }
        
        public override void AnimatedHide()
        {
            base.AnimatedHide();
            if (m_inputBlocker != null)
            {
                m_inputBlocker.AnimatedHide();
            }
        }

        protected override void OnDestroy()
        {
            UIRuntime.NotifyWindowDestroy(this);
        }

        /// <summary>
        /// Changes the vertical position of the VisualRoot.
        /// </summary>
        /// <param name="bottomToTop">0f is Bottom, 1f is top.</param>
        /// <param name="changePivot">Also update the pivot position to match the given value, recommended.</param>
        public void SetVerticalPosition(float bottomToTop = 0.5f, bool changePivot = true)
        {
            RectTransform root = (RectTransform) m_visualRoots.First().transform;

            root.anchorMax = new Vector2(root.anchorMax.x, bottomToTop);
            root.anchorMin = new Vector2(root.anchorMin.x, bottomToTop);

            if (changePivot)
            {
                root.pivot = new Vector2(root.pivot.x, bottomToTop);
            }

            root.anchoredPosition = Vector2.zero;
        }
        
        public UIInputBlocker AddInputBlocker(UIWindow window, UIInputBlocker inputBlocker = null)
        {
            if (inputBlocker == null)
                inputBlocker = GameResources.Settings.UI.Default.InputBlocker;
            
            UIInputBlocker newInputBlocker = Instantiate(inputBlocker, window.VisualRoots.First().transform);
            var ibTransform = (RectTransform)newInputBlocker.transform;
            ibTransform.localPosition = Vector3.zero;
            ibTransform.localScale = Vector3.one;
            ibTransform.SetAsFirstSibling();
            newInputBlocker.Disable();

            return newInputBlocker;
        }

        public virtual void SetInputBlockerBehaviour(InputBlockClickBehaviour newBehaviour)
        {
            m_inputBlockClickBehaviour = newBehaviour;
        }

        public virtual void OnInputBlockerClick()
        {
            switch (m_inputBlockClickBehaviour)
            {
                case InputBlockClickBehaviour.Hide:
                    Hide();
                    return;
                case InputBlockClickBehaviour.AnimatedHide:
                    AnimatedHide();
                    return;
                case InputBlockClickBehaviour.Show:
                    Show();
                    return;
                case InputBlockClickBehaviour.AnimatedShow:
                    AnimatedShow();
                    return;
                case InputBlockClickBehaviour.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public enum InputBlockClickBehaviour
        {
            None,
            Hide,
            AnimatedHide,
            Show,
            AnimatedShow,
        }
    }
}