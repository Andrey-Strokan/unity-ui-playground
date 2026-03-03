using TMPro;
using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Base class for tooltips.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class TooltipBase : MonoBehaviour
    {
        [Tooltip("The text that is displayed on the tooltip.")]
        [SerializeField]
        [TextArea]
        private string displayText;

        [SerializeField]
        [Tooltip("TMP_Text component reference.")]
        private TMP_Text txt_Front;

        /// <summary>
        /// Front text reference.
        /// </summary>
        public TMP_Text Txt_Front => txt_Front;

        [SerializeField]
        [Tooltip("TMP_Text component reference.")]
        private TMP_Text txt_Back;

        /// <summary>
        /// Back text reference.
        /// </summary>
        public TMP_Text Txt_Back => txt_Back;

        [SerializeField]
        [Tooltip("Tooltip_Background Mesh renderer component reference.")]
        private MeshRenderer backgroundMeshRenderer;

        /// <summary>
        /// Background mesh renderer reference.
        /// </summary>
        public MeshRenderer BackgroundMeshRenderer => backgroundMeshRenderer;

        [SerializeField]
        [Tooltip("TMP_Text Mesh renderer component reference.")]
        protected MeshRenderer Txt_Front_MR;

        [SerializeField]
        [Tooltip("TMP_Text Mesh renderer component reference.")]
        protected MeshRenderer Txt_Back_MR;

        /// <summary>
        /// If tooltip is shown or appearing returns true, otherwise false.
        /// </summary>
        public abstract bool IsShown { get; }

        /// <summary>
        /// Makes tooltip shown after given duration.
        /// </summary>
        /// <param name="duration"></param>
        public abstract void Show(float duration = 1f);

        /// <summary>
        /// Makes tooltip hidden after given duration.
        /// </summary>
        /// <param name="duration"></param>
        public abstract void Hide(float duration = 1f);

        /// <summary>
        /// Set enabled tooltips mesh renderers.
        /// </summary>
        protected abstract void SetEnabledMeshRenderers(bool isEnable);

        /// <summary>
        /// The text that is displayed on the tooltip.
        /// Changing it will update the tooltip.
        /// </summary>
        public string DisplayText
        {
            get => displayText;
            set
            {
                displayText = value;
                UpdateText();
            }
        }

        /// <summary>
        /// Update Text components.
        /// </summary>
        protected void UpdateText()
        {
            if (Txt_Front != null)
            {
                Txt_Front.text = DisplayText;
            }

            if (txt_Back != null)
            {
                txt_Back.text = DisplayText;
            }
        }
    }
}