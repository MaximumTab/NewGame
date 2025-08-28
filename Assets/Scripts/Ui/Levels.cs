using UnityEngine;
using UnityEngine.SceneManagement;

public static class Levels
{
    public static void MarkLevelComplete(string sceneName)
    {
        PlayerPrefs.SetInt(sceneName + "_Completed", 1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelComplete(string sceneName)
    {
        return PlayerPrefs.GetInt(sceneName + "_Completed", 0) == 1;
    }

    public static bool HasCompletedPrerequisites(string[] prerequisites)
    {
        foreach (string prereq in prerequisites)
        {
            if (!IsLevelComplete(prereq))
                return false;
        }
        return true;
    }

    public static void DebugLevelStatus()
    {
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        Debug.Log("=== LEVEL STATUS ===");
        for (int i = 0; i < totalScenes; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            bool complete = IsLevelComplete(sceneName);
            Debug.Log(sceneName + " : " + (complete ? "COMPLETED" : "INCOMPLETE"));
        }
        Debug.Log("====================");
    }
}
