using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Animation that changes transparency of the tooltip.
    /// </summary>
    public class Fade : TooltipAnimation
    {
        private MaterialPropertyBlock tempPropBlock;
        private readonly int colorPropertyId = Shader.PropertyToID("_Color");

        /// <summary>
        /// Property block for material color change.
        /// </summary>
        public MaterialPropertyBlock TempPropBlock => tempPropBlock ??= new MaterialPropertyBlock();

        private void UpdateOpacity(float alpha)
        {
            var curveValue = TooltipAnimCurve.Evaluate(alpha);
            TargetTooltip.TooltipColor = new Vector4(TargetTooltip.TooltipColor.r, TargetTooltip.TooltipColor.g, TargetTooltip.TooltipColor.b, curveValue);

            SetAlphaToRenderer(TargetTooltip.LineRenderer, curveValue);
            SetAlphaToRenderer(TargetTooltip.EndPointRenderer, curveValue);
            SetAlphaToRenderer(TargetTooltip.StartPointRenderer, curveValue);

            var color = TargetTooltip.Txt_Front.color;
            color.a = curveValue;
            TargetTooltip.Txt_Front.color = color;

            color = TargetTooltip.Txt_Back.color;
            color.a = curveValue;
            TargetTooltip.Txt_Back.color = color;
        }

        private void SetAlphaToRenderer(MeshRenderer targetTenderer, float alpha)
        {
            targetTenderer.GetPropertyBlock(TempPropBlock);
            var color = TempPropBlock.GetColor(colorPropertyId);
            color.a = alpha;
            TempPropBlock.SetColor(colorPropertyId, color);
            targetTenderer.SetPropertyBlock(TempPropBlock);
        }

        /// <inheritdoc />
        public override void EvaluateAnimation(float alpha)
        {
            UpdateOpacity(alpha);
        }
    }
}
