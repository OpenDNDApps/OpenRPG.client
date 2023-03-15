using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VGDevs
{
    public class GameRuntime : MonoSingletonSelfGenerated<GameRuntime>, IGameDependency
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private CameraController m_mainCameraController;
        
        private SceneManager m_sceneManager;

        public static Action<CameraController, CameraController> OnCameraControllerChanged;

        public static Camera Camera => Instance.m_camera == null ? Instance.m_camera = Camera.main : Instance.m_camera;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            SceneManager.activeSceneChanged += OnActiveSceneChange;

            IsReady = true;
        }

        private void OnActiveSceneChange(Scene current, Scene next)
        {
            m_camera = Camera.main;
        }

        public CameraController MainCameraController
        {
            get => m_mainCameraController;
            set
            {
                OnCameraControllerChanged?.Invoke(m_mainCameraController, value);
                m_mainCameraController = value;
            }
        }

        public bool IsReady { get; set; }
    }
}