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
        public List<string> destroyedObjects = new List<string>();
    }
    public List<SceneObjects> sceneObjectsList = new List<SceneObjects>();

    public void MarkObjectDestroyed(string sceneName, string objectName)
    {
        SceneObjects sceneObjects = sceneObjectsList.Find(s => s.sceneName == sceneName);
        if (sceneObjects == null)
        {
            sceneObjects = new SceneObjects { sceneName = sceneName };
            sceneObjectsList.Add(sceneObjects);
        }

        if (!sceneObjects.destroyedObjects.Contains(objectName))
        {
            sceneObjects.destroyedObjects.Add(objectName);
        }
    }

    public bool IsObjectDestroyed(string sceneName, string objectName)
    {
        SceneObjects sceneObjects = sceneObjectsList.Find(s => s.sceneName == sceneName);
        return sceneObjects != null && sceneObjects.destroyedObjects.Contains(objectName);
    }
}
