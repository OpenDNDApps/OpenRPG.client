using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace VGDevs
{
    public class ScriptableAnimation<TIAnimStep> : ScriptableObject where TIAnimStep : BaseAnimationStep
    {
        public List<TIAnimStep> Steps = new List<TIAnimStep>();

        public float OverallDuration => GetOverallDuration();

        public float GetOverallDuration()
        {
            float overallDuration = 0f;
            float latestLargerAppend = 0f;
            float latestLargerJoin = 0f;

            foreach (var step in Steps)
            {
                float stepDuration = GetDurationByStepType(step);
                
                if (step.JoinType.Equals(ScriptableAnimationJoinType.Append))
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


        public virtual void StartAnimation()
        {
            
        }
        
        public static IEnumerator WaitForAnimationComplete(Animator animator, Action onComplete, int animationLayer = 0)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(animationLayer);
            yield return new WaitForSeconds(stateInfo.length * stateInfo.speed);
            onComplete?.Invoke();
        }

        protected virtual float GetDurationByStepType(BaseAnimationStep step)
        {
            return 0f;
        }
    }
        
    public enum ScriptableAnimationJoinType
    {
        Join,
        Append
    }

    [Serializable]
    public class AnimationBaseParams<T>
    {
        public float Duration;
        public float Delay;
        public T StartValue;
        public T EndValue;
        public T TargetValue;
        public Ease Ease;
    }

    [Serializable]
    public class UIAnimationParamsAnimator
    {
        public RuntimeAnimatorController Animator;
        public string MotionKey;
        public string TriggerKey;
        public AnimationBaseParams<float> Params;
    }
    
    [Serializable]
    public class BaseAnimationStep
    {
        public UIAnimationStepType Type;
        public ScriptableAnimationJoinType JoinType;
    }
}