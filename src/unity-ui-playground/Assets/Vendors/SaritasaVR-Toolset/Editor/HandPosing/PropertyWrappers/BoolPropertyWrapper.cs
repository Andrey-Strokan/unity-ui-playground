using UnityEditor;
using UnityEngine;

namespace HandPosing.Editor.PropertyWrappers
{
    /// <summary>
    /// <see cref="PropertyWrapper{T}"/> override for <see cref="System.Boolean"/> type.
    /// </summary>
    public class BoolPropertyWrapper : PropertyWrapper<bool>
    {
        public BoolPropertyWrapper(Object target, string propertyName) : base(target, propertyName)
        {
        }

        public BoolPropertyWrapper(SerializedObject serializedObject, string propertyName) : base(serializedObject,
            propertyName)
        {
        }

        public BoolPropertyWrapper(SerializedProperty serializedProperty) : base(serializedProperty)
        {
        }

        /// <inheritdoc />
        public override bool Value
        {
            get => SerializedProperty.boolValue;
            set => SerializedProperty.boolValue = value;
        }
    }
}