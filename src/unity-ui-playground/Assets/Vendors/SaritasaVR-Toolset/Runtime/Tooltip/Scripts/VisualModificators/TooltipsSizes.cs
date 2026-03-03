using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Allows setup sizes of the tooltip's parts.
    /// </summary>
    [DisallowMultipleComponent]
    public class TooltipsSizes : TooltipModificator
    {
        private const float LineWidthConversion = 0.0015f;
        
        [SerializeField]
        private float startPointSize = 1f;

        [SerializeField]
        private float endPointSize = 1f;
        
        [SerializeField]
        private float lineWidth = 1f;

        [SerializeField]
        private Vector2 backgroundSize = Vector2.one;

        [SerializeField]
        private Vector2 additionalCornerOffset;

        [SerializeField]
        private Vector2 additionalMidpointOffset;

        /// <inheritdoc />
        protected override void ModifyTooltip()
        {
            ModifySizes();
        }

        private void ModifySizes()
        {
            var scaleOffset = TargetTooltip.DefaultScale;
            TargetTooltip.LineWidth = lineWidth * LineWidthConversion;
            TargetTooltip.BackgroundMeshRenderer.transform.localScale = new Vector3(
                backgroundSize.x * scaleOffset.x,
                backgroundSize.y * scaleOffset.y,
                1);

            TargetTooltip.EndPointRenderer.transform.localScale = new Vector3(endPointSize, endPointSize, endPointSize);
            TargetTooltip.StartPointRenderer.transform.localScale = new Vector3(startPointSize, startPointSize, startPointSize);

            TargetTooltip.AdditionalCornerOffset = additionalCornerOffset;
            TargetTooltip.AdditionalMidpointOffset = additionalMidpointOffset;
        }
    }
}
