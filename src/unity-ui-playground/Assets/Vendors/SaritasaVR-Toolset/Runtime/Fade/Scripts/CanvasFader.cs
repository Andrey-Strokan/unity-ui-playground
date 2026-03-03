using System;
using System.Collections;
using UnityEngine;

// TODO: Review usage in project. Delete if needed for examples only.
/// <summary>
/// Class that handels CanvasGroup Alpha fade in/out.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour, IFader
{
    protected CanvasGroup canvasGroup;

    protected Coroutine changeAlphaCoroutine;

    protected Coroutine fadeCoroutine;

    [SerializeField]
    [Tooltip("Is must be interactable.")]
    protected bool isMustBeInteractable = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Changes canvas group alpha to 0 over given duration.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
    public virtual IEnumerator FadeRoutine(float duration = 1f, Action callback = null)
    {
        if (changeAlphaCoroutine != null)
        {
            StopCoroutine(changeAlphaCoroutine);
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        changeAlphaCoroutine = StartCoroutine(ChangeAlpha(false, duration));
        yield return changeAlphaCoroutine;
        callback?.Invoke();
    }

    /// <summary>
    /// Changes canvas group alpha to 1 or max value over given duration.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="maxAlpha"></param>
    /// <param name="callback"></param>
    public virtual IEnumerator UnFadeRoutine(float duration = 1f, float maxAlpha = 1f, Action callback = null)
    {
        if (changeAlphaCoroutine != null)
        {
            StopCoroutine(changeAlphaCoroutine);
        }

        canvasGroup.interactable = isMustBeInteractable;
        canvasGroup.blocksRaycasts = isMustBeInteractable;

        changeAlphaCoroutine = StartCoroutine(ChangeAlpha(true, duration, maxAlpha));
        yield return changeAlphaCoroutine;
        callback?.Invoke();
    }

    /// <summary>
    /// Function that evaluates alpha value.
    /// </summary>
    /// <param name="isFadeForward"></param>
    /// <param name="duration"></param>
    /// <param name="maxAlpha"></param>
    protected virtual IEnumerator ChangeAlpha(bool isFadeForward, float duration, float maxAlpha = 1f)
    {
        var targetAlpha = isFadeForward ? maxAlpha : 0;
        if (duration <= 0)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (Mathf.Clamp(canvasGroup.alpha, 0, maxAlpha) != targetAlpha)
        {
            if (isFadeForward)
            {
                canvasGroup.alpha += Time.deltaTime * 1f / duration;
            }
            else
            {
                canvasGroup.alpha -= Time.deltaTime * 1f / duration;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Set fader canvas alpha.
    /// </summary>
    /// <param name="value">Alpha value.</param>
    public void SetAlpha(float value)
    {
        canvasGroup.alpha = value;
    }

    /// <summary>
    /// Changes canvas group alpha to 0 over given duration.
    /// Returning void implementation.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
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
    /// <param name="duration"></param>
    /// <param name="maxAlpha"></param>
    /// <param name="callback"></param>
    public void UnFade(float duration = 1, float maxAlpha = 1f, Action callback = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(UnFadeRoutine(duration, maxAlpha, callback));
    }
}