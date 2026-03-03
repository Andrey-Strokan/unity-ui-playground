using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Colors overriders for line and points of the tooltip.
    /// </summary>
    [DisallowMultipleComponent]
    public class TooltipLineColors : TooltipModificator
    {
        [SerializeField]
        private Color endPointColor = Color.white;

        [SerializeField]
        private Color lineColor = Color.white;

        [SerializeField]
        private Color startPointColor = Color.white;

        private MaterialPropertyBlock propBlock;
        
        private readonly int colorPropertyId = Shader.PropertyToID("_Color");

        /// <summary>
        /// Property block for material color change.
        /// </summary>
        public MaterialPropertyBlock PropBlock => propBlock ??= new MaterialPropertyBlock();

        protected override void ModifyTooltip()
        {
            PropBlock.SetColor(colorPropertyId, endPointColor);
            TargetTooltip.EndPointRenderer.SetPropertyBlock(PropBlock);

            PropBlock.SetColor(colorPropertyId, lineColor);
            TargetTooltip.LineRenderer.SetPropertyBlock(PropBlock);
            
            PropBlock.SetColor(colorPropertyId, startPointColor);
            TargetTooltip.StartPointRenderer.SetPropertyBlock(PropBlock);
        }
    }
}
