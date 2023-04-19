using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using VGDevs;

namespace ORC
{
    public class GameCameraController : CameraController
    {
        [SerializeField] private float m_scrollZoomSpeedFactor = .003f;
        [SerializeField] private float m_rotationSpeedFactor = .6f;
        //[SerializeField] private float m_cameraMouseMovementFactor = 1f;

        [SerializeField] private List<CinemachineVirtualCamera> m_vCameras = new List<CinemachineVirtualCamera>();
        
        [SerializeField] private CinemachineVirtualCamera m_currentVCamera;
        [SerializeField] private CinemachineFramingTransposer m_currentFramingTransposer;
        [Range(0.1f, 8f)]
        [SerializeField] private float m_dragSpeedMultiplier = 1f;
        [SerializeField] private Transform m_moveTarget;
        
        private Vector3 m_dragOrigin;
        private Vector3 m_rotateOrigin;
        
        private float m_currentZoom = 0.5f;
        private Vector2 m_currentZoomRange = new Vector2(0.1f, 1f);

        private int m_lastCameraActiveIndex = -1;
        
        protected override void Awake()
        {
            base.Awake();
            
            SetNextVirtualCamera();
            
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdate);
            
            Board.OnMouseOverScroll += OnMouseOverScroll;
        }

        public CinemachineVirtualCamera SetNextVirtualCamera()
        {
            m_lastCameraActiveIndex++;
            if(m_lastCameraActiveIndex >= m_vCameras.Count)
                m_lastCameraActiveIndex = 0;
            
            for (int i = 0; i < m_vCameras.Count; i++)
            {
                m_vCameras[i].Priority = i == m_lastCameraActiveIndex ? 1 : 0;
            }
            
            m_currentVCamera = m_vCameras[m_lastCameraActiveIndex];
            Board.NotifyCameraChange(m_currentVCamera);

            SetNewCameraZoomLogic();
            
            return m_vCameras[m_lastCameraActiveIndex];
        }

        private void SetNewCameraZoomLogic()
        {
            bool isInit = m_currentFramingTransposer == null;
            m_currentFramingTransposer = m_currentVCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            m_currentZoomRange = new Vector2(m_currentVCamera.m_Lens.FarClipPlane / 100f, m_currentVCamera.m_Lens.FarClipPlane / 2f);
            m_currentZoom = isInit ? Mathf.InverseLerp(m_currentZoomRange.x, m_currentZoomRange.y, m_currentFramingTransposer.m_CameraDistance) : m_currentZoom;
            
            var desiredZoomDistance = Mathf.Lerp(m_currentZoomRange.x, m_currentZoomRange.y, m_currentZoom);
            m_currentFramingTransposer.m_CameraDistance = desiredZoomDistance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetNextVirtualCamera();
            }
            
            if (Input.mouseScrollDelta.y != 0)
            {
                OnMouseOverScroll(Input.mouseScrollDelta.y);
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
            else if (Input.GetMouseButtonDown(1))
            {
                m_rotateOrigin = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 newPosition = Input.mousePosition;
                Vector3 delta = newPosition - m_rotateOrigin;
                if (Input.mousePosition.y < Screen.height / 2f)
                {
                    delta.x *= -1;
                }
                if (Input.mousePosition.x > Screen.width / 2f)
                {
                    delta.y *= -1;
                }
                m_rotateOrigin = newPosition;
                Quaternion desiredRotation = transform.localRotation * Quaternion.Euler(0,  -1 * ((delta.x + delta.y) * m_rotationSpeedFactor), 0);
                Board.NotifyCameraRotationChanged(desiredRotation);
                transform.localRotation = desiredRotation;
            }
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = GameRuntime.WorldCamera.ScreenPointToRay(Input.mousePosition);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return ray.origin;
            }
            
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

    public static class OrcCameraExtensions
    {
        public static bool IsOrthographic(this CinemachineVirtualCamera vCamera)
        {
            // TODO: Find a better way to check rather than using the FOV
            return vCamera.m_Lens.Orthographic || vCamera.m_Lens.FieldOfView <= 3;
        }
    }
}
