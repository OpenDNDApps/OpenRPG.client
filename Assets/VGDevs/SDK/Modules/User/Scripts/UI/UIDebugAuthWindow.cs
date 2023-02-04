using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace VGDevs
{
    using static UserRuntime;
    
    public class UIDebugAuthWindow : UIWindow
    {
        [Header("Debug Auth Window")] 
        [SerializeField] private TMP_Text m_playerID;

        protected override void OnInit()
        {
            base.OnInit();
            UserAccount.OnUserDataChanged += Build;
            UserLogin.OnFullLoginSuccess += Build;
            UserLogin.OnSilentLoginSuccess += Build;
        }

        public void Build(UserData result)
        {
            m_playerID.SetText($"ID: {result.ID}");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UserAccount.OnUserDataChanged -= Build;
            UserLogin.OnFullLoginSuccess -= Build;
            UserLogin.OnSilentLoginSuccess -= Build;
        }
    }
}
