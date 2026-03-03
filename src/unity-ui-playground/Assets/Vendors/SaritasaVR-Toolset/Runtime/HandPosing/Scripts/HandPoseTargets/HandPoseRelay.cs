using UnityEngine;

namespace HandPosing.HandPoseTargets
{
    /// <summary>
    /// Class used to route to <see cref="HandPoseTarget"/>.
    /// </summary>
    public class HandPoseRelay : MonoBehaviour
    {
        [SerializeField]
        private HandPoseTarget target;

        /// <summary>
        /// <see cref="HandPoseTarget"/> to route to.
        /// </summary>
        public HandPoseTarget Target => target;
    }
}