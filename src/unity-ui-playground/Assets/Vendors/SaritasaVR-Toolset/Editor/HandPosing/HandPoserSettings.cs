using HandPosing.HandPoseTargets;
using UnityEditor;
using UnityEngine;

namespace HandPosing.Editor
{
    /// <summary>
    /// Global settings for hand poser.
    /// </summary>
    [CreateAssetMenu(fileName = FileName, menuName = "Hand Poser Settings", order = 0)]
    public class HandPoserSettings : ScriptableObject
    {
        private const string FileName = "HandPoserSettings";
        private const string DefaultPath = "Assets/Resources/";

        private static HandPoserSettings instance;

        /// <summary>
        /// Settings instance.
        /// </summary>
        public static HandPoserSettings Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Resources.Load<HandPoserSettings>(FileName);

                    if (instance == null)
                    {
                        HandPoserSettings newInstance = CreateInstance<HandPoserSettings>();
                        newInstance.handPoseTarget = Resources.Load<HandPoseTarget>("OculusHand_R");

                        if (!AssetDatabase.IsValidFolder(DefaultPath))
                        {
                            AssetDatabase.CreateFolder("Assets", "Resources");
                        }

                        AssetDatabase.CreateAsset(newInstance, $"{DefaultPath}/{FileName}.asset");
                        instance = newInstance;
                    }
                }

                return instance;
            }
        }

        [SerializeField]
        [Tooltip("Hand target to use in project.")]
        private HandPoseTarget handPoseTarget;

        /// <summary>
        /// Hand target used in project.
        /// </summary>
        public HandPoseTarget HandPoseTarget => handPoseTarget;
    }
}