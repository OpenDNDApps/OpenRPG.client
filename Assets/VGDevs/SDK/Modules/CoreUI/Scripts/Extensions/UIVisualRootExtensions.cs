using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VGDevs
{
    public static class UIVisualRootExtensions
    {
        public static void Init(this List<UIVisualRoot> p_visualRootPairs)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.Init();
            }
        }
        
        public static void AnimatedShow(this List<UIVisualRoot> p_visualRoots, Action p_onComplete)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRoots)
            {
                l_visualRoot.Disable();
                l_visualRoot.Activate();
                l_visualRoot.StartAnimation(VisualRootAnimTriggerType.AnimatedShow, p_onComplete);
            }
        }
        
        public static void AnimatedHide(this List<UIVisualRoot> p_visualRoots, Action p_onComplete)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRoots)
            {
                l_visualRoot.StartAnimation(VisualRootAnimTriggerType.AnimatedHide, p_onComplete);
            }
        }
        
        public static List<UIVisualRoot> StartAnimation(this List<UIVisualRoot> p_visualRoots, VisualRootAnimTriggerType p_trigger, Action p_onComplete = null)
        {
            float l_largestDuration = 0f;
            List<UIVisualRoot> l_toStartAnimation = p_visualRoots.GetVisualRootsByTriggerType(p_trigger);
            foreach (UIVisualRoot l_visualRoot in p_visualRoots)
            {
                if (!l_visualRoot.AnimationsInfo.Exists(p_pair => p_pair.TriggerType.HasFlag(p_trigger)))
                    continue;
                l_visualRoot.Activate();
                l_largestDuration = Mathf.Max(l_largestDuration, l_visualRoot.StartAnimation(p_trigger));
            }

            GameResources.Runtime.ActionAfterSeconds(l_largestDuration, p_onComplete);
            return l_toStartAnimation;
        }
        
        public static List<UIVisualRoot> GetVisualRootsByTriggerType(this List<UIVisualRoot> p_visualRoots, VisualRootAnimTriggerType p_trigger)
        {
            List<UIVisualRoot> l_toReturn = new List<UIVisualRoot>();
            foreach (UIVisualRoot l_visualRoot in p_visualRoots)
            {
                if (!l_visualRoot.AnimationsInfo.Exists(p_pair => p_pair.TriggerType.HasFlag(p_trigger)))
                    continue;
                
                l_toReturn.AddUnique(l_visualRoot);
            }
            return l_toReturn;
        }
        
        public static void Enable(this List<UIVisualRoot> p_visualRootPairs)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.Enable();
            }
        }
        
        public static void Activate(this List<UIVisualRoot> p_visualRootPairs)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.Activate();
            }
        }
        
        public static void Disable(this List<UIVisualRoot> p_visualRootPairs, bool p_softDisable = false)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.Disable(p_softDisable);
            }
        }
        
        public static void Deactivate(this List<UIVisualRoot> p_visualRootPairs)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.Deactivate();
            }
        }
        
        public static void DOKill(this List<UIVisualRoot> p_visualRoots)
        {
            foreach (var l_visualRoot in p_visualRoots)
            {
                l_visualRoot.DOKill();
            }
        }
        
        public static void HandleOnPointerEnter(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.HandleOnPointerEnter(p_pointerEventData);
            }
        }
        
        public static void TriggerOnPointerEnterBehaviour(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            p_visualRootPairs.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.PointerClick).HandleOnPointerEnter(p_pointerEventData);
        }
        
        public static void HandleOnPointerExit(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.HandleOnPointerExit(p_pointerEventData);
            }
        }
        
        public static void TriggerOnPointerExitBehaviour(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            p_visualRootPairs.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.PointerClick).HandleOnPointerExit(p_pointerEventData);
        }
        
        public static void HandleOnPointerClick(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            foreach (UIVisualRoot l_visualRoot in p_visualRootPairs)
            {
                l_visualRoot.HandleOnPointerClick(p_pointerEventData);
            }
        }
        
        public static void TriggerOnPointerClickBehaviour(this List<UIVisualRoot> p_visualRootPairs, PointerEventData p_pointerEventData)
        {
            p_visualRootPairs.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.PointerClick).HandleOnPointerClick(p_pointerEventData);
        }
    }
}