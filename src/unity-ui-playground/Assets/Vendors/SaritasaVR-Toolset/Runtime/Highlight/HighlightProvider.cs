using System;
using System.Collections;
using UnityEngine;

namespace Saritasa.VRToolset.Highlight
{
    /// <summary>
    /// Base generic class for highlight provider.
    /// </summary>
    /// <typeparam name="TSettings">Settings type to use.</typeparam>
    public abstract class HighlightProvider<TSettings> : HighlightProvider where TSettings : HighlightProvider.Settings
    {
        [SerializeField]
        [Tooltip("Settings that used during highlight.")]
        protected TSettings HighlightSettings;

        [SerializeField]
        [Tooltip("Settings that used during unhighlight.")]
        protected TSettings UnhighlightSettings;

        [SerializeField]
        [Tooltip("Animation curve used to blend current state and new state.")]
        protected AnimationCurve BlendCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Highlight coroutine that used for both Highlight and Unhighlight.
        /// </summary>
        /// <param name="settings">Highlight settings.</param>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        protected abstract IEnumerator Highlight(TSettings settings, Action onFinished);

        /// <inheritdoc/>
        protected sealed override IEnumerator Highlight(Settings settings, Action onFinished)
        {
            return Highlight(settings as TSettings, onFinished);
        }

        /// <inheritdoc/>
        protected override Settings GetHighlightSettings()
        {
            return HighlightSettings;
        }

        /// <inheritdoc/>
        protected override Settings GetUnhighlightSettings()
        {
            return UnhighlightSettings;
        }
    }

    /// <summary>
    /// Base class for highlight provider.
    /// </summary>
    public abstract class HighlightProvider : MonoBehaviour
    {
        /// <summary>
        /// Class that contains fields for animation.
        /// </summary>
        [Serializable]
        public class Settings
        {
            /// <summary>
            /// Duration of animation.
            /// </summary>
            public float Duration = 0.5f;

            /// <summary>
            /// Delta time used to increment if coroutines.
            /// </summary>
            public float DeltaTime => Time.deltaTime / Duration;
        }

        /// <summary>
        /// Method that returns highlight settings.
        /// </summary>
        protected abstract Settings GetHighlightSettings();

        /// <summary>
        /// Method that returns unhighlight settings.
        /// </summary>
        protected abstract Settings GetUnhighlightSettings();

        /// <summary>
        /// Highlight coroutine that used for both Highlight and Unhighlight.
        /// </summary>
        /// <param name="settings">Highlight settings.</param>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        protected abstract IEnumerator Highlight(Settings settings, Action onFinished);

        /// <summary>
        /// Highlight coroutine. 
        /// </summary>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public IEnumerator Highlight(Action onFinished)
        {
            return Highlight(GetHighlightSettings(), onFinished);
        }

        /// <summary>
        /// Unhighlight coroutine. 
        /// </summary>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public IEnumerator Unhighlight(Action onFinished)
        {
            return Highlight(GetUnhighlightSettings(), onFinished);
        }

        /// <summary>
        /// Method used to set highlight state without animation.
        /// </summary>
        /// <param name="isHighlighted">Is object highlighted.</param>
        public abstract void SetState(bool isHighlighted);
    }
}