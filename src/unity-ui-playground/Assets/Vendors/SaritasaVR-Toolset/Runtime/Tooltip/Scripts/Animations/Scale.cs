using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Animation that changes scale of the tooltip.
    /// </summary>
    public class Scale : TooltipAnimation
    {
        [SerializeField]
        private AnimationCurve startPointAnimCurve;

        [SerializeField]
        private AnimationCurve endPointAnimCurve;

        /// <inheritdoc />
        public override void EvaluateAnimation(float alpha)
        {
            UpdateLineScale(alpha);
            UpdateContainerScale(alpha);
        }

        private void UpdateLineScale(float alpha)
        {
            var lineAlpha = LineAnimCurve.Evaluate(alpha);
            var lineTransform = TargetTooltip.LineRenderer.transform;
            var endPointParent = TargetTooltip.EndPointParent;
            var startPointParent = TargetTooltip.StartPointParent;

            lineTransform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 0f), Vector3.one, lineAlpha);
            lineTransform.transform.localPosition = Vector3.Lerp(new Vector3(0f, 0f, -0.5f), Vector3.zero, lineAlpha);

            endPointParent.localScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), Vector3.one, endPointAnimCurve.Evaluate(alpha));
            startPointParent.localScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), Vector3.one, startPointAnimCurve.Evaluate(alpha));
        }

        private void UpdateContainerScale(float alpha)
        {
            var scale = TooltipAnimCurve.Evaluate(alpha);
            TargetTooltip.ContainerRoot.localScale = new Vector3(scale, scale, scale);
        }
    }
}
