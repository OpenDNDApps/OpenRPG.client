using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace VGDevs
{
    [CreateAssetMenu(fileName = "UIAnimation_", menuName = GameResources.kPluginName + "/UI/Animation")]
    public class UIAnimation : ScriptableObject
    {
        public bool UseAnimation;
        public UIAnimationParamsAnimator Animation;
        
        [Header("Alpha Transparency")]
        public bool UseAlpha = false;
        public UIAnimationBaseParams<float> Alpha;
        
        [Header("Scaling")]
        public bool UseScaling = false;
        public UIAnimationBaseParams<Vector3> Scaling;

        public float OverallDuration = 1f;

        public virtual void StartAnimation(UIItem targetItem, Action onComplete = null)
        {
            StartAnimation(targetItem.VisualRoot, onComplete);
        }
        
        public virtual void StartAnimation(CanvasGroup target, Action onComplete = null)
        {
            if (target == null)
            {
                Debug.LogWarning($"[UIAnimation] Warning: target or visualRoot are missing on '{name}', defaulting to OnComplete.");
                onComplete?.Invoke();
                return;
            }

            target.DOKill(true);
            var sequence = DOTween.Sequence(target);
            sequence.AppendInterval(OverallDuration);
            
            if (UseAnimation)
            {
                var animator = target.GetOrAddComponent<Animator>();
                animator.runtimeAnimatorController = Animation.Animator;
                animator.enabled = true;
                sequence.PrependCallback(() =>
                {
                    if (animator == null) return;
                    animator.SetFloat(Animation.MotionKey, Animation.Params.StartValue);
                    animator.SetTrigger(Animation.TriggerKey);
                });
                sequence.Join(DOVirtual.Float(Animation.Params.StartValue, Animation.Params.EndValue, Animation.Params.Duration, value => {
                    if (animator == null) 
                        return;
                    animator.SetFloat(Animation.MotionKey, value);
                }).SetDelay(Animation.Params.Delay).SetEase(Animation.Params.Ease));
            }
            
            if (UseAlpha)
            {
                target.alpha = Alpha.StartValue;
                sequence.Join(target.DOFade(Alpha.EndValue, Alpha.Duration).SetDelay(Alpha.Delay).SetEase(Alpha.Ease));
            }

            if (UseScaling)
            {
                target.transform.localScale = Scaling.StartValue;
                sequence.Join(target.transform.DOScale(Scaling.EndValue, Scaling.Duration).SetDelay(Scaling.Delay).SetEase(Scaling.Ease));
            }
            
            sequence.OnComplete(() => onComplete?.Invoke());
            sequence.Play();
        }

        [Serializable]
        public struct UIAnimationParamsAnimator
        {
            public RuntimeAnimatorController Animator;
            public string MotionKey;
            public string TriggerKey;
            public UIAnimationBaseParams<float> Params;
        }

        [Serializable]
        public struct UIAnimationBaseParams<T>
        {
            public float Duration;
            public float Delay;
            public T StartValue;
            public T EndValue;
            public Ease Ease;
        }
        
        public static IEnumerator WaitForAnimationComplete(Animator animator, Action onComplete)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length * stateInfo.speed);
            onComplete?.Invoke();
        }
    }
}

