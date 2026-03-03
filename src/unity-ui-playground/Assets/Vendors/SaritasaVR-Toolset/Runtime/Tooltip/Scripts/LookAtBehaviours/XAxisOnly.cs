using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Rotates tooltip only on X axis.
    /// </summary>
    public class XAxisOnly : LookAtBehaviour
    {
        /// <inheritdoc />
        public override void UpdateTooltipRotation(Transform lookAtTransform)
        {
            var containerRoot = TargetTooltip.ContainerRoot;
            containerRoot.LookAt(lookAtTransform);
            var rotationCache = containerRoot.localEulerAngles;
            rotationCache.y = 0;
            
            if (transform.InverseTransformPoint(lookAtTransform.position).z < 0)
            {
                rotationCache.x = 180 - rotationCache.x;
            }

            containerRoot.localEulerAngles = rotationCache;
        }
    }
}
