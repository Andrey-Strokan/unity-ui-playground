using UnityEngine;
using UnityEngine.Serialization;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Rotates tooltip smoothly on X and Y axes.
    /// </summary>
    public class XYAxisSmooth : LookAtBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        private float smoothness = 0.95f;

        /// <inheritdoc />
        public override void UpdateTooltipRotation(Transform lookAtTransform)
        {
            var containerRoot = TargetTooltip.ContainerRoot;
            var targetRotation = Quaternion.LookRotation(lookAtTransform.position - containerRoot.position);

            containerRoot.rotation = Quaternion.Slerp(containerRoot.rotation, targetRotation, 1 - smoothness);
        }
    }
}
