using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace ORC
{
    public static class Board
    {
        public static Action<Vector3> OnSurfaceClick;
        public static Action<bool> OnMouseOverSurface;
        public static Action<float> OnMouseOverScroll;
        
        public static event Action<LensSettings.OverrideModes> OnPerspectiveChanged;

        public static void SetCamera(CinemachineVirtualCamera vCamera)
        {
            
            OnPerspectiveChanged?.Invoke(vCamera.m_Lens.ModeOverride);
        }
    }
}