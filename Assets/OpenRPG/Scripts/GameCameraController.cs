using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using VGDevs;

namespace ORC
{
    public class GameCameraController : CameraController
    {
        [SerializeField] private float m_scrollZoomSpeedFactor = 1f;
        //[SerializeField] private float m_cameraMouseMovementFactor = 1f;

        [SerializeField] private List<CinemachineVirtualCamera> m_vCameras = new List<CinemachineVirtualCamera>();

        private int m_lastCameraActiveIndex = -1;
        
        protected override void Awake()
        {
            base.Awake();
            
            BlendToNextCamera();
            
            //Board.OnMouseOverScroll += OnMouseOverScroll;
        }

        public CinemachineVirtualCamera BlendToNextCamera()
        {
            m_lastCameraActiveIndex++;
            if(m_lastCameraActiveIndex >= m_vCameras.Count)
                m_lastCameraActiveIndex = 0;
            
            for (var i = 0; i < m_vCameras.Count; i++)
            {
                m_vCameras[i].Priority = i == m_lastCameraActiveIndex ? 1 : 0;
            }
            
            Board.SetCamera(m_vCameras[m_lastCameraActiveIndex]);

            return m_vCameras[m_lastCameraActiveIndex];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                BlendToNextCamera();
            }
        }

        private void OnMouseOverScroll(float scrollDelta)
        {
            Vector3 desiredPosition = GameRuntime.Camera.transform.position;
            desiredPosition += Vector3.up * -scrollDelta * m_scrollZoomSpeedFactor;
            if (desiredPosition.y < 1)
            {
                desiredPosition = new Vector3(desiredPosition.x, 1, desiredPosition.z);
            }

            GameRuntime.Camera.transform.position = desiredPosition;
            GameRuntime.Camera.farClipPlane = Mathf.Ceil(desiredPosition.y) + 10;
        }
    }
}
