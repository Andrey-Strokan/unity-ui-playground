using System;
using System.Collections;
using UnityEngine;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Base class for animated tooltips.
    /// </summary>
    [RequireComponent(typeof(TooltipAnimation))]
    [ExecuteInEditMode]
    public class AnimatedTooltip : TooltipWithLines
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float alpha;

        [SerializeField]
        private TooltipAnimation tooltipAnimation;

        /// <summary>
        /// Invoked when Alpha property is changed.
        /// </summary>
        public event Action<float> AlphaChanged;

        /// <summary>
        /// Alpha of the tooltip.
        /// 0 means the tooltip is hidden.
        /// 1 means the tooltip is shown.
        /// </summary>
        public float Alpha
        {
            get => alpha;
            set 
            {
                alpha = Mathf.Clamp01(value);
                tooltipAnimation.EvaluateAnimation(Alpha);
                MeshRenderersVisibilityCheck();
                AlphaChanged?.Invoke(alpha);
            }
        }

        /// <inheritdoc />
        public override bool IsShown => Alpha > 0;

        private Coroutine fadeCoroutine;

        private IEnumerator Evaluate(Func<float> getter, Action<float> setter, float dest, float speed)
        {
            float addition;
            float currentValue;
            speed = Mathf.Abs(speed);

            while (true)
            {
                addition = Time.deltaTime * speed;
                currentValue = getter();

                if (currentValue < dest)
                {
                    setter(Mathf.Clamp(currentValue + addition, float.MinValue, dest));
                }
                else
                {
                    setter(Mathf.Clamp(currentValue - addition, dest, float.MaxValue));
                }

                if (Mathf.Approximately(getter(), dest))
                {
                    yield break;
                }

                yield return null;
            }
        }

        private void Fade(Func<float> getter, Action<float> setter, float dest, float speed)
        {
            if (!isActiveAndEnabled)
                return;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(Evaluate(getter, setter, dest, speed));
        }

        /// <inheritdoc />
        public override void Show(float duration = 1f)
        {
            if (Mathf.Approximately(Alpha, 1f))
            {
                return;
            }

            if (duration <= 0f)
            {
                Alpha = 1f;
                return;
            }

            var invertedAlpha = 1f - Alpha;
            var speed = ConvertDurationToSpeed(invertedAlpha, duration * invertedAlpha);
            Fade(() => Alpha, 
                (x) => { Alpha = x; }, 
                1f, 
                speed);
        }

        /// <inheritdoc />
        public override void Hide(float duration = 1f)
        {
            if (Mathf.Approximately(Alpha, 0f))
            {
                return;
            }

            if (duration <= 0f)
            {
                Alpha = 0f;
                return;
            }

            var speed = ConvertDurationToSpeed(Alpha, duration * Alpha);
            Fade(() => Alpha, 
                (x) => { Alpha = x; }, 
                0f, 
                speed);
        }

        private float ConvertDurationToSpeed(float distance, float duration)
        {
            return distance / duration;
        }

        private void MeshRenderersVisibilityCheck()
        {
            SetEnabledMeshRenderers(Alpha > 0);
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (tooltipAnimation == null)
            {
                return;
            }
            
            tooltipAnimation.EvaluateAnimation(Alpha);
            MeshRenderersVisibilityCheck();
        }
#endif
    }
}