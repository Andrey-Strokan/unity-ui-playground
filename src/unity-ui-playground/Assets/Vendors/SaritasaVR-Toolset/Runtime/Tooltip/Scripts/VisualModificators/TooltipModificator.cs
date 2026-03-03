using UnityEngine;
using UnityEngine.Serialization;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Base class for tooltip modification.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class TooltipModificator : MonoBehaviour
    {
        [SerializeField]
        private TooltipWithLines targetTooltip;

        /// <summary>
        /// Target tooltip reference.
        /// </summary>
        public TooltipWithLines TargetTooltip => targetTooltip;
        
        protected virtual void Awake()
        {
            FindTargetTooltip();
            ModifyTooltip();
        }

        /// <summary>
        /// Method for tooltip modification. Works at runtime and editor.
        /// </summary>
        protected abstract void ModifyTooltip();

        private void FindTargetTooltip()
        {
            if (TargetTooltip == null)
            {
                TryGetComponent(out targetTooltip);
            }
        }

        protected virtual void OnValidate()
        {
            FindTargetTooltip();
            ModifyTooltip();
        }
    }
}
