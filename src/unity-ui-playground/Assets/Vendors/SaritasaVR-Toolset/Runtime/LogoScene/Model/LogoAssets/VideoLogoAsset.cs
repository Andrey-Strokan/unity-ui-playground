using UnityEngine;
using UnityEngine.Video;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Logo asset for video logo.
    /// </summary>
    [CreateAssetMenu(fileName = "VideoLogoAsset", menuName = "Data/VideoLogoAsset", order = 1)]
    public class VideoLogoAsset : LogoAsset
    {
        /// <summary>
        /// Logo in video format.
        /// </summary>
        [field: SerializeField]
        public VideoClip LogoVideo { get; private set; }
    }
}