using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public class UIItemBase : MonoBehaviour
    {
        protected UIWindow m_window;
        protected UIVisualRoot m_visualRoot;
        protected bool m_initialized = false;

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

        public UIVisualRoot VisualRoot
        {
            get
            {
                if (m_visualRoot == null)
                    m_visualRoot = GetComponent<UIVisualRoot>();
                if (m_window == null)
                    m_visualRoot = GetComponentInParent<UIVisualRoot>();
                
                if (m_visualRoot == null)
                    Debug.LogError($"VisualRoot not found on '{this.name}' gameObject");
                
                return m_visualRoot;
            }
        }
        
        protected virtual void Awake()
        {
            Init();
        }

        public virtual bool Init()
        {
            if (m_initialized) 
                return false;

            OnInit();

            m_initialized = true;
            return m_initialized;
        }

        protected virtual void OnInit() { }
    }
}
