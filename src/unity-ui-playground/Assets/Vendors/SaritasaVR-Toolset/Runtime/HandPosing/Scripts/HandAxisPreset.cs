using System;
using UnityEngine;

namespace HandPosing
{
    /// <summary>
    /// Axis preset to use in hand target.
    /// </summary>
    [CreateAssetMenu(fileName = "HandAxisPreset", menuName = "Hand Axis Preset", order = 0)]
    public class HandAxisPreset : ScriptableObject
    {
        /// <summary>
        /// Axis.
        /// </summary>
        public enum Axis
        {
            XPlus,
            XMinus,
            YPlus,
            YMinus,
            ZPlus,
            ZMinus
        }

        [SerializeField]
        private Axis right;

        [SerializeField]
        private Axis forward;

        [SerializeField]
        private Axis up;

        /// <summary>
        /// Right axis enum.
        /// </summary>
        public Axis Right => right;

        /// <summary>
        /// Up axis enum.
        /// </summary>
        public Axis Up => up;

        /// <summary>
        /// Forward axis enum.
        /// </summary>
        public Axis Forward => forward;

        /// <summary>
        /// Right axis index.
        /// </summary>
        public int RightIndex => AxisToVectorIndex(right);

        /// <summary>
        /// Up axis index.
        /// </summary>
        public int UpIndex => AxisToVectorIndex(up);

        /// <summary>
        /// Forward axis index.
        /// </summary>
        public int ForwardIndex => AxisToVectorIndex(forward);

        /// <summary>
        /// Right axis vector.
        /// </summary>
        public Vector3 RightVector => AxisToVector(right);

        /// <summary>
        /// Up axis vector.
        /// </summary>
        public Vector3 UpVector => AxisToVector(up);

        /// <summary>
        /// Forward axis vector.
        /// </summary>
        public Vector3 ForwardVector => AxisToVector(forward);

        /// <summary>
        /// Converts axis enum to <see cref="UnityEngine.Vector3"/>.
        /// </summary>
        /// <param name="axis">Axis enum.</param>
        /// <returns>Vector.</returns>
        public static Vector3 AxisToVector(Axis axis)
        {
            return axis switch
            {
                Axis.XPlus => Vector3.right,
                Axis.XMinus => Vector3.left,
                Axis.YPlus => Vector3.up,
                Axis.YMinus => Vector3.down,
                Axis.ZPlus => Vector3.forward,
                Axis.ZMinus => Vector3.back,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }

        /// <summary>
        /// Converts axis enum to <see cref="UnityEngine.Vector3"/> axis index.
        /// </summary>
        /// <param name="axis">Axis enum.</param>
        /// <returns>Index.</returns>
        public static int AxisToVectorIndex(Axis axis)
        {
            return axis switch
            {
                Axis.XPlus => 0,
                Axis.XMinus => 0,
                Axis.YPlus => 1,
                Axis.YMinus => 1,
                Axis.ZPlus => 2,
                Axis.ZMinus => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }
    }
}