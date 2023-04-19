using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VGDevs;

namespace ORC
{
    public class BoardInputObserver : OrcMono
    {
        private bool m_lastMouseOverState = false;
        
        private void OnMouseUpAsButton()
        {
            var hitPosition = GameRuntime.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
            Board.OnSurfaceClick?.Invoke(hitPosition);
            //Debug.Log($"Clicked on board at {hitPosition}");
        }

        private void OnMouseOver()
        {
            if (!m_lastMouseOverState)
            {
                Board.OnMouseOverSurface?.Invoke(true);
               // Debug.Log($"Clicked on mouse over board: true");
                m_lastMouseOverState = true;
            }
        }

        private void OnMouseEnter()
        {
            Board.OnMouseOverSurface?.Invoke(true);
            //Debug.Log($"Clicked on mouse over board: true");
            m_lastMouseOverState = true;
        }

        private void OnMouseExit()
        {
            Board.OnMouseOverSurface?.Invoke(false);
            //Debug.Log($"Clicked on mouse over board: false");
        }
    }
}