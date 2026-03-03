using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// This is a copy of class from UnityEngine.UI assembly.
    /// It is defined as internal there so there is no access from
    /// other assemblies.
    /// </summary>
    public static class SetPropertyUtility
    {
        /// <summary>
        /// Update value of property of type Color.
        /// </summary>
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        /// <summary>
        /// Update value of struct property.
        /// </summary>
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        /// <summary>
        /// Update value of class property.
        /// </summary>
        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}