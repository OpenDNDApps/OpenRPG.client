using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public class BaseBehaviour : MonoBehaviour
    {
        private bool m_initialized = false;
            
        protected void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (m_initialized)
                return;

            m_initialized = true;
            OnInit();
        }

        public virtual void Disable(bool softDisable = false)
        {
            gameObject.SetActive(false);
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        protected virtual void OnInit() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}