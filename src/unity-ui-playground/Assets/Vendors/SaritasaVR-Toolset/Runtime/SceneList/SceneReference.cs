/// <summary>
/// Class that defines the dependency between the Scene Enum and the real name of the scene file.
/// </summary>
[System.Serializable]
public class SceneReference
{
    /// <summary>
    /// Scene Enum.
    /// </summary>
    public SceneEnum Scene;

    /// <summary>
    /// Scene name. Real name of the scene file.
    /// </summary>
    public string SceneName;
}