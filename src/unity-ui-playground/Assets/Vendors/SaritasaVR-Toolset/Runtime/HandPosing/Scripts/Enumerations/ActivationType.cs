using System;

namespace HandPosing.Enumerations
{
    /// <summary>
    /// Pose activation type.
    /// </summary>
    [Flags]
    public enum ActivationType
    {
        /// <summary>
        /// Activates on object selection.
        /// </summary>
        Select = 0b01,

        /// <summary>
        /// Activates on object hover.
        /// </summary>
        Hover = 0b10
    }
}