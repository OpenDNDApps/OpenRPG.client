using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public class CameraController : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameRuntime.Instance.MainCameraController = this;
        }
    }
}
