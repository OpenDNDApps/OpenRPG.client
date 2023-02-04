using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VGDevs
{
    public class MenuScene : MonoBehaviour
    {
        private UIMenuScene m_menuSceneUI = null;
        
        private const string kUIMenuScene = "UIMenuScene";
        
        private void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            
            if (!UIRuntime.TryShowWindow(kUIMenuScene, out m_menuSceneUI))
                return;
            
            m_menuSceneUI.AnimatedShow();
            
            UserAccount.GetAccountStatistics();
            UserAccount.GetAccountCurrencyData();
            UserAccount.GetAccountCustomData();
        }

        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (m_menuSceneUI == null)
                return;
            
            m_menuSceneUI.AnimatedHide();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }
}