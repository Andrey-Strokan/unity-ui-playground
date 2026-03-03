using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that contains the list of scenes
/// available in the project. It is used to avoid 
/// using strings for scene names when calling
/// LoadScene() commands. If you want to add new a
/// new scene in the project, add it to the SceneEnum
/// and  add this scene to the list of the ScriptableObject
/// asset (it must be stored in the Resources folder.
/// </summary>
[CreateAssetMenu(fileName = "SceneList", menuName = "SceneList", order = 1)]
public class SceneList : ScriptableObject
{
    [SerializeField]
    private List<SceneReference> sceneList = new List<SceneReference>();

    /// <summary>
    /// Property to access to actual scene list. 
    /// </summary>
    public List<SceneReference> CurrentSceneList
    {
        get 
        {
            return sceneList;
        }
    }
}