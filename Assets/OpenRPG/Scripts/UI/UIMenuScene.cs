using System;
using System.Collections.Generic;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static VGDevs.GameResources;

namespace VGDevs
{
    public class UIMenuScene : UIWindow
    {
        [SerializeField] private UIButton m_playButton;
        [SerializeField] private UIButton m_loginButton;

        [SerializeField] private TMP_Text m_generalMessage;

        private const string kUILoginWindow = "UILoginWindow";
        
        protected override void OnInit()
        {
            base.OnInit();
            
            m_playButton.OnClick = OnPlayButtonClick;

            UserLogin.OnFullLoginSuccess += HandleOnUserLoggedInChange;
            UserAccount.OnUserDataChanged += HandleOnUserLoggedInChange;
            
            HandleOnUserLoggedInChange(UserAccount.UserData);
        }

        private void HandleOnUserLoggedInChange(UserData userData)
        {
            if (!UserLogin.IsClientLoggedInFull)
            {
                m_loginButton.OnClick = OnLoginButtonClick;
            }
            else
            {
                m_loginButton.Hide(true);
            }
            
            m_generalMessage.SetText($"Welcome <b>{userData.Username}</b>" +
                                     $"\n" +
                                     $"\nScore: <b>{userData.Score}</b>" +
                                     $"\nKills: <b>{userData.Kills}</b>" +
                                     $"\nDeaths: <b>{userData.Deaths}</b>" +
                                     $"\nLevel: <b>{userData.Level}</b>" +
                                     $"\nSoftCoin: <b>{userData.SoftCoin}</b>" +
                                     $"\nHardCoin: <b>{userData.HardCoin}</b>");
        }

        public void OnPlayButtonClick()
        {
            this.Disable();
            SceneManager.LoadScene("GameScene");
        }
        
        public void OnLoginButtonClick()
        {
            if (!UIRuntime.TryShowWindow(kUILoginWindow, out UILoginWindow loginWindow))
                return;
            
            loginWindow.AnimatedShow();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UserLogin.OnFullLoginSuccess -= HandleOnUserLoggedInChange;
            UserAccount.OnUserDataChanged -= HandleOnUserLoggedInChange;
        }
    }  
}
