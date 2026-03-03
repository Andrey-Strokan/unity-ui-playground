using System.Collections.Generic;
using UnityEngine;

namespace Saritasa.Controllers
{
    public abstract class ControllerPreset
    {
        /// <summary>
        /// List of possible names.
        /// </summary>
        public abstract List<string> HeadSetNameVariants { get; set; }

        /// <summary>
        /// Prefab for left controller.
        /// </summary>
        protected abstract string LeftControllerResourcePath { get; }

        /// <summary>
        /// Prefab for right controller.
        /// </summary>
        protected abstract string RightControllerResourcePath { get; }

        /// <summary>
        /// Get left controller prefab transform.
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetLeftControllerPrefabTransform()
        {
            if (string.IsNullOrEmpty(LeftControllerResourcePath)) return null;

            var controller = Resources.Load<Transform>(LeftControllerResourcePath);
            return controller;
        }

        /// <summary>
        /// Get right controller prefab transform.
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetRightControllerPrefabTransform()
        {
            if (string.IsNullOrEmpty(LeftControllerResourcePath)) return null;

            var controller = Resources.Load<Transform>(RightControllerResourcePath);
            return controller;
        }
    }
}