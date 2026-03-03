using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Class for view logo.
    /// </summary>
    public class LogoCanvas : MonoBehaviour
    {
        [SerializeField]
        private LazyFollow lazyFollow;

        [SerializeField]
        private RawImage img_Logo;

        [SerializeField]
        private AspectRatioFitter logoAspectRatioFitter;

        [SerializeField]
        private VideoPlayer videoPlayer;

        private RenderTexture temporary;

        /// <summary>
        /// Initialize canvas logo.
        /// </summary>
        public void Initialize(Transform followTarget)
        {
            img_Logo.color = new(1, 1, 1, 0);
            lazyFollow.target = followTarget;
        }

        /// <summary>
        /// Show logo with fade duration.
        /// </summary>
        public IEnumerator ShowLogoCoroutine(LogoSequenceModel.LogoSequenceElement logo, float fadeDuration)
        {
            if (logo.LogoTexture != null)
            {
                yield return StartCoroutine(ShowTextureLogoCoroutine(logo, fadeDuration));
            }
            else if (logo.LogoVideo != null)
            {
                yield return StartCoroutine(ShowVideoLogoCoroutine(logo));
            }

            yield return StartCoroutine(FadeLogoCoroutine(1.0f, fadeDuration));
        }

        /// <summary>
        /// Hide logo with fade duration.
        /// </summary>
        public IEnumerator HideLogoCoroutine(float duration)
        {
            yield return StartCoroutine(FadeLogoCoroutine(0.0f, duration));

            if (temporary != null)
            {
                RenderTexture.ReleaseTemporary(temporary);
            }
        }

        private IEnumerator ShowVideoLogoCoroutine(LogoSequenceModel.LogoSequenceElement logo)
        {
            if (logo.LogoVideo == null)
            {
                Debug.LogError("The video clip of the logo is missing", this);

                yield break;
            }

            var video = logo.LogoVideo;

            if (temporary != null)
            {
                RenderTexture.ReleaseTemporary(temporary);
            }

            temporary = RenderTexture.GetTemporary((int)video.width, (int)video.height, 0);

            videoPlayer.targetTexture = temporary;
            videoPlayer.clip = video;

            var color = img_Logo.color;
            color.a = 1;

            img_Logo.color = color;
            img_Logo.texture = temporary;
            logoAspectRatioFitter.aspectRatio = (float)temporary.width / temporary.height;

            videoPlayer.Play();

            yield return new WaitForSeconds((float)video.length);
        }

        private IEnumerator ShowTextureLogoCoroutine(LogoSequenceModel.LogoSequenceElement logo, float duration)
        {
            if (logo.LogoTexture == null)
            {
                Debug.LogError("The image of the logo is missing", this);

                yield break;
            }

            var image = logo.LogoTexture;

            img_Logo.texture = image;
            logoAspectRatioFitter.aspectRatio = (float)image.width / image.height;

            yield return StartCoroutine(FadeLogoCoroutine(1.0f, duration));
        }

        private IEnumerator FadeLogoCoroutine(float value, float duration)
        {
            value = Mathf.Clamp01(value);

            var color = img_Logo.color;
            var multiplier = value == 0 ? 1 : -1;

            while (value == 0 ? img_Logo.color.a > 0 : img_Logo.color.a < 1)
            {
                color.a = Mathf.Clamp01(color.a - Time.deltaTime / duration * multiplier);
                img_Logo.color = color;
                yield return null;
            }
        }
    }
}