using System;
using System.Collections;
using UnityEngine;

namespace Saritasa.VRToolset.Highlight
{
    /// <summary>
    /// Highlight provider that uses material.
    /// </summary>
    public class MaterialHighlightProvider : HighlightProvider<MaterialHighlightProvider.Settings>
    {
        [SerializeField]
        [Tooltip("Highlight to modify.")]
        private Highlight highlight;

        /// <inheritdoc/>
        [Serializable]
        public new class Settings : HighlightProvider.Settings
        {
            /// <summary>
            /// Gradient used in animation.
            /// </summary>
            public Gradient Gradient;
        }

        /// <inheritdoc/>
        protected override IEnumerator Highlight(Settings settings, Action onFinished)
        {
            Color startColor = highlight.Color;

            for (float t = 0; t < 1; t += settings.DeltaTime)
            {
                Modify(settings, t, startColor);
                yield return null;
            }

            Modify(settings, 1, startColor);
            onFinished?.Invoke();
        }

        private void Modify(Settings settings, float t, Color startColor)
        {
            float blendTime = BlendCurve.Evaluate(t);
            Color color = settings.Gradient.Evaluate(t);
            color = Color.Lerp(startColor, color, blendTime);
            highlight.Color = color;
        }

        /// <inheritdoc/>
        public override void SetState(bool isHighlighted)
        {
            if (isHighlighted)
            {
                Modify(HighlightSettings, 1, Color.clear);
            }
            else
            {
                Modify(UnhighlightSettings, 1, Color.clear);
            }
        }
    }
}