using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Base class of toolips's animation.
    /// </summary>
    [RequireComponent(typeof(AnimatedTooltip))]
    public abstract class TooltipAnimation : MonoBehaviour
    {
        [SerializeField]
        protected AnimatedTooltip TargetTooltip;

        [SerializeField]
        [Tooltip("Curve for line Animation.")]
        protected AnimationCurve LineAnimCurve;

        [SerializeField]
        [Tooltip("Curve for tooltip animation.")]
        protected AnimationCurve TooltipAnimCurve;

        /// <summary>
        /// Property block for material animation.
        /// </summary>
        public MaterialPropertyBlock TooltipPropBlock => TargetTooltip.BackGroundPropBlock;

        /// <summary>
        /// Updates animation.
        /// </summary>
        /// <param name="alpha">Current position of the animation.</param>
        public abstract void EvaluateAnimation(float alpha);
    }
}
