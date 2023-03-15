using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VGDevs;

namespace OpenRPG
{
    public class MenuScene : OrcMono
    {
        private UIMainMenu m_uiMainMenu = null;
        
        private const string kUIMainMenu = "UIMainMenu";
        
        IEnumerator Start()
        {
            yield return new WaitUntil(() => GameResources.Runtime.IsReady);
            yield return new WaitUntil(() => GameResources.UIRuntime.IsReady);
            yield return new WaitUntil(() => GameResources.UserRuntime.IsReady);
            
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            if (UIRuntime.TryShowWindow(kUIMainMenu, out m_uiMainMenu))
            {
                m_uiMainMenu.AnimatedShow();
            }
            
            UserAccount.GetAccountStatistics();
            UserAccount.GetAccountCurrencyData();
            UserAccount.GetAccountCustomData();
        }

        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (m_uiMainMenu == null)
                return;
            
            m_uiMainMenu.AnimatedHide();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }
}