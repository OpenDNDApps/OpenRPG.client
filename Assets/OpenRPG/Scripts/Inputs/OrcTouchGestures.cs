using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ORC
{
    public class OrcTouchGestures : OrcMono
    {
        private OrcInputControls m_controls;
        
        public static event Action<float> OnPinchGesture;
        public static event Action<float> OnZoomGesture;
        
        private bool m_isPinching = false;
        private float m_lastPinchDistance = 0f;

        protected override void OnInit()
        {
            base.OnInit();
            m_controls = new OrcInputControls();
        }

        private void Start()
        {
            m_controls.Touch.SecondaryFingerContact.started += HandleOnSecondaryFingerContactStarted;
            m_controls.Touch.SecondaryFingerContact.canceled += HandleOnSecondaryFingerContactCanceled;
            
            m_controls.Keyboard.ScrollZoom.performed += HandleOnScrollZoomPerformed;
        }

        private void HandleOnScrollZoomPerformed(InputAction.CallbackContext context)
        {
            Debug.Log($"ScrollZoom: {context.ReadValue<float>()}");
            OnZoomGesture?.Invoke(context.ReadValue<float>());
        }

        private void Update()
        {
            HandlePinchLogic();
        }

        private void HandlePinchLogic()
        {
            if (!m_isPinching)
                return;
            
            Vector2 primaryTouchPosition = m_controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Vector2 secondaryTouchPosition = m_controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
            float currentPinchDistance = (primaryTouchPosition - secondaryTouchPosition).magnitude;
            float pinchDelta = Mathf.Sign(currentPinchDistance - m_lastPinchDistance);
            m_lastPinchDistance = currentPinchDistance;
            OnPinchGesture?.Invoke(pinchDelta);
        }

        private void HandleOnSecondaryFingerContactStarted(InputAction.CallbackContext context)
        {
            m_isPinching = true;
        }

        private void HandleOnSecondaryFingerContactCanceled(InputAction.CallbackContext context)
        {
            m_isPinching = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_controls.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_controls.Disable();
        }
    }
}