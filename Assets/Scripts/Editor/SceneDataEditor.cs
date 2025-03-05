#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(SceneData))]
public class SceneDataEditor : Editor
{
    private bool showAllScenes = false;

    public override void OnInspectorGUI()
    {
        SceneData data = (SceneData)target;
        string currentScene = EditorSceneManager.GetActiveScene().name;

        showAllScenes = EditorGUILayout.Toggle("Show all scenes", showAllScenes);

        var scenesToShow = showAllScenes ? data.sceneObjectsList : data.sceneObjectsList.Where(s => s.sceneName == currentScene).ToList();

        foreach (var sceneObjects in scenesToShow)
        {
            EditorGUILayout.LabelField($"Scene: {sceneObjects.sceneName}", EditorStyles.boldLabel);

            foreach (var objName in sceneObjects.destroyedObjects)
            {
                EditorGUILayout.LabelField($"- {objName}");
            }

            EditorGUILayout.Space();
        }
    }
}
#endif