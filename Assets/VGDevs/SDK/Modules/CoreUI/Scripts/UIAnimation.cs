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
        public List<UIAnimationStep> Steps = new List<UIAnimationStep>();

        public float OverallDuration => GetOverallDuration();
        
        public virtual void StartAnimation(CanvasGroup target, Action onComplete = null, Animator targetAnimator = null)
        {
            if (Steps.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }
            
            if (target == null)
            {
                Debug.LogWarning($"[UIAnimation] Warning: target or visualRoot are missing on '{name}', defaulting to OnComplete.");
                onComplete?.Invoke();
                return;
            }
            
            target.DOKill(true);
            var sequence = DOTween.Sequence(target);

            foreach (var step in Steps)
            {
                switch (step.Type)
                {
                    case UIAnimationStepType.Animation:
                        Animator animator = targetAnimator;
                        if (animator == null)
                            animator = target.GetOrAddComponent<Animator>();
                        
                        animator.runtimeAnimatorController = step.Animation.Animator;
                        animator.enabled = true;
                        sequence.JoinByStepType(step.JoinType, DOVirtual.Float(0f, 1f, step.Animation.Params.Duration, currentValue => {
                            if (animator == null)
                                return;
                            animator.SetFloat(step.Animation.MotionKey, currentValue);
                        }).SetDelay(step.Animation.Params.Delay).SetEase(step.Animation.Params.Ease));
                    break;
                    case UIAnimationStepType.Alpha:
                        sequence.JoinByStepType(step.JoinType, target.DOFade(step.Alpha.TargetValue, step.Alpha.Duration).SetDelay(step.Alpha.Delay).SetEase(step.Alpha.Ease));
                    break;
                    case UIAnimationStepType.Scaling:
                        sequence.JoinByStepType(step.JoinType, target.transform.DOScale(step.Scaling.TargetValue, step.Scaling.Duration).SetDelay(step.Scaling.Delay).SetEase(step.Scaling.Ease));
                    break;
                    case UIAnimationStepType.AnchorMin:
                        sequence.JoinByStepType(step.JoinType, ((RectTransform)target.transform).DOAnchorMin(step.AnchorMin.TargetValue, step.AnchorMin.Duration).SetDelay(step.AnchorMin.Delay).SetEase(step.AnchorMin.Ease));
                    break;
                    case UIAnimationStepType.AnchorMax:
                        sequence.JoinByStepType(step.JoinType, ((RectTransform)target.transform).DOAnchorMax(step.AnchorMin.TargetValue, step.AnchorMin.Duration).SetDelay(step.AnchorMin.Delay).SetEase(step.AnchorMin.Ease));
                    break;
                    case UIAnimationStepType.AnchorPositions:
                        sequence.JoinByStepType(step.JoinType, ((RectTransform)target.transform).DOAnchorPos(step.AnchorPositions.TargetValue, step.AnchorPositions.Duration).SetDelay(step.AnchorPositions.Delay).SetEase(step.AnchorPositions.Ease));
                    break;
                    case UIAnimationStepType.AnchorPositionX:
                        sequence.JoinByStepType(step.JoinType, ((RectTransform)target.transform).DOAnchorPosX(step.AnchorPositionX.TargetValue, step.AnchorPositionX.Duration).SetDelay(step.AnchorPositionX.Delay).SetEase(step.AnchorPositionX.Ease));
                    break;
                    case UIAnimationStepType.AnchorPositionY:
                        sequence.JoinByStepType(step.JoinType, ((RectTransform)target.transform).DOAnchorPosY(step.AnchorPositionY.TargetValue, step.AnchorPositionY.Duration).SetDelay(step.AnchorPositionY.Delay).SetEase(step.AnchorPositionY.Ease));
                    break;
                }
            }
            
            sequence.OnComplete(() => onComplete?.Invoke());
            sequence.Play();
        }

        public float GetOverallDuration()
        {
            float overallDuration = 0f;
            float latestLargerAppend = 0f;
            float latestLargerJoin = 0f;

            foreach (var step in Steps)
            {
                float stepDuration = GetDurationByStepType(step);
                
                if (step.JoinType.Equals(UIAnimationJoinType.Append))
                {
                    overallDuration += Mathf.Max(stepDuration, latestLargerJoin);
                    latestLargerAppend = stepDuration;
                    latestLargerJoin = 0f;
                    continue;
                }

                latestLargerJoin = Mathf.Max(stepDuration, latestLargerJoin);
            }
            overallDuration += latestLargerAppend > latestLargerJoin ? 0f : latestLargerJoin - latestLargerAppend;

            return overallDuration;
        }

        private static float GetDurationByStepType(UIAnimationStep step)
        {
            float stepDuration = 0f;
            switch (step.Type)
            {
                case UIAnimationStepType.Alpha:
                    stepDuration = step.Alpha.Duration + step.Alpha.Delay;
                break;
                case UIAnimationStepType.Scaling:
                    stepDuration = step.Scaling.Duration + step.Scaling.Delay;
                break;
                case UIAnimationStepType.Animation:
                    stepDuration = step.Animation.Params.Duration + step.Animation.Params.Delay;
                break;
                case UIAnimationStepType.AnchorMin:
                    stepDuration = step.AnchorMin.Duration + step.AnchorMin.Delay;
                break;
                case UIAnimationStepType.AnchorMax:
                    stepDuration = step.AnchorMax.Duration + step.AnchorMax.Delay;
                break;
                case UIAnimationStepType.AnchorPositions:
                    stepDuration = step.AnchorPositions.Duration + step.AnchorPositions.Delay;
                break;
                case UIAnimationStepType.AnchorPositionX:
                    stepDuration = step.AnchorPositionX.Duration + step.AnchorPositionX.Delay;
                break;
                case UIAnimationStepType.AnchorPositionY:
                    stepDuration = step.AnchorPositionY.Duration + step.AnchorPositionY.Delay;
                break;
            }

            return stepDuration;
        }

        [Serializable]
        public enum UIAnimationStepType
        {
            None,
            Alpha,
            Scaling,
            Animation,
            AnchorMin,
            AnchorPositions,
            AnchorMax,
            AnchorPositionX,
            AnchorPositionY
        }
        
        public enum UIAnimationJoinType
        {
            Join,
            Append
        }
        
        [Serializable]
        public struct UIAnimationStep
        {
            public UIAnimationStepType Type;
            public UIAnimationJoinType JoinType;
            public UIAnimationBaseParams<float> Alpha;
            public UIAnimationBaseParams<Vector3> Scaling;
            public UIAnimationParamsAnimator Animation;
            public UIAnimationBaseParams<Vector2> AnchorMin;
            public UIAnimationBaseParams<Vector2> AnchorMax;
            public UIAnimationBaseParams<Vector2> AnchorPositions;
            public UIAnimationBaseParams<float> AnchorPositionX;
            public UIAnimationBaseParams<float> AnchorPositionY;
        }

        [Serializable]
        public class UIAnimationParamsAnimator
        {
            public RuntimeAnimatorController Animator;
            public string MotionKey;
            public string TriggerKey;
            public UIAnimationBaseParams<float> Params;
        }

        [Serializable]
        public class UIAnimationBaseParams<T>
        {
            public float Duration;
            public float Delay;
            public T StartValue;
            public T EndValue;
            public T TargetValue;
            public Ease Ease;
        }
        
        public static IEnumerator WaitForAnimationComplete(Animator animator, Action onComplete, int animationLayer = 0)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(animationLayer);
            yield return new WaitForSeconds(stateInfo.length * stateInfo.speed);
            onComplete?.Invoke();
        }
    }
}

