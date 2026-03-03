using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Allows setup border for the tooltip.
    /// </summary>
    [DisallowMultipleComponent]
    public class TooltipBorder : TooltipModificator
    {
        [SerializeField]
        private Color borderColor = Color.white;

        [SerializeField]
        [Range(0, 1)]
        private float borderWidth = 0f;

        [SerializeField]
        private float borderTransition = 0f;

        private readonly int borderColorPropertyId = Shader.PropertyToID("_BorderColor");
        private readonly int borderWidthPropertyId = Shader.PropertyToID("_BorderWidth");
        private readonly int borderTransitionPropertyId = Shader.PropertyToID("_BorderTransition");

        /// <inheritdoc />
        protected override void ModifyTooltip()
        {
            ModifyBorderParameters();
        }

        private void ModifyBorderParameters()
        {
            var propBlock = TargetTooltip.BackGroundPropBlock;
            propBlock.SetColor(borderColorPropertyId, borderColor);
            propBlock.SetFloat(borderWidthPropertyId, borderWidth * TargetTooltip.DefaultScale.y);
            propBlock.SetFloat(borderTransitionPropertyId, borderTransition * TargetTooltip.DefaultScale.y);
        }
    }
}
