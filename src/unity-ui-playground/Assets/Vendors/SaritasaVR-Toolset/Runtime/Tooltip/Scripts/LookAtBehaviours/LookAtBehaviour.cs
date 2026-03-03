using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Base class for tooltip's rotation at the specific target.
    /// </summary>
    public abstract class LookAtBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Target tooltip reference.
        /// </summary>
        public TooltipWithLines TargetTooltip
        {
            get => targetTooltip;
            set => targetTooltip = value;
        }

        [SerializeField]
        private TooltipWithLines targetTooltip;

        private Transform headset;

        protected virtual void OnEnable()
        {
            headset = Camera.main.transform;

            if (TargetTooltip == null)
            {
                TargetTooltip = GetComponent<TooltipWithLines>();
            }
        }

        private void Update()
        {
            if (headset != null && TargetTooltip != null)
            {
                UpdateTooltipRotation(headset);
            }
        }

        /// <summary>
        /// Updates the rotation of tooltip's pivot object.
        /// </summary>
        /// <param name="lookAtTransform">Target transform for LookAt logic.</param>
        public abstract void UpdateTooltipRotation(Transform lookAtTransform);
    }
}
