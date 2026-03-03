using UnityEditor;
using UnityEngine;

namespace HandPosing.Editor.PropertyWrappers
{
    /// <summary>
    /// <see cref="PropertyWrapper{T}"/> override for <see cref="UnityEngine.Quaternion"/> type.
    /// </summary>
    public class QuaternionPropertyWrapper : PropertyWrapper<Quaternion>
    {
        public QuaternionPropertyWrapper(Object target, string propertyName) : base(target, propertyName)
        {
        }

        public QuaternionPropertyWrapper(SerializedObject serializedObject, string propertyName) : base(
            serializedObject, propertyName)
        {
        }

        public QuaternionPropertyWrapper(SerializedProperty serializedProperty) : base(serializedProperty)
        {
        }

        /// <inheritdoc />
        public override Quaternion Value
        {
            get => SerializedProperty.quaternionValue;
            set => SerializedProperty.quaternionValue = value;
        }
    }
}