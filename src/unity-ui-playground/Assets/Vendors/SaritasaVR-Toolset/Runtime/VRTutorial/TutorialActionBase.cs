using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for tutorial logic. Used for implementation of logic for tutorial steps.
/// Name of the derived class should match tha name of tutorial step.
/// </summary>
public abstract class TutorialActionBase
{
    /// <summary>
    /// Reference to a tutorial manager.
    /// </summary>
    protected TutorialManager manager { get; private set; }

    /// <summary>
    /// Reference to a tutorial context.
    /// </summary>
    protected TutorialContextBase context { get; private set; }

    /// <summary>
    /// This method is called when tutorial step is started. Use it to initialize objects,
    /// subscribe events etc.
    /// </summary>
    protected virtual void Begin() {}

    /// <summary>
    /// This coroutine is intended to be used for implementation of tutorial logic.
    /// Use yield statements to wait for completion of step conditions. When all conditions
    /// satisfied call manager.NextStage() inside of this coroutine to perceed to the next step.
    /// </summary>
    public abstract IEnumerator ActionCoroutine();

    /// <summary>
    /// This method is called when tutorial action is completed or when tutorial is aborted,
    /// For example when player exits scene. Use it to release any resources used in this step
    /// or to unsubscribe events.
    /// </summary>
    protected virtual void End() {}

    /// <summary>
    /// This method used for initialization of external references passed to this step.
    /// You can override it to initialize your own references.
    /// </summary>
    public virtual void SetReferences(TutorialManager m, TutorialContextBase c)
    {
        manager = m;
        context = c;
    }

    public void InvokeBegin()
    {
        Begin();
    }

    public void InvokeEnd()
    {
        End();
    }
}
