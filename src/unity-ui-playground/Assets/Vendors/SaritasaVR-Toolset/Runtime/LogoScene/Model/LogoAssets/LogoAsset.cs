using UnityEngine;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Base class for logo asset.
    /// </summary>
    public abstract class LogoAsset : ScriptableObject
    {
        /// <summary>
        /// Duration of the logo showing.
        /// </summary>
        [field: SerializeField]
        public float LogoDuration {get; private set;}
    }
}