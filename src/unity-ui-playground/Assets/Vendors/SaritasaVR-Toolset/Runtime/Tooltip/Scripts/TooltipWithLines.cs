using UnityEngine;
using UnityEngine.Serialization;

namespace Saritasa.VRToolset.Tooltips
{
    /// <summary>
    /// Tooltips alignment positions.
    /// </summary>
    public enum TooltipAlignment
    {
        TopLeft = 1 << 0,
        TopCenter = 1 << 1,
        TopRight = 1 << 2,
        MiddleLeft = 1 << 3,
        MiddleCenter = 1 << 4,
        MiddleRight = 1 << 5,
        BottomLeft = 1 << 6,
        BottomCenter = 1 << 7,
        BottomRight = 1 << 8,
        DoNotAlign = 0
    }

    /// <summary>
    /// Base tooltip class with lines, rotations and positions handling.
    /// </summary>
    [SelectionBase]
    public abstract class TooltipWithLines : TooltipBase
    {
        [SerializeField]
        private TooltipAlignment tooltipAlignment;

        /// <summary>
        /// Alignment position of the tooltip.
        /// </summary>
        public TooltipAlignment TooltipAlignment
        {
            get => tooltipAlignment;
            protected set
            {
                tooltipAlignment = value;
                UpdateTooltipAlignment();
            }
        }

        [SerializeField]
        private Color tooltipColor;

        /// <summary>
        /// Color of the tooltip's background.
        /// </summary>
        public Color TooltipColor
        {
            get => tooltipColor;
            set
            {
                tooltipColor = value;
                SetBackgroundColor(tooltipColor);
            }
        }

        [Tooltip("A transform of another object in the scene that a line will be drawn from the tooltip to. If no transform is provided the line will be disabled.")]
        [SerializeField]
        private Transform drawLineTo;

        /// <summary>
        /// A transform of another object in the scene that a line will be drawn from the tooltip to.
        /// If no transform is provided the line will be disabled.
        /// Changing this property updates line state.
        /// </summary>
        public Transform DrawLineTo
        {
            get => drawLineTo;
            set
            {
                drawLineTo = value;
                UpdateLine();
            }
        }

        private float lineWidth = 0.001f;

        /// <summary>
        /// The width of the line drawn between the tooltip and the destination transform.
        /// Changing the property will update the line.
        /// </summary>
        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                UpdateLine();
            }
        }

        [Tooltip("If true, line start and end points will be updated at runtime.\n Use it if the tooltip or DrawLineTo transform changes it's position at runtime.")]
        [SerializeField]
        private bool linePointsRuntimeUpdate;

        /// <summary>
        /// If true, line start and end points will be updated at runtime.
        /// Use it if the tooltip or DrawLineTo transform changes it's position at runtime.
        /// </summary>
        public bool UpdateLineAtRuntime
        {
            get => linePointsRuntimeUpdate;
            set => linePointsRuntimeUpdate = value;
        }

        #region Tooltip components refs

        /// <summary>
        /// Container transform reference.
        /// Intended to be a parent for tooltip elements e.g. text, background.
        /// Used to set local offsets e.g. tooltip alignment offset.
        /// </summary>
        public Transform Container { get => container; protected set => container = value; }
        
        /// <summary>
        /// ContainerRoot transform reference.
        /// Intended to be a parent for tooltip container.
        /// Used as pivot point to set rotation or scale of the tooltip.
        /// </summary>
        public Transform ContainerRoot { get => containerRoot; protected set => containerRoot = value; }
        
        /// <summary>
        /// Background transform reference.
        /// </summary>
        public Transform Background { get => background; protected set => background = value; }
        
        /// <summary>
        /// Transform reference that intended to be a parent of the tooltip line.
        /// Used for line positioning and scaling.
        /// </summary>
        public Transform LineParent { get => lineParent; protected set => lineParent = value; }
        
        /// <summary>
        /// Transform reference that intended to be a parent of the tooltip end point.
        /// Used for end point positioning and scaling.
        /// </summary>
        public Transform EndPointParent { get => endPointParent; protected set => endPointParent = value; }
        
        /// <summary>
        /// Transform reference that intended to be a parent of the tooltip start point.
        /// Used for start point positioning and scaling.
        /// </summary>
        public Transform StartPointParent { get => startPointParent; protected set => startPointParent = value; }
        
        /// <summary>
        /// Line mesh renderer reference.
        /// Used to set visuals for line.
        /// </summary>
        public MeshRenderer LineRenderer { get => lineRenderer; protected set => lineRenderer = value; }
        
        /// <summary>
        /// End point mesh renderer reference.
        /// Used to set visuals for end point.
        /// </summary>
        public MeshRenderer EndPointRenderer { get => endPointRenderer; protected set => endPointRenderer = value; }
        
        /// <summary>
        /// Start point mesh renderer reference.
        /// Used to set visuals for start point.
        /// </summary>
        public MeshRenderer StartPointRenderer { get => startPointRenderer; protected set => startPointRenderer = value; }
        
        /// <summary>
        /// Background mesh filter reference.
        /// </summary>
        public MeshFilter BackgroundMeshFilter { get => backgroundMeshFilter; protected set => backgroundMeshFilter = value; }

        [SerializeField]
        [Tooltip("Container transform reference.")]
        private Transform container;

        [SerializeField]
        [Tooltip("Container root transform reference.")]
        private Transform containerRoot;

        [SerializeField]
        [Tooltip("Background transform reference.")]
        private Transform background;

        [SerializeField]
        [Tooltip("Line parent transform reference.")]
        private Transform lineParent;

        [SerializeField]
        [Tooltip("First point transform reference.")]
        private Transform endPointParent;

        [SerializeField]
        [Tooltip("Second point transform reference.")]
        private Transform startPointParent;

        [SerializeField]
        [Tooltip("Line mesh renderer.")]
        private MeshRenderer lineRenderer;

        [SerializeField]
        [Tooltip("Tooltip_Point_1 mesh renderer.")]
        private MeshRenderer endPointRenderer;

        [SerializeField]
        [Tooltip("Tooltip_Point_2_Point mesh renderer.")]
        private MeshRenderer startPointRenderer;

        [SerializeField]
        [Tooltip("Background mesh filter.")]
        private MeshFilter backgroundMeshFilter;

        #endregion

        /// <summary>
        /// Default scale of the tooltip's background.
        /// </summary>
        public readonly Vector2 DefaultScale = new(0.1f, 0.045f);

        private Vector2 additionalCornerOffset;

        /// <summary>
        /// Offset of the position when tooltip is aligned in the corner.
        /// </summary>
        public Vector2 AdditionalCornerOffset
        { 
            get => additionalCornerOffset;
            set => additionalCornerOffset = value;
        }

        private Vector2 additionalMidpointOffset;

        /// <summary>
        /// Offset of the position when tooltip is aligned in the middle point.
        /// </summary>
        public Vector2 AdditionalMidpointOffset
        {
            get => additionalMidpointOffset;
            set => additionalMidpointOffset = value;
        }

        private MaterialPropertyBlock backGroundPropBlock;
        
        private readonly int colorPropertyId = Shader.PropertyToID("_Color");

        private void SetBackgroundColor(Color color)
        {
            BackGroundPropBlock.SetColor(colorPropertyId, color);
            BackgroundMeshRenderer.SetPropertyBlock(BackGroundPropBlock);
        }

        /// <summary>
        /// Property block for material animation.
        /// </summary>
        public MaterialPropertyBlock BackGroundPropBlock => backGroundPropBlock ??= new MaterialPropertyBlock();

        protected virtual void Awake()
        {
            UpdateTooltipVisuals();
        }
 
        protected virtual void Update()
        {
            if (UpdateLineAtRuntime)
            {
                UpdateLine();
            }
        }

        /// <summary>
        /// Fully updates all components of the tooltip
        /// according to it's properties.
        /// </summary>
        public virtual void UpdateTooltipVisuals()
        {
            UpdateBackgroundVisuals();
            UpdateLine();
            UpdateTooltipAlignment();
            UpdateText();
        }

        private void UpdateBackgroundVisuals()
        {
            SetBackgroundColor(TooltipColor);
        }

        /// <summary>
        /// Update line start and end points.
        /// </summary>
        private void UpdateLine()
        {
            if (drawLineTo == null)
            {
                EndPointParent.gameObject.SetActive(false);
                StartPointParent.gameObject.SetActive(false);
                LineParent.gameObject.SetActive(false);
                
                return;
            }

            EndPointParent.gameObject.SetActive(true);
            StartPointParent.gameObject.SetActive(true);
            LineParent.gameObject.SetActive(true);

            var drawFormPos = transform.position;
            EndPointParent.transform.position = drawFormPos;
                
            var drawToPos = drawLineTo.position;
            StartPointParent.transform.position = drawToPos;

            var direction = drawFormPos - drawToPos;
            var length = direction.magnitude;
            var lineParentScale = LineParent.parent.lossyScale.x;
            var newHorizontalScale = LineWidth / lineParentScale;

            if (!float.IsInfinity(newHorizontalScale))
            {
                LineParent.localScale = new Vector3(newHorizontalScale, newHorizontalScale, length / lineParentScale);
            }

            LineParent.position = (transform.position + drawLineTo.position) / 2f;
            LineParent.rotation = Quaternion.LookRotation(direction);
        }

        /// <summary>
        /// Returns true if tooltip aligned on left or right.
        /// </summary>
        public bool IsRightOrLeftAlignment
        {
            get
            {
                var cornerSet = TooltipAlignment.BottomRight |
                    TooltipAlignment.BottomLeft |
                    TooltipAlignment.TopRight |
                    TooltipAlignment.TopLeft | 
                    TooltipAlignment.MiddleLeft |
                    TooltipAlignment.MiddleRight;
                return (cornerSet & TooltipAlignment) != 0;
            }
        }

        /// <summary>
        /// Returns true if tooltip aligned in the middle point.
        /// </summary>
        public bool IsMidpointAlignment
        {
            get
            {
                var cornerSet = TooltipAlignment.BottomCenter |
                    TooltipAlignment.TopCenter |
                    TooltipAlignment.MiddleRight |
                    TooltipAlignment.MiddleLeft;
                return (cornerSet & TooltipAlignment) != 0;
            }
        }

        /// <summary>
        /// Returns true if tooltip aligned in the corner.
        /// </summary>
        public bool IsCornerAlignment
        {
            get
            {
                var cornerSet = TooltipAlignment.BottomRight |
                    TooltipAlignment.BottomLeft |
                    TooltipAlignment.TopRight |
                    TooltipAlignment.TopLeft;
                return (cornerSet & TooltipAlignment) != 0;
            }
        }

        private void UpdateTooltipAlignment()
        {
            var localScale = BackgroundMeshRenderer.transform.localScale;
            var staticOffset = new Vector2(
                0.5f * localScale.x,
                0.5f * localScale.y);

            Vector2 additionalOffset = Vector2.zero;

            if (IsCornerAlignment)
            {
                additionalOffset = AdditionalCornerOffset;
            }
            else if (IsMidpointAlignment)
            {
                additionalOffset = AdditionalMidpointOffset;
            }

            staticOffset += additionalOffset;

            Container.localPosition = TooltipAlignment switch
            {
                TooltipAlignment.BottomCenter => new Vector3(additionalOffset.x, staticOffset.y, 0),
                TooltipAlignment.BottomRight => new Vector3(staticOffset.x, staticOffset.y, 0),
                TooltipAlignment.BottomLeft => new Vector3(-staticOffset.x, staticOffset.y, 0),
                TooltipAlignment.MiddleCenter => new Vector3(0, 0, 0),
                TooltipAlignment.MiddleRight => new Vector3(staticOffset.x, additionalOffset.y, 0),
                TooltipAlignment.MiddleLeft => new Vector3(-staticOffset.x, additionalOffset.y, 0),
                TooltipAlignment.TopCenter => new Vector3(additionalOffset.x, -staticOffset.y, 0),
                TooltipAlignment.TopRight => new Vector3(staticOffset.x, -staticOffset.y, 0),
                TooltipAlignment.TopLeft => new Vector3(-staticOffset.x, -staticOffset.y, 0),
                _ => Container.localPosition
            };
        }

        /// <inheritdoc />
        public override void Show(float duration = 1f)
        {
            SetEnabledMeshRenderers(true);
        }

        /// <inheritdoc />
        public override void Hide(float duration = 1f)
        {
            SetEnabledMeshRenderers(false);
        }

        /// <inheritdoc />
        protected override void SetEnabledMeshRenderers(bool isEnable)
        {
            BackgroundMeshRenderer.enabled = isEnable;
            Txt_Front_MR.enabled = isEnable;
            Txt_Back_MR.enabled = isEnable;
            LineRenderer.enabled = isEnable;
            EndPointRenderer.enabled = isEnable;
            StartPointRenderer.enabled = isEnable;
        }
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            UpdateTooltipVisuals();
        }

        [ContextMenu("Show")]
        private void FadeTest()
        {
            Show(1f);
        }

        [ContextMenu("Hide")]
        private void UnFadeTest()
        {
            Hide(1f);
        }
#endif
    }
}