using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VGDevs;

namespace VGDevs
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIVisualRoot : UIItemBase, IPointerEnterHandler, IPointerExitHandler
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
                UIItem parentAsUIItem = transform.parent.GetComponent<UIItem>();
                if (parentAsUIItem != null)
                {
                    parentAsUIItem.VisualRoots.AddUnique(this);
                }
            }

            if (m_animationsInfo.Count == 0)
            {
                m_animationsInfo.Add(new WhenAnimationPair
                {
                    TriggerType = VisualRootAnimTriggerType.AnimatedShow,
                    Animation = null
                });
                
                m_animationsInfo.Add(new WhenAnimationPair
                {
                    TriggerType = VisualRootAnimTriggerType.AnimatedHide,
                    Animation = null
                });
            }
        }
        
        private bool HasAnyAnimationsOfType(VisualRootAnimTriggerType p_trigger)
        {
            return m_animationsInfo.Any(p_pair => p_pair.TriggerType.HasFlag(p_trigger));
        }

        public virtual float StartAnimation(VisualRootAnimTriggerType triggerType, Action onComplete = null)
        {
            if (!HasAnyAnimationsOfType(triggerType))
                return 0f;
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
                if (pair.TriggerType.HasFlag(VisualRootAnimTriggerType.AnimatedShow))
                {
                    theAnimation = theAnimation != null ? theAnimation : pair.Animation != null ? pair.Animation : GameResources.Settings.UI.Default.ShowAnimation;
                } 
                if (pair.TriggerType.HasFlag(VisualRootAnimTriggerType.AnimatedHide))
                {
                    theAnimation = theAnimation != null ? theAnimation : pair.Animation != null ? pair.Animation : GameResources.Settings.UI.Default.HideAnimation;
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

        public void Enable()
        {
            m_canvasGroup.Enable();
        }

        public void Activate()
        {
            m_canvasGroup.Activate();
            StartAnimation(VisualRootAnimTriggerType.Activate);
        }

        public void Disable(bool softDisable = false)
        {
            m_canvasGroup.Disable(softDisable);
        }

        public void Deactivate()
        {
            m_canvasGroup.Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartAnimation(VisualRootAnimTriggerType.PointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartAnimation(VisualRootAnimTriggerType.PointerExit);
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
        AnimatedShow = 1 << 0,
        AnimatedHide = 1 << 1,
        Activate = 1 << 2,
        // Deactivate = 1 << 3, Use AnimatedHide instead.
        PointerEnter = 1 << 4,
        PointerExit = 1 << 5,
        PointerClick = 1 << 6,
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
    
    public static List<UIVisualRoot> Enable(this List<UIVisualRoot> visualRootPairs)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Enable();
        }

        return visualRootPairs;
    }
    
    public static List<UIVisualRoot> Activate(this List<UIVisualRoot> visualRootPairs)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Activate();
        }

        return visualRootPairs;
    }
    
    public static List<UIVisualRoot> Disable(this List<UIVisualRoot> visualRootPairs, bool softDisable = false)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Disable(softDisable);
        }
        
        return visualRootPairs;
    }
    
    public static List<UIVisualRoot> Deactivate(this List<UIVisualRoot> visualRootPairs)
    {
        foreach (var visualRoot in visualRootPairs)
        {
            visualRoot.Deactivate();
        }

        return visualRootPairs;
    }
    
    public static List<UIVisualRoot> DOKill(this List<UIVisualRoot> visualRoots)
    {
        foreach (var visualRoot in visualRoots)
        {
            visualRoot.DOKill();
        }
        
        return visualRoots;
    }
}
