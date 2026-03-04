using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIToolKitSafeAreaContainer : VisualElement
{
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;

    [UxmlAttribute("exclude-left")]
    private bool excludeLeft;

    [UxmlAttribute("exclude-right")]
    private bool excludeRight;

    [UxmlAttribute("exclude-top")]
    private bool excludeTop;

    [UxmlAttribute("exclude-bottom")]
    private bool excludeBottom;

    public UIToolKitSafeAreaContainer()
    {
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.top = 0;
        style.bottom = 0;
        style.left = 0;
        style.right = 0;

        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        ApplySafeArea();
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (lastSafeArea != Screen.safeArea || lastScreenSize != currentScreenSize)
        {
            lastSafeArea = Screen.safeArea;
            lastScreenSize = currentScreenSize;
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        // As RuntimePanelUtils is not available in UIBuilder,
        // the handling is wrapped in a try/catch to avoid InvalidCastExceptions when working in UIBuilder.
        try
        {
            if (Screen.width == 0 || Screen.height == 0 || panel == null)
                return;

            var safeArea = Screen.safeArea;

            var leftTop = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            var rightBottom = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            style.marginLeft = excludeLeft ? 0.0f : leftTop.x;
            style.marginTop = excludeTop ? 0.0f : leftTop.y;
            style.marginRight = excludeRight ? 0.0f : rightBottom.x;
            style.marginBottom = excludeBottom ? 0.0f : rightBottom.y;
        }
        catch (Exception e)
        {
            switch(e)
            {
                case InvalidCastException:
                    break;
                default:
                    Debug.LogError(e);
                    break;
            }
        }
    }
}
