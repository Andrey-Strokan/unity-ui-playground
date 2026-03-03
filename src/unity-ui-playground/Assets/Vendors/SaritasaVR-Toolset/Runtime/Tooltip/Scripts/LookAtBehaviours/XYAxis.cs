using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Rotates tooltip on X and Y axes.
    /// </summary>
    public class XYAxis : LookAtBehaviour
    {
        /// <inheritdoc />
        public override void UpdateTooltipRotation(Transform lookAtTransform)
        {
            TargetTooltip.ContainerRoot.LookAt(lookAtTransform);
        }
    }
}
