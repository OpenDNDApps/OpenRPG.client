using DG.Tweening;

namespace VGDevs
{
    public static class UIAnimationExtensions
    {
        public static void JoinByStepType(this Sequence sequence, UIAnimation.UIAnimationJoinType step, Tween tween)
        {
            switch (step)
            {
                default:
                case UIAnimation.UIAnimationJoinType.Join:
                    sequence.Join(tween);
                    break;
                case UIAnimation.UIAnimationJoinType.Append:
                    sequence.Append(tween);
                    break;
            }
        }
    }
}