using System;
using System.Collections;
using UnityEngine;

// TODO: Add a namespace for fader scripts.
/// <summary>
/// Class that handels mesh Alpha fade in/out.
/// </summary>
public class SphereFader : MonoBehaviour, IFader
{
    [SerializeField]
    [Tooltip("Fader mesh renderer.")]
    private MeshRenderer meshRender;

    private static readonly int fadeOpacity = Shader.PropertyToID("_FadeOpacity");

    private MaterialPropertyBlock fadePropertyBlock;

    private Coroutine changeAlphaCoroutine;

    private Coroutine fadeCoroutine;

    /// <summary>
    /// Alpha of fader.
    /// </summary>
    public float Alpha
    {
        get
        {
            fadePropertyBlock ??= new MaterialPropertyBlock();
            meshRender.GetPropertyBlock(fadePropertyBlock);
            var alpha = fadePropertyBlock.GetFloat(fadeOpacity);
            return alpha;
        }
        set
        {
            fadePropertyBlock ??= new MaterialPropertyBlock();
            meshRender.GetPropertyBlock(fadePropertyBlock);
            fadePropertyBlock.SetFloat(fadeOpacity, value);
            meshRender.SetPropertyBlock(fadePropertyBlock);
        }
    }

    /// <summary>
    /// Changes canvas group alpha to 0 over given duration.
    /// </summary>
    public IEnumerator FadeRoutine(float duration = 1f, Action callback = null)
    {
        if (changeAlphaCoroutine != null)
        {
            StopCoroutine(changeAlphaCoroutine);
        }

        changeAlphaCoroutine = StartCoroutine(ChangeAlpha(0f, duration));
        yield return changeAlphaCoroutine;
        callback?.Invoke();
    }

    /// <summary>
    /// Changes canvas group alpha to 1 or max value over given duration.
    /// </summary>
    public IEnumerator UnFadeRoutine(float duration = 1f, float maxAlpha = 1f, Action callback = null)
    {
        if (changeAlphaCoroutine != null)
        {
            StopCoroutine(changeAlphaCoroutine);
        }

        changeAlphaCoroutine = StartCoroutine(ChangeAlpha(maxAlpha, duration));
        yield return changeAlphaCoroutine;
        callback?.Invoke();
    }

    /// <summary>
    /// Function that evaluates alpha value.
    /// </summary>
    private IEnumerator ChangeAlpha(float targetAlpha, float duration)
    {
        if (duration <= 0)
        {
            Alpha = targetAlpha;
            yield break;
        }

        var elapsedTime = 0.0f;
        var startAlpha = Alpha;

        while (elapsedTime < duration)
        {
            Alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Alpha = targetAlpha;
    }

    /// <summary>
    /// Changes canvas group alpha to 0 over given duration.
    /// Returning void implementation.
    /// </summary>
    public void Fade(float duration = 1, Action callback = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(duration, callback));
    }

    /// <summary>
    /// Changes canvas group alpha to 1 or max value over given duration.
    /// Returning void implementation.
    /// </summary>
    public void UnFade(float duration = 1, float maxAlpha = 1f, Action callback = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(UnFadeRoutine(duration, maxAlpha, callback));
    }
}