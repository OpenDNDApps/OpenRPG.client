using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace ORC
{
    public class BoardTile : OrcMono
    {
        [SerializeField] private TileType m_tileType;

        [SerializeField] private Transform m_visualRoot2D;
        [SerializeField] private Transform m_visualRood3D;
        
        public TileType TileType => m_tileType;

        private void OnPerspectiveChangedHandler(LensSettings.OverrideModes vCamMode)
        {
            switch (vCamMode)
            {
                case LensSettings.OverrideModes.Orthographic:
                    m_visualRoot2D.gameObject.SetActive(true);
                    m_visualRood3D.gameObject.SetActive(false);
                    break;
                case LensSettings.OverrideModes.Perspective:
                    m_visualRoot2D.gameObject.SetActive(false);
                    m_visualRood3D.gameObject.SetActive(true);
                    break;
            }
        }

        protected override void OnEnable()
        {
            Board.OnPerspectiveChanged += OnPerspectiveChangedHandler;
        }

        protected override void OnDisable()
        {
            Board.OnPerspectiveChanged -= OnPerspectiveChangedHandler;
        }
    }

    public enum TileType
    {
        Board,
        Tile,
        Prop,
        Token,
    }
}
