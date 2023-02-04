using System;
using DG.Tweening;
using UnityEngine;

namespace VGDevs
{
    public class UIItem : MonoBehaviour
    {
        [Header("UIItem Settings")] 
        [SerializeField] protected CanvasGroup m_visualRoot;

        [Header("UIItem Settings")] 
        [SerializeField] protected UIAnimation m_uiShowAnimation;
        [SerializeField] protected UIAnimation m_uiHideAnimation;
        [SerializeField] protected bool destroyOnHide = false;
        
        protected RectTransform m_rectTransform;
        private UIWindow m_window;

        public UIWindow Window
        {
            get
            {
                if (m_window == null)
                    m_window = GetComponent<UIWindow>();
                if (m_window == null)
                    m_window = GetComponentInParent<UIWindow>();
                
                if (m_window == null)
                    Debug.LogError($"Window not found on '{this.name}' gameObject");
                
                return m_window;
            }
        }

        public CanvasGroup VisualRoot
        {
            get => m_visualRoot;
            set => m_visualRoot = value;
        }

        public UIAnimation UIShowAnimation => m_uiShowAnimation != null ? m_uiShowAnimation : GameResources.Settings.UI.Default.ShowAnimation;
        public UIAnimation UIHideAnimation => m_uiHideAnimation != null ? m_uiHideAnimation : GameResources.Settings.UI.Default.HideAnimation;
        public RectTransform RectTransform => m_rectTransform;

        public event Action OnShow;
        public event Action OnAnimatedShowStart;
        public event Action OnAnimatedHideStart;
        public event Action OnHide;

        protected bool m_initialized = false;
        private bool m_hidingAnimationActive = false;

        protected virtual void Awake()
        {
            Init();
        }

        public virtual bool Init()
        {
            if (m_initialized) 
                return false;

            if (m_rectTransform == null)
            {
                m_rectTransform = transform as RectTransform;
            }

            OnInit();

            m_initialized = true;
            return m_initialized;
        }

        protected virtual void OnInit() { }

        public virtual UIAnimation StartAnimation(UIAnimation uiAnimation, UIItem target, Action onComplete = null)
        {
            return StartAnimation(uiAnimation, target.VisualRoot, onComplete);
        }

        public virtual UIAnimation StartAnimation(UIAnimation uiAnimation, CanvasGroup target, Action onComplete = null)
        {
            var animClone = Instantiate(uiAnimation);
            animClone.StartAnimation(target, onComplete);
            return animClone;
        }

        public virtual void AnimatedShow()
        {
            OnAnimatedShowStart?.Invoke();
            StartAnimation(UIShowAnimation, this, Show);
        }

        public virtual void AnimatedHide()
        {
            if (m_hidingAnimationActive)
                return;
            OnAnimatedHideStart?.Invoke();
            m_hidingAnimationActive = true;
            StartAnimation(UIHideAnimation, this, Hide);
        }

        public virtual void Show()
        {
            if (m_visualRoot != null)
            {
                m_visualRoot.Enable();
            }

            OnShow?.Invoke();
        }

        public virtual void Show(bool includeRoot)
        {
            if (includeRoot)
            {
                gameObject.SetActive(true);
            }

            Show();
        }

        public virtual void Hide()
        {
            if (m_visualRoot != null)
            {
                m_visualRoot.Disable();
            }

            OnHide?.Invoke();
            m_hidingAnimationActive = false;

            if (destroyOnHide)
            {
                this.SafeDestroy(this.gameObject);
            }
        }
        

        public virtual void Hide(bool includeRoot)
        {
            if (includeRoot)
            {
                gameObject.SetActive(false);
            }

            Hide();
        }

        public virtual void Disable(bool softDisable = false)
        {
            if (m_visualRoot != null)
            {
                m_visualRoot.Disable(softDisable);
            }
        }

        public virtual void Enable()
        {
            if (m_visualRoot != null)
            {
                m_visualRoot.Enable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_visualRoot != null)
            {
                m_visualRoot.DOKill();
            }
        }
    }
}
