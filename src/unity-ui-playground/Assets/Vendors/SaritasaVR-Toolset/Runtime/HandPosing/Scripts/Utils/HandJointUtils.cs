using System.Collections.Generic;
using HandPosing.Enumerations;

namespace HandPosing.Utils
{
    /// <summary>
    /// Class that contains utilities for hand joints.
    /// </summary>
    public static class HandJointUtils
    {
        /// <summary>
        /// Method that returns parent joint.
        /// </summary>
        /// <param name="joint">Child joint.</param>
        /// <returns>Parent joint. <see cref="HandJointType.Invalid"/> if there are no parent.</returns>
        public static HandJointType GetParentJoint(HandJointType joint)
        {
            return joint switch
            {
                HandJointType.ThumbTrapezium => HandJointType.Wrist,
                HandJointType.ThumbMeta => HandJointType.ThumbTrapezium,
                HandJointType.ThumbProximal => HandJointType.ThumbMeta,
                HandJointType.ThumbDistal => HandJointType.ThumbProximal,
                HandJointType.IndexProximal => HandJointType.Wrist,
                HandJointType.IndexIntermediate => HandJointType.IndexProximal,
                HandJointType.IndexDistal => HandJointType.IndexIntermediate,
                HandJointType.MiddleProximal => HandJointType.Wrist,
                HandJointType.MiddleIntermediate => HandJointType.MiddleProximal,
                HandJointType.MiddleDistal => HandJointType.MiddleIntermediate,
                HandJointType.RingProximal => HandJointType.Wrist,
                HandJointType.RingIntermediate => HandJointType.RingProximal,
                HandJointType.RingDistal => HandJointType.RingIntermediate,
                HandJointType.PinkyMeta => HandJointType.Wrist,
                HandJointType.PinkyProximal => HandJointType.PinkyMeta,
                HandJointType.PinkyIntermediate => HandJointType.PinkyProximal,
                HandJointType.PinkyDistal => HandJointType.PinkyIntermediate,
                _ => HandJointType.Invalid
            };
        }

        /// <summary>
        /// Method that tries to return parent joint.
        /// </summary>
        /// <param name="joint">Child joint.</param>
        /// <param name="parentJoint">Parent joint.</param>
        /// <returns>True if parent exists, otherwise false.</returns>
        public static bool TryGetParentJoint(HandJointType joint, out HandJointType parentJoint)
        {
            parentJoint = GetParentJoint(joint);
            return parentJoint != HandJointType.Invalid;
        }

        /// <summary>
        /// Collection of joints with free X axis.
        /// </summary>
        public static readonly HashSet<HandJointType> JointsWithFreeRotation = new HashSet<HandJointType>
        {
            HandJointType.ThumbTrapezium,
            HandJointType.IndexProximal,
            HandJointType.MiddleProximal,
            HandJointType.RingProximal,
            HandJointType.PinkyMeta
        };
    }
}