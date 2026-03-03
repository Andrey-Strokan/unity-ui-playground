using System;
using System.Collections;

// TODO: Reconsider the need for a callback when calling faders.
/// <summary>
/// Fader interface to change some alpha over time.
/// </summary>
public interface IFader
{
    /// <summary>
    /// Intended to change alpha value in forward direction over time and invoke callback in the end.
    /// Coroutine implementation.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator FadeRoutine(float duration = 1f, Action callback = null);

    /// <summary>
    /// Intended to change alpha value in backward direction over time and invoke callback in the end.
    /// Coroutine implementation.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="maxAlpha"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator UnFadeRoutine(float duration = 1f, float maxAlpha = 1f, Action callback = null);

    /// <summary>
    /// Intended to change alpha value in forward direction over time and invoke callback in the end.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
    public void Fade(float duration = 1f, Action callback = null);

    /// <summary>
    /// Intended to change alpha value in backward direction over time and invoke callback in the end.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="maxAlpha"></param>
    /// <param name="callback"></param>
    public void UnFade(float duration = 1f, float maxAlpha = 1f, Action callback = null);
}