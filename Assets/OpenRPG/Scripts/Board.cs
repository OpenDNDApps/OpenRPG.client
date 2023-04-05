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
        public static Action<Quaternion> OnCameraRotationChanged;
        public static Action<bool> OnMouseOverSurface;
        public static Action<float> OnMouseOverScroll;
        
        public static Quaternion LastCameraRotation { get; private set; }
        
        public static event Action<CinemachineVirtualCamera> OnCameraChanged;

        public static void NotifyCameraChange(CinemachineVirtualCamera vCamera)
        {
            OnCameraChanged?.Invoke(vCamera);
        }
        
        public static void NotifyCameraRotationChanged(Quaternion rotation)
        {
            LastCameraRotation = rotation;
            OnCameraRotationChanged?.Invoke(LastCameraRotation);
        }
    }
}