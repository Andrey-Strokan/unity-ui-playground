using HandPosing.Enumerations;
using UnityEngine;

namespace HandPosing
{
    /// <summary>
    /// Collection containing hand rotations.
    /// </summary>
    public class HandRotations
    {
        private readonly Quaternion[] rotations = new Quaternion[(int)HandJointType.Count];

        /// <summary>
        /// Indexer for joint rotations.
        /// </summary>
        /// <param name="jointType">Joint to get rotation for.</param>
        public Quaternion this[HandJointType jointType]
        {
            get => rotations[(int)jointType];
            set => rotations[(int)jointType] = value;
        }
    }
}