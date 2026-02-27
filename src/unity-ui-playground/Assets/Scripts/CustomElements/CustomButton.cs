using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomButton : Button
{
    private const string UnityTextElementClassName = "unity-text-element";
    private const string UnityButtonClassName = "unity-button";

    private const string CustomButtonClassName = "button-custom";

    public CustomButton()
    {
        RemoveFromClassList(UnityTextElementClassName);
        RemoveFromClassList(UnityButtonClassName);
        AddToClassList(CustomButtonClassName);
    }
}
