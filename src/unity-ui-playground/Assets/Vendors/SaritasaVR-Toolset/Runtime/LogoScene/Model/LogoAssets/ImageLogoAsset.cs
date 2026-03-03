using UnityEngine;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Logo asset for image logo.
    /// </summary>
    [CreateAssetMenu(fileName = "ImageLogoAsset", menuName = "Data/ImageLogoAsset", order = 1)]
    public class ImageLogoAsset : LogoAsset
    {
        /// <summary>
        /// Logo in image format.
        /// </summary>
        [field: SerializeField]
        public Texture LogoImage { get; private set; }
    }
}