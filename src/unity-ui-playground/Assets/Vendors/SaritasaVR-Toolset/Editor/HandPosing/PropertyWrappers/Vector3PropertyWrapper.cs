using UnityEditor;
using UnityEngine;

namespace HandPosing.Editor.PropertyWrappers
{
    /// <summary>
    /// <see cref="PropertyWrapper{T}"/> override for <see cref="UnityEngine.Vector3"/> type.
    /// </summary>
    public class Vector3PropertyWrapper : PropertyWrapper<Vector3>
    {
        public Vector3PropertyWrapper(Object target, string propertyName) : base(target, propertyName)
        {
        }

        public Vector3PropertyWrapper(SerializedObject serializedObject, string propertyName) : base(serializedObject,
            propertyName)
        {
        }

        public Vector3PropertyWrapper(SerializedProperty serializedProperty) : base(serializedProperty)
        {
        }

        /// <inheritdoc />
        public override Vector3 Value
        {
            get => SerializedProperty.vector3Value;
            set => SerializedProperty.vector3Value = value;
        }
    }
}