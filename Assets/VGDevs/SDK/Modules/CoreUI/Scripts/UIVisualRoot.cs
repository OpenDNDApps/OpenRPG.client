using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VGDevs;

namespace VGDevs
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIVisualRoot : UIItemBase
    {
        [Header("UIVisualRoot Settings")]
        [SerializeField] private CanvasGroup m_canvasGroup;
        
        [Header("UIAnimation Settings")] 
        [SerializeField] private List<WhenAnimationPair> m_animationsInfo = new List<WhenAnimationPair>();
        
        public List<WhenAnimationPair> AnimationsInfo => m_animationsInfo;

        protected override void OnInit()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();

            UIWindow isParentWindow = transform.parent.GetComponent<UIWindow>();
            if (isParentWindow != null)
            {
                Window.VisualRoots.AddUnique(this);
            }
            else
            {
                var parentAsUIItem = transform.parent.GetComponent<UIItem>();
                if (parentAsUIItem != null)
                {
                    parentAsUIItem.VisualRoots.AddUnique(this);
                }
            }

            if (m_animationsInfo.Count == 0)
            {
                m_animationsInfo.Add(new WhenAnimationPair
                {
                    TriggerType = VisualRootAnimTriggerType.EnterAnimation,
                    Animation = null
                });
                
                m_animationsInfo.Add(new WhenAnimationPair
                {
                    TriggerType = VisualRootAnimTriggerType.ExitAnimation,
                    Animation = null
                });
            }
        }

        public virtual float StartAnimation(VisualRootAnimTriggerType triggerType, Action onComplete = null)
        {
            return StartAnimationsOnTriggerMatches(m_animationsInfo, triggerType, onComplete);
        }
        
        public float StartAnimationsOnTriggerMatches(List<WhenAnimationPair> animPair, VisualRootAnimTriggerType trigger, Action onComplete = null)
        {
            float largestDuration = 0f;
            foreach (var pair in animPair)
            {
                if (!pair.TriggerType.HasFlag(trigger))
                    continue;
            
                UIAnimation theAnimation = null;
                if (pair.TriggerType.HasFlag(VisualRootAnimTriggerType.EnterAnimation))
                {
                    theAnimation = theAnimation != null ? theAnimation : pair.Animation != null ? pair.Animation : GameResources.Settings.UI.Default.ShowAnimation;
                } 
                if (pair.TriggerType.HasFlag(VisualRootAnimTriggerType.ExitAnimation))
                {
                    theAnimation = theAnimation != null ? theAnimation : pair.Animation != null ? pair.Animation : GameResources.Settings.UI.Default.ShowAnimation;
                }
                theAnimation = theAnimation != null ? theAnimation : pair.Animation != null ? pair.Animation : GameResources.Settings.UI.Default.EmptyAnimation;
            
                var animClone = Instantiate(theAnimation);
                animClone.StartAnimation(m_canvasGroup, null);
            
                largestDuration = Mathf.Max(largestDuration, theAnimation.OverallDuration);
            }

            if (onComplete != null)
            {
                this.ActionAfterSeconds(largestDuration, onComplete);
            }

            return largestDuration;
        }

        public void Disable(bool softDisable = false)
        {
            m_canvasGroup.Disable(softDisable);
        }

        public void Enable()
        {
            m_canvasGroup.Enable();
        }
    }

    [Serializable]
    public struct WhenAnimationPair
    {
        public VisualRootAnimTriggerType TriggerType;
        public UIAnimation Animation;
    }

    [System.Flags]
    public enum VisualRootAnimTriggerType
    {
        Manual,
        EnterAnimation,
        ExitAnimation,
        OnEnable,
        OnDisable
    }
}

public static class VisualRootExtensions
{
    public static List<UIVisualRoot> StartAnimation(this List<UIVisualRoot> visualRoots, VisualRootAnimTriggerType trigger, Action onComplete = null)
    {
        float largestDuration = 0f;
        List<UIVisualRoot> toStartAnimation = visualRoots.GetVisualRootsByTriggerType(trigger);
        foreach (var visualRoot in visualRoots)
        {
            if (!visualRoot.AnimationsInfo.Exists(pair => pair.TriggerType.HasFlag(trigger)))
                continue;

            largestDuration = Mathf.Max(largestDuration, visualRoot.StartAnimation(trigger));
        }

        GameRuntime.Instance.ActionAfterSeconds(largestDuration, onComplete);
        return toStartAnimation;
    }
    
    public static List<UIVisualRoot> GetVisualRootsByTriggerType(this List<UIVisualRoot> visualRoots, VisualRootAnimTriggerType trigger)
    {
        List<UIVisualRoot> toReturn = new List<UIVisualRoot>();
        foreach (var visualRoot in visualRoots)
        {
            if (!visualRoot.AnimationsInfo.Exists(pair => pair.TriggerType.HasFlag(trigger)))
                continue;
            
            toReturn.AddUnique(visualRoot);
        }
        return toReturn;
    }
    
    public static void Disable(this List<UIVisualRoot> visualRootPairs, bool softDisable = false)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Disable(softDisable);
        }
    }
    
    public static void Enable(this List<UIVisualRoot> visualRootPairs)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Enable();
        }
    }
    
    public static void DOKill(this List<UIVisualRoot> visualRoots)
    {
        foreach (var visualRoot in visualRoots)
        {
            visualRoot.DOKill();
        }
    }
}
