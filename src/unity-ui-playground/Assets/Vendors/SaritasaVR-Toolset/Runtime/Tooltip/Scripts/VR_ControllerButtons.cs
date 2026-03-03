/// <summary>
/// All available controller buttons.
/// </summary>
public enum VR_ControllerButtons
{
    /// <summary>
    /// Touchpad or thumbstick of controller
    /// </summary>
    Touchpad,

    /// <summary>
    /// Trigger of controller
    /// </summary>
    Trigger,

    /// <summary>
    /// Menu button of controller
    /// </summary>
    ButtonEnter,

    /// <summary>
    /// Button X or A on Oculus controllers, Vive controllers do not have this button
    /// </summary>
    ButtonOne,

    /// <summary>
    /// Button Y or B on Oculus controllers, button on Vive controllers
    /// </summary>
    ButtonTwo,

    /// <summary>
    /// Grip button of controller
    /// </summary>
    Grip
}

/// <summary>
/// All supported types of actions with controllers buttons.
/// </summary>
public enum VR_ControllerButtonActions
{
    /// <summary>
    /// Button is in a pressed state at the current moment.
    /// </summary>
    Press,

    /// <summary>
    /// Button was just pressed down at this frame.
    /// </summary>
    PressDown,

    /// <summary>
    /// Button was just released at this frame.
    /// </summary>
    PressUp,

    /// <summary>
    /// Button is in a pressed state at the current moment.
    /// </summary>
    Touch,

    /// <summary>
    /// Button was just touched down at this frame.
    /// </summary>
    TouchDown,

    /// <summary>
    /// Button was just untouched at this frame.
    /// </summary>
    TouchUp
}
