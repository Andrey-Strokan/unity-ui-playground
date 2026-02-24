using System.Drawing.Drawing2D;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GradientUXMLElement : VisualElement
{
    private Gradient gradient;

    [Header("Gradient")]
    [UxmlAttribute]
    private Gradient Gradient
    {
        get => gradient;

        set
        {
            if (gradient == value)
                return;

            gradient = value;
            MarkDirtyRepaint();
        }
    }

    private GradientType gradientType = GradientType.Linear;

    [UxmlAttribute]
    private GradientType GradientType
    {
        get => gradientType;

        set
        {
            if (gradientType == value)
                return;

            gradientType = value;
            MarkDirtyRepaint();
        }
    }

    private AddressMode addressMode = AddressMode.Clamp;

    [UxmlAttribute]
    private AddressMode AddressMode
    {
        get => addressMode;

        set
        {
            if (addressMode == value)
                return;

            addressMode = value;
            MarkDirtyRepaint();
        }
    }

    private Vector2 startPosition;

    [UxmlAttribute]
    private Vector2 StartPosition
    {
        get => startPosition;

        set
        {
            if (startPosition == value)
                return;

            startPosition.x = Mathf.Clamp01(value.x);
            startPosition.y = Mathf.Clamp01(value.y);

            MarkDirtyRepaint();
        }
    }

    private Vector2 endPosition;

    [UxmlAttribute]
    private Vector2 EndPosition
    {
        get => endPosition;

        set
        {
            if (endPosition == value)
                return;

            endPosition.x = Mathf.Clamp01(value.x);
            endPosition.y = Mathf.Clamp01(value.y);

            MarkDirtyRepaint();
        }
    }

    private float radius;

    [UxmlAttribute]
    private float Radius
    {
        get => radius;

        set
        {
            if (Mathf.Approximately(radius, value))
                return;

            radius = value;
            MarkDirtyRepaint();
        }
    }

    public GradientUXMLElement()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    ~GradientUXMLElement()
    {
        generateVisualContent -= OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var rect = contentRect;
        if (rect.width < 0.01f || rect.height < 0.01f)
            return;

        var painter = mgc.painter2D;
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 0));
        painter.LineTo(new Vector2(rect.width, 0));
        painter.LineTo(new Vector2(rect.width, rect.height));
        painter.LineTo(new Vector2(0, rect.height));
        painter.ClosePath();

        FillGradient gradient;

        if (GradientType == GradientType.Linear)
        {
            gradient = FillGradient.MakeLinearGradient(
                Gradient,
                StartPosition * rect.size,
                EndPosition * rect.size,
                AddressMode);
        }
        else
        {
            gradient = FillGradient.MakeRadialGradient(
                Gradient,
                StartPosition * rect.size,
                Radius,
                EndPosition * rect.size,
                AddressMode);
        }

        painter.fillGradient = gradient;
        painter.Fill(FillRule.OddEven);
    }
}
