using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] protected Camera m_worldCamera;
        [SerializeField] protected Camera m_uiCamera;
        
        protected virtual void Awake()
        {
            GameResources.Runtime.MainCameraController = this;
            GameRuntime.WorldCamera = m_worldCamera;
            UIRuntime.UICamera = m_uiCamera;
        }
    }
}
