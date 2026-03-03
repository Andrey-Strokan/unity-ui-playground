using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Animation that fills tooltip circularly.
    /// </summary>
    public class CircleFill : TooltipAnimation
    {
        /// <inheritdoc />
        public override void EvaluateAnimation(float alpha)
        {
            UpdateContainerVisuals(alpha);
            UpdateLineScale(alpha);
        }

        [SerializeField]
        private AnimationCurve startPointAnimCurve;

        [SerializeField]
        private AnimationCurve endPointAnimCurve;

        private readonly int fillRadiusPropertyId = Shader.PropertyToID("_FillRadius");

        /// <summary>
        /// Renderer for material animation.
        /// </summary>
        public Renderer BackgroundRenderer => TargetTooltip.BackgroundMeshRenderer;

        private Vector2 centerOfFill;

        private float CalculateFillRadius()
        {
            var scale = BackgroundRenderer.transform.localScale;
            var isCorner = TargetTooltip.IsRightOrLeftAlignment;
            if (!isCorner)
            {
                scale /= 2f;
            }

            return Mathf.Max(scale.x, scale.y) + 0.02f;
        }

        private float fillRadius;
        
        private readonly int centerPropertyId = Shader.PropertyToID("_Center");

        private void SetSettingsPresetForShader()
        {
            switch (TargetTooltip.TooltipAlignment)
            {
                case TooltipAlignment.BottomRight:
                    centerOfFill = new Vector2(1f, 0f);
                    break;
                case TooltipAlignment.MiddleRight:
                    centerOfFill = new Vector2(1f, 0.5f);
                    break;
                case TooltipAlignment.TopRight:
                    centerOfFill = new Vector2(1f, 1f);
                    break;
                case TooltipAlignment.BottomCenter:
                    centerOfFill = new Vector2(0.5f, 0);
                    break;
                case TooltipAlignment.MiddleCenter:
                    centerOfFill = new Vector2(0.5f, 0.5f);
                    break;
                case TooltipAlignment.TopCenter:
                    centerOfFill = new Vector2(0.5f, 1f);
                    break;
                case TooltipAlignment.BottomLeft:
                    centerOfFill = new Vector2(0f, 0f);
                    break;
                case TooltipAlignment.MiddleLeft:
                    centerOfFill = new Vector2(0f, 0.5f);
                    break;
                case TooltipAlignment.TopLeft:
                    centerOfFill = new Vector2(0f, 1f);
                    break;
                default:
                    break;
            }

            BackgroundRenderer.GetPropertyBlock(TooltipPropBlock);
            fillRadius = CalculateFillRadius();
            TooltipPropBlock.SetVector(centerPropertyId, new Vector4(centerOfFill.x, centerOfFill.y));
            BackgroundRenderer.SetPropertyBlock(TooltipPropBlock);
        }

        private void UpdateContainerVisuals(float alpha)
        {
            SetSettingsPresetForShader();
            var curveValue = TooltipAnimCurve.Evaluate(alpha);
            BackgroundRenderer.GetPropertyBlock(TooltipPropBlock);

            TooltipPropBlock.SetFloat(fillRadiusPropertyId, curveValue * fillRadius);
            BackgroundRenderer.SetPropertyBlock(TooltipPropBlock);
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
