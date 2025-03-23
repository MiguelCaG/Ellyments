using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "Game/Scene Data")]
public class SceneData : ScriptableObject
{
    [System.Serializable]
    public class SceneObjects
    {
        public string sceneName;
        public List<string> finalizedObjects = new List<string>();
    }
    public List<SceneObjects> sceneObjectsList = new List<SceneObjects>();

    private const string SaveKey = "SceneData";

    public void MarkObjectFinalized(string sceneName, string objectName)
    {
        SceneObjects sceneObjects = sceneObjectsList.Find(s => s.sceneName == sceneName);
        if (sceneObjects == null)
        {
            sceneObjects = new SceneObjects { sceneName = sceneName };
            sceneObjectsList.Add(sceneObjects);
        }

        if (!sceneObjects.finalizedObjects.Contains(objectName))
        {
            sceneObjects.finalizedObjects.Add(objectName);
            SaveState();
        }
    }

    public bool HasObjectFinalized(string sceneName, string objectName)
    {
        SceneObjects sceneObjects = sceneObjectsList.Find(s => s.sceneName == sceneName);
        return sceneObjects != null && sceneObjects.finalizedObjects.Contains(objectName);
    }

    public void SaveState()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(SaveKey), this);
        }
    }
}
