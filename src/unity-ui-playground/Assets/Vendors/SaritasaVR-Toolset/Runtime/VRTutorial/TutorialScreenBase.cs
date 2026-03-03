using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any implementation of a tutorial screen. Derive from this class
/// to implement your custom tutorial screen.
/// </summary>
public abstract class TutorialScreenBase : MonoBehaviour
{
    /// <summary>
    /// Called to show or hide the screen in the scene.
    /// </summary>
    public abstract void Show(bool show);

    /// <summary>
    /// Called to set text on the screen.
    /// </summary>
    public abstract void SetText(string title, string message);
}
