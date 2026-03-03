using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Rotates tooltip only on Y axis.
    /// </summary>
    public class YAxisOnly : LookAtBehaviour
    {
        /// <inheritdoc />
        public override void UpdateTooltipRotation(Transform lookAtTransform)
        {
            var containerRoot = TargetTooltip.ContainerRoot;
            var dotProduct = Vector3.Dot(transform.up, lookAtTransform.forward);
            var newRotation = Quaternion.LookRotation(lookAtTransform.position - containerRoot.position, Vector3.up);

            newRotation = Quaternion.Lerp(newRotation, containerRoot.rotation, Mathf.Abs(dotProduct));
            containerRoot.rotation = newRotation;

            var rotationCache = containerRoot.localEulerAngles;
            rotationCache.x = 0;
            rotationCache.z = 0;

            containerRoot.localEulerAngles = rotationCache;
        }
    }
}
