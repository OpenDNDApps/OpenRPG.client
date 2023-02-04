using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VGDevs
{
    using static UserRuntime;
    
    public class UILoginWindow : UIWindow
    {
        [Header("UILoginWindow")]
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private UIButton m_submitButton;
        
        [Header("Login")]
        [SerializeField] private UIInput m_username;
        [SerializeField] private UIInput m_email;
        [SerializeField] private UIInput m_password;
        [SerializeField] private UIToggle m_rememberMe;
        [SerializeField] private UIButton m_registerOption;
        
        [Header("Register")]
        [SerializeField] private UIInput m_confirmPassword;
        [SerializeField] private UIButton m_loginOption;
        
        private const string kTitleAsLogin = "Login";
        private const string kSubmitButtonAsLogin = "Login";
        
        private const string kTitleAsRegister = "Register";
        private const string kSubmitButtonAsRegister = "Register";
        
        protected override void OnInit()
        {
            base.OnInit();

            m_loginOption.OnClick = SetFormAsLogin;
            m_registerOption.OnClick = SetFormAsRegister;
            m_rememberMe.OnValueChanged += newValue => { UserLogin.RememberMe = newValue; };
            
            SetFormAsLogin();
        }

        private void Login()
        {
            if (!IsValidLogin())
            {
                Debug.LogError($"Missing Login data.");
                return;
            }
            
            var userName = new UserData
            {
                Username = m_username.Value,
                Email = m_email.Value,
                Password = m_password.Value
            };
            
            if (!userName.IsValidData(UserAccount.AuthType.EmailAndPassword, out string errorMessage))
            {
                UIRuntime.TryShowGenericError(errorMessage);
                return;
            }
            
            UserLogin.OnFullLoginSuccess += HandleOnUserLoginOnOnFullLoginSuccess;
            UserLogin.DoManualLogin(userName);
        }

        private void HandleOnUserLoginOnOnFullLoginSuccess(UserData userData)
        {
            UserLogin.OnFullLoginSuccess -= HandleOnUserLoginOnOnFullLoginSuccess;
            UIRuntime.TryShowWindow("UIAlert - Info", out UIAlertPopup alertPopup);
            alertPopup.Build("Logged In!");
            alertPopup.AnimatedShow();
            AnimatedHide();
        }

        private void Register()
        {
            var userName = new UserData
            {
                Username = m_username.Value,
                Email = m_email.Value,
                Password = m_password.Value
            };

            userName.IsValidData(UserAccount.AuthType.EmailAndPassword, out string errorMessage);
            
            if (!m_password.Value.Equals(m_confirmPassword.Value))
                errorMessage = "Passwords don't match.";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                UIRuntime.TryShowWindow("UIAlert - Error", out UIAlertPopup alertPopup);
                alertPopup.Build(errorMessage);
                alertPopup.AnimatedShow();
                return;
            }

            UserRegister.OnRegisterSuccess += HandleOnUserRegisterOnOnRegisterSuccess;
            UserRegister.DoRegisterAccount(userName);
        }

        private void HandleOnUserRegisterOnOnRegisterSuccess(UserData data)
        {
            UserRegister.OnRegisterSuccess -= HandleOnUserRegisterOnOnRegisterSuccess;
            UIRuntime.TryShowWindow("UIAlert - Info", out UIAlertPopup alertPopup);
            alertPopup.Build("Logged In!");
            alertPopup.AnimatedShow();
        }

        private bool IsValidLogin()
        {
            return true;
        }

        private bool IsValidRegister()
        {
            return true;
        }

        private void SetFormAsLogin()
        {
            m_title.SetLocalizedText(kTitleAsLogin);
            m_submitButton.SetLabel(kSubmitButtonAsLogin);

            m_submitButton.OnClick = Login;
            
            // Hidings
            m_confirmPassword.Hide(true);
            m_loginOption.Hide(true);
            m_username.Hide(true);
            
            // Showings
            m_registerOption.Show(true);
        }

        private void SetFormAsRegister()
        {
            m_title.SetLocalizedText(kTitleAsRegister);
            m_submitButton.SetLabel(kSubmitButtonAsRegister);

            m_submitButton.OnClick = Register;
            
            // Showings
            m_registerOption.Hide(true);
            
            // Hidings
            m_username.Show(true);
            m_confirmPassword.Show(true);
            m_loginOption.Show(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            UserLogin.OnFullLoginSuccess -= HandleOnUserLoginOnOnFullLoginSuccess;
            UserRegister.OnRegisterSuccess -= HandleOnUserRegisterOnOnRegisterSuccess;
        }
    }
}