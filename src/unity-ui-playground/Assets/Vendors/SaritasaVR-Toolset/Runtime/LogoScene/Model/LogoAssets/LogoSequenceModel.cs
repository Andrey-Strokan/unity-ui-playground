using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Model for storing a sequence of logos.
    /// </summary>
    [CreateAssetMenu(fileName = "LogoSequenceModel", menuName = "Models/LogoSequenceModel", order = 1)]
    public class LogoSequenceModel : ScriptableObject
    {
        /// <summary>
        /// Structure for storing data about one element of a logo sequence.
        /// </summary>
        [Serializable]
        public struct LogoSequenceElement
        {
            /// <summary>
            /// Logo texture.
            /// </summary>
            [field: SerializeField]
            public Texture LogoTexture { get; private set; }

            /// <summary>
            /// Logo video.
            /// </summary>
            [field: SerializeField]
            public VideoClip LogoVideo { get; private set; }

            /// <summary>
            /// Display duration in seconds. Ignored if video is used.
            /// </summary>
            [field: SerializeField]
            public float DisplayDuration { get; private set; }
        }

        [SerializeField]
        private LogoSequenceElement[] logoSequence = Array.Empty<LogoSequenceElement>();

        /// <summary>
        /// List of logo sequences.
        /// </summary>
        public IReadOnlyList<LogoSequenceElement> LogoSequence => logoSequence;
    }
}
