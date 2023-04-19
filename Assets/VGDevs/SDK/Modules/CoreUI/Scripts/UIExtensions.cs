using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VGDevs
{
    public static class UIExtensions
    {
        public static string GetLocalizedText(this TMP_Text tmp)
        {
            // if defaultAsFail just set key as string.
            // TODO: Get Localization
            return tmp.text;
        }
        
        public static void SetLocalizedText(this TMP_Text tmp, string localizationKey, bool defaultAsFail = true)
        {
            // if defaultAsFail just set key as string.
            // TODO: Localization
            tmp.text = localizationKey;
        }

        public static void Enable(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = GameResources.Settings.UI.EnabledAlpha;
            canvasGroup.Activate();
        }

        public static void Activate(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.gameObject.SetActive(true);
        }

        public static void Disable(this CanvasGroup canvasGroup, bool softDisable = false)
        {
            canvasGroup.alpha = softDisable ? GameResources.Settings.UI.DisabledAlpha : 0f;
            canvasGroup.Deactivate(softDisable);
        }

        public static void Deactivate(this CanvasGroup canvasGroup, bool softDisable = false)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            if (softDisable) 
                return;

            canvasGroup.gameObject.SetActive(false);
            canvasGroup.DOKill();
        }

        public static void Enable(this TMP_Text text)
        {
            text.alpha = GameResources.Settings.UI.EnabledAlpha;
            text.Activate();
        }

        public static void Activate(this TMP_Text text)
        {
            text.raycastTarget = false;
            text.gameObject.SetActive(true);
        }

        public static void Disable(this TMP_Text text, bool softDisable = false)
        {
            text.alpha = softDisable ? GameResources.Settings.UI.DisabledAlpha : 0f;
            text.raycastTarget = false;
            
            if (softDisable) 
                return;
            
            text.Deactivate();
        }
        
        public static void Deactivate(this TMP_Text text)
        {
            text.raycastTarget = false;
            text.gameObject.SetActive(false);
            text.DOKill();
        }
    }
}
