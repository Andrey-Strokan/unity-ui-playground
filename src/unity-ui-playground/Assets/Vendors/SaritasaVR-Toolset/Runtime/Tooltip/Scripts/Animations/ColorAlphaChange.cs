using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Animation that changes alpha of the tooltip's material color.
    /// </summary>
    public class ColorAlphaChange : TooltipAnimation
    {
        private MaterialPropertyBlock tempPropBlock;

        /// <summary>
        /// Property block for material color change.
        /// </summary>
        private MaterialPropertyBlock TempPropBlock => tempPropBlock ??= new MaterialPropertyBlock();

        private readonly int colorPropertyId = Shader.PropertyToID("_Color");

        private Color currentColor => TargetTooltip.TooltipColor;

        /// <inheritdoc />
        public override void EvaluateAnimation(float alpha)
        {
            UpdateContainerVisuals(alpha);
        }

        private void UpdateContainerVisuals(float alpha)
        {
            var curveValue = TooltipAnimCurve.Evaluate(alpha);
            TargetTooltip.TooltipColor = new Vector4(currentColor.r, currentColor.g, currentColor.b, curveValue);
            SetAlphaToRenderer(TargetTooltip.LineRenderer, curveValue);
            SetAlphaToRenderer(TargetTooltip.EndPointRenderer, curveValue);
            SetAlphaToRenderer(TargetTooltip.StartPointRenderer, curveValue);
        }
        
        private void SetAlphaToRenderer(MeshRenderer targetRenderer, float alpha)
        {
            targetRenderer.GetPropertyBlock(TempPropBlock);
            var color = TempPropBlock.GetColor(colorPropertyId);
            color.a = alpha;
            TempPropBlock.SetColor(colorPropertyId, color);
            targetRenderer.SetPropertyBlock(TempPropBlock);
        }
    }
}
