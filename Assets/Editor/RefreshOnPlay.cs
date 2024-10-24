using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;


[InitializeOnLoadAttribute]
public static class PlayRefreshEditor
{

    static PlayRefreshEditor()
    {
        EditorApplication.playModeStateChanged += PlayRefresh;
    }

    private static void PlayRefresh(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }
}

