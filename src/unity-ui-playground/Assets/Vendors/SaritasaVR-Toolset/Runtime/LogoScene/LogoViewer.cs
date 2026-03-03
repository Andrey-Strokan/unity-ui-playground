using System.Collections;
using UnityEngine;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Logo viewer for showing logos.
    /// </summary>
    public class LogoViewer : MonoBehaviour
    {
        [SerializeField]
        private LogoCanvas logoCanvas;

        [SerializeField]
        private LogoSequenceModel logoSequenceModel;

        [SerializeField]
        private float fadeDuration;

        [SerializeField]
        private Transform followTarget;

        private void Awake()
        {
            logoCanvas.Initialize(followTarget);
        }

        private void Start()
        {
            StartCoroutine(ShowLogoSequenceCoroutine());
        }

        /// <summary>
        /// Show logo sequence.
        /// </summary>
        public IEnumerator ShowLogoSequenceCoroutine()
        {
            foreach (var sequenceElement in logoSequenceModel.LogoSequence)
            {
                yield return StartCoroutine(logoCanvas.ShowLogoCoroutine(sequenceElement, fadeDuration));
                yield return new WaitForSeconds(sequenceElement.DisplayDuration);
                yield return StartCoroutine(logoCanvas.HideLogoCoroutine(fadeDuration));
            }
        }
    }
}