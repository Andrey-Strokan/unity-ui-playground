using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Animation that dissolves tooltip by mask texture.
    /// </summary>
    public class Dissolve : TooltipAnimation
    {
        [SerializeField]
        private AnimationCurve startPointAnimCurve;

        [SerializeField]
        private AnimationCurve endPointAnimCurve;


        public override void EvaluateAnimation(float alpha)
        {
            UpdateContainerVisuals(alpha);
            UpdateLineScale(alpha);
        }

        private void UpdateContainerVisuals(float alpha)
        {
            var curveValue = TooltipAnimCurve.Evaluate(alpha);
            TargetTooltip.TooltipColor = new Vector4(TargetTooltip.TooltipColor.r, TargetTooltip.TooltipColor.g, TargetTooltip.TooltipColor.b, curveValue);
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
    }
}
