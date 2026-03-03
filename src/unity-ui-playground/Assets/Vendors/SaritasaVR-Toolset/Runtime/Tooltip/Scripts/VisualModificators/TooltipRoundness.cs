using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Allows setup roundness of the tooltip.
    /// </summary>
    [DisallowMultipleComponent]
    public class TooltipRoundness : TooltipModificator
    {
        [SerializeField]
        [Range(0,1)]
        private float roundness = 0f;

        private static readonly int RoundnessPropertyId = Shader.PropertyToID("_Roundness");

        /// <inheritdoc />
        protected override void ModifyTooltip()
        {
            ModifyRoundness();
        }

        private void ModifyRoundness()
        {
            var propBlock = TargetTooltip.BackGroundPropBlock;
            propBlock.SetFloat(RoundnessPropertyId, roundness * TargetTooltip.DefaultScale.y);
        }
    }
}
