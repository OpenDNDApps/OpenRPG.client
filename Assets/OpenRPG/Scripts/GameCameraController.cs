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
        
        [SerializeField] private CinemachineVirtualCamera m_currentVCamera;
        [SerializeField] private CinemachineFramingTransposer m_currentFramingTransposer;
        [Range(0.1f, 8f)]
        [SerializeField] private float m_dragSpeedMultiplier = 1f;
        [SerializeField] private Transform m_moveTarget;
        
        private Vector3 m_dragOrigin;
        
        private float m_currentZoom = 0.5f;
        private Vector2 m_currentZoomRange = new Vector2(0.1f, 1f);

        private int m_lastCameraActiveIndex = -1;
        
        protected override void Awake()
        {
            base.Awake();
            
            SetNextVirtualCamera();
            
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdate);
            
            //Board.OnMouseOverScroll += OnMouseOverScroll;
        }

        public CinemachineVirtualCamera SetNextVirtualCamera()
        {
            m_lastCameraActiveIndex++;
            if(m_lastCameraActiveIndex >= m_vCameras.Count)
                m_lastCameraActiveIndex = 0;
            
            for (var i = 0; i < m_vCameras.Count; i++)
            {
                m_vCameras[i].Priority = i == m_lastCameraActiveIndex ? 1 : 0;
            }
            
            m_currentVCamera = m_vCameras[m_lastCameraActiveIndex];
            m_currentFramingTransposer = m_currentVCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            m_currentZoomRange = new Vector2(m_currentVCamera.m_Lens.FarClipPlane / 100f, m_currentVCamera.m_Lens.FarClipPlane / 2f);
            m_currentZoom = Mathf.InverseLerp(m_currentZoomRange.x, m_currentZoomRange.y, m_currentFramingTransposer.m_CameraDistance);
            
            Board.SetCamera(m_currentVCamera);

            return m_vCameras[m_lastCameraActiveIndex];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetNextVirtualCamera();
            }
        }

        private void OnCameraUpdate(CinemachineBrain brain)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_dragOrigin = GetMouseWorldPosition();
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 newPosition = GetMouseWorldPosition();
                Vector3 delta = m_dragOrigin - newPosition;
                delta.y = 0f; // Lock movement to X and Z axes only
                m_moveTarget.localPosition += delta * m_dragSpeedMultiplier;
                m_dragOrigin = newPosition;
            }
            
            float scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0f)
            {
                OnMouseOverScroll(scrollDelta);
            }
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = GameRuntime.Camera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            plane.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }

        private void OnMouseOverScroll(float scrollDelta)
        {
            m_currentZoom += -scrollDelta * m_scrollZoomSpeedFactor;
            m_currentZoom = Mathf.Clamp(m_currentZoom, 0f, 1f);
            var desiredZoomDistance = Mathf.Lerp(m_currentZoomRange.x, m_currentZoomRange.y, m_currentZoom);

            m_currentFramingTransposer.m_CameraDistance = desiredZoomDistance;
        }
    }
}
