using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HandPosing.Editor.PropertyWrappers
{
    /// <summary>
    /// Property wrapper used for easier access to <see cref="UnityEditor.SerializedProperty"/>.
    /// </summary>
    /// <typeparam name="T">Represented property type.</typeparam>
    public abstract class PropertyWrapper<T> : IDisposable
    {
        private readonly bool shouldDisposeObject;
        private readonly bool shouldDisposeProperty;

        /// <summary>
        /// <see cref="UnityEditor.SerializedObject"/> this object connected to.
        /// </summary>
        public readonly SerializedObject SerializedObject;

        /// <summary>
        /// <see cref="UnityEditor.SerializedProperty"/> this object connected to.
        /// </summary>
        public readonly SerializedProperty SerializedProperty;

        /// <summary>
        /// Property name.
        /// </summary>
        public readonly string PropertyName;

        protected PropertyWrapper(Object target, string propertyName)
        {
            shouldDisposeObject = true;
            shouldDisposeProperty = true;

            SerializedObject = new SerializedObject(target);
            SerializedProperty = SerializedObject.FindProperty(propertyName);

            PropertyName = SerializedProperty.displayName;
        }

        protected PropertyWrapper(SerializedObject serializedObject, string propertyName)
        {
            shouldDisposeObject = false;
            shouldDisposeProperty = true;

            SerializedObject = serializedObject;
            SerializedProperty = SerializedObject.FindProperty(propertyName);

            PropertyName = SerializedProperty.displayName;
        }

        protected PropertyWrapper(SerializedProperty serializedProperty)
        {
            shouldDisposeObject = false;
            shouldDisposeProperty = false;

            SerializedObject = serializedProperty.serializedObject;
            SerializedProperty = serializedProperty;

            PropertyName = SerializedProperty.displayName;
        }

        private T lastValue;

        /// <summary>
        /// Current property value.
        /// </summary>
        public abstract T Value { get; set; }

        /// <summary>
        /// Boolean that indicates whether value has changed or not.
        /// </summary>
        public bool HasChanged
        {
            get
            {
                if (lastValue.Equals(Value))
                {
                    return false;
                }

                lastValue = Value;
                return true;
            }
        }

        /// <summary>
        /// Method used to apply changes to <see cref="SerializedObject"/>.
        /// </summary>
        /// <param name="allowUndo">Whether to save undo or not.</param>
        public void Apply(bool allowUndo = true)
        {
            if (allowUndo)
            {
                SerializedObject.ApplyModifiedProperties();
            }
            else
            {
                SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (shouldDisposeObject)
            {
                SerializedObject.Dispose();
            }

            if (shouldDisposeProperty)
            {
                SerializedProperty.Dispose();
            }
        }
    }
}