using System;
using System.Linq;
using UnityEngine;

namespace Saritasa.VRToolset.Highlight
{
    /// <summary>
    /// Implementation of the mesh highlight for objects.
    /// </summary>
    public class Highlight : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Does this object needs to include children.")]
        private bool includeChildren;

        [SerializeField]
        [Tooltip("Highlight color.")]
        private Color highlightColor = Color.white;

        private static Material highlightMaterial;
        private static MaterialPropertyBlock propertyBlock;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private Renderer[] renderers;
        private int[] materialIndices;

        private static Material HighlightMaterial
        {
            get
            {
                if (highlightMaterial == null)
                {
                    highlightMaterial = new Material(Shader.Find("VR-Toolset/HighlightShader"))
                    {
                        enableInstancing = true
                    };
                }

                return highlightMaterial;
            }
        }

        private static MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if (propertyBlock == null)
                {
                    propertyBlock = new MaterialPropertyBlock();
                }

                return propertyBlock;
            }
        }

        /// <summary>
        /// Current highlight color.
        /// </summary>
        public Color Color
        {
            get => highlightColor;
            set
            {
                highlightColor = value;
                UpdateColor();
            }
        }

        private void OnEnable()
        {
            InitRenderers();
            UpdateColor();
        }

        private void UpdateColor()
        {
            PropertyBlock.SetColor(ColorID, highlightColor);
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                renderer.SetPropertyBlock(PropertyBlock, materialIndices[i]);
            }
        }

        private int AppendMaterial(ref Material[] materials, Material material)
        {
            materials = materials.Append(material).ToArray();
            return materials.Length - 1;
        }

        private void InitRenderers()
        {
            if (includeChildren)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
            else
            {
                renderers = new[]
                {
                    GetComponent<Renderer>()
                };
            }

            materialIndices = new int[renderers.Length];
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                var materials = renderer.sharedMaterials;
                int index = Array.IndexOf(materials, HighlightMaterial);

                if (index == -1)
                {
                    index = AppendMaterial(ref materials, HighlightMaterial);
                    renderer.sharedMaterials = materials;
                }

                materialIndices[i] = index;
            }
        }
    }
}