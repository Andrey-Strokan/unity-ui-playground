using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Saritasa.VRToolset.Highlight
{
    /// <summary>
    /// Class responsible for highlight management.
    /// </summary>
    public class HighlightSystem : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Boolean that identifies whether this object is highlighted.")]
        private bool isHighlighted;

        [SerializeField]
        [Tooltip("Highlight provider that contains actual logic.")]
        private HighlightProvider highlightProvider;

        [SerializeField]
        [Tooltip("Event that fired when highlight started.")]
        private UnityEvent highlightStarted = new UnityEvent();

        [SerializeField]
        [Tooltip("Event that fired when highlight finished.")]
        private UnityEvent highlightFinished = new UnityEvent();

        [SerializeField]
        [Tooltip("Event that fired when unhighlight started.")]
        private UnityEvent unhighlightStarted = new UnityEvent();

        [SerializeField]
        [Tooltip("Event that fired when unhighlight finished.")]
        private UnityEvent unhighlightFinished = new UnityEvent();

        [SerializeField]
        [Tooltip("Event that fired when highlight/unhighlight started.")]
        private UnityEvent animationStarted = new UnityEvent();

        [SerializeField]
        [Tooltip("Event that fired when highlight/unhighlight finished.")]
        private UnityEvent animationFinished = new UnityEvent();

        /// <summary>
        /// Event that fired when highlight started.
        /// </summary>
        public UnityEvent HighlightStarted => highlightStarted;

        /// <summary>
        /// Event that fired when highlight finished.
        /// </summary>
        public UnityEvent HighlightFinished => highlightFinished;

        /// <summary>
        /// Event that fired when unhighlight started.
        /// </summary>
        public UnityEvent UnhighlightStarted => unhighlightStarted;

        /// <summary>
        /// Event that fired when unhighlight finished.
        /// </summary>
        public UnityEvent UnhighlightFinished => unhighlightFinished;

        /// <summary>
        /// Event that fired when highlight/unhighlight started.
        /// </summary>
        public UnityEvent AnimationStarted => animationStarted;

        /// <summary>
        /// Event that fired when highlight/unhighlight finished.
        /// </summary>
        public UnityEvent AnimationFinished => animationFinished;

        /// <summary>
        /// Boolean that identifies whether any animation is playing.
        /// </summary>
        public bool IsAnimated { get; private set; }

        /// <summary>
        /// Boolean that identifies whether this object is highlighted.
        /// </summary>
        public bool IsHighlighted => isHighlighted;

        private IEnumerator currentCoroutine;

        private void Update()
        {
            ProcessCoroutine();
        }

        private void Start()
        {
            highlightProvider.SetState(isHighlighted);
        }

        private void DeactivateHighlight()
        {
            IsAnimated = false;
            isHighlighted = true;
            highlightFinished?.Invoke();
            animationFinished?.Invoke();
        }

        private void DeactivateUnhighlight()
        {
            IsAnimated = false;
            isHighlighted = false;
            unhighlightFinished?.Invoke();
            animationFinished?.Invoke();
        }

        /// <summary>
        /// Method that highlights object.
        /// </summary>
        public void Highlight()
        {
            Highlight(null);
        }

        /// <summary>
        /// Method that highlights object.
        /// </summary>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public void Highlight(Action onFinished)
        {
            Highlight(highlightProvider, onFinished);
        }

        /// <summary>
        /// Method that highlights object.
        /// </summary>
        /// <param name="provider">Provider used to highlight object.</param>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public void Highlight(HighlightProvider provider, Action onFinished)
        {
            Action finishAction;
            if (onFinished == null)
            {
                finishAction = DeactivateHighlight;
            }
            else
            {
                finishAction = (Action)Delegate.Combine(onFinished, (Action)DeactivateHighlight);
            }

            IsAnimated = true;
            highlightStarted?.Invoke();
            animationStarted?.Invoke();

            currentCoroutine = provider.Highlight(finishAction);
        }

        /// <summary>
        /// Method that unhighlights object.
        /// </summary>
        public void Unhighlight()
        {
            Unhighlight(null);
        }

        /// <summary>
        /// Method that unhighlights object.
        /// </summary>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public void Unhighlight(Action onFinished)
        {
            Unhighlight(highlightProvider, onFinished);
        }

        /// <summary>
        /// Method that unhighlights object.
        /// </summary>
        /// <param name="provider">Provider used to unhighlight object.</param>
        /// <param name="onFinished">Callback that called when animation finished.</param>
        public void Unhighlight(HighlightProvider provider, Action onFinished)
        {
            Action finishAction;
            if (onFinished == null)
            {
                finishAction = DeactivateUnhighlight;
            }
            else
            {
                finishAction = (Action)Delegate.Combine(onFinished, (Action)DeactivateUnhighlight);
            }

            IsAnimated = true;
            unhighlightStarted?.Invoke();
            animationStarted?.Invoke();

            currentCoroutine = provider.Unhighlight(finishAction);
        }

        private bool ProcessCoroutine()
        {
            return currentCoroutine != null && currentCoroutine.MoveNext();
        }
    }
}