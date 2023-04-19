using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public partial class GameSettingsCollection
    {
        [Header("UI Settings")]
        public UISettings UI;

        [Serializable]
        public struct UISettings
        {
            public float DisabledAlpha;
            public float EnabledAlpha;
            public List<GameUIResourcesCollection.UISortingKeyPair> Sorting;
            public UIDefaults Default;
        }

        [Serializable]
        public struct UIDefaults
        {
            public float DelayedTimeOnUIInputOnValueChanged;
            public float PositionBasedAnimationDelay;
            public UIItem Transition;
            public UIInputBlocker InputBlocker;
            public UIAnimation ShowAnimation;
            public UIAnimation HideAnimation;
            public UIAnimation EmptyAnimation;
            public Sprite TransparentSprite;
        }
    }
}