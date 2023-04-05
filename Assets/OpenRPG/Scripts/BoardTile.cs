using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Networking;
using VGDevs;

namespace ORC
{
    public class BoardTile : OrcMono
    {
        [SerializeField] private TileType m_tileType;

        [SerializeField] private Transform m_visualRoot2D;
        [SerializeField] private Transform m_visualRood3D;
        
        public TileType TileType => m_tileType;

        private void Start()
        {
            m_visualRood3D.rotation = Board.LastCameraRotation;
        }

        private void HandleOnCameraChanged(CinemachineVirtualCamera vCamMode)
        {
            if (vCamMode.IsOrthographic())
            {
                m_visualRoot2D.gameObject.SetActive(true);
                m_visualRood3D.gameObject.SetActive(false);
            }
            else
            {
                if (m_tileType != TileType.Board)
                {
                    m_visualRoot2D.gameObject.SetActive(false);
                }
                m_visualRood3D.gameObject.SetActive(true);
            }
        }

        private void HandleOnCameraRotationChanged(Quaternion newRotation)
        {
            if (m_tileType != TileType.Board)
            {
                m_visualRood3D.rotation = newRotation;
            }
        }

        protected override void OnEnable()
        {
            Board.OnCameraChanged += HandleOnCameraChanged;
            Board.OnCameraRotationChanged += HandleOnCameraRotationChanged;
        }

        protected override void OnDisable()
        {
            Board.OnCameraChanged -= HandleOnCameraChanged;
            Board.OnCameraRotationChanged -= HandleOnCameraRotationChanged;
        }
    }

    public enum TileType
    {
        Board,
        Prop,
        Tile,
        Token,
    }
}
