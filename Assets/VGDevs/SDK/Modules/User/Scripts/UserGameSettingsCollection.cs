using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    public partial class GameSettingsCollection
    {
        [Header("User Settings")]
        public UserSettings User;

        [Serializable]
        public struct UserSettings
        {
            public string DebugDeviceOverrideID;
            public Vector2 UsernameLengthMinMax;
            public Vector2 PasswordLengthMinMax;
            public Vector2 EmailLengthMinMax;
            public Vector2 DisplayNameLengthMinMax;
            public bool ShowDebugWindow;
            public bool ForceLink;
            public GetPlayerCombinedInfoRequestParams InfoRequestParams;
        }
    }
}