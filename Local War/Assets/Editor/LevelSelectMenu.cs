using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Linq;
using System.IO;
using System;

public class LevelSelectMenu : EditorWindow
{

    static List<SceneAsset> m_SceneAssets = new List<SceneAsset>();

    [MenuItem("Window/Main Menu Level Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelSelectMenu));

        m_SceneAssets.Clear();
        foreach (var scene in LevelInfoCollection.LoadLevels(LevelInfoCollection.DefaultEditorFileCache))
        {
            m_SceneAssets.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Scenes to include in build:", EditorStyles.boldLabel);
        for(int i = 0; i < m_SceneAssets.Count; i++)
        {
            GUILayout.BeginHorizontal();
            m_SceneAssets[i] = (SceneAsset)EditorGUILayout.ObjectField(m_SceneAssets[i], typeof(SceneAsset), false);
            if (GUILayout.Button("Delete"))
            {
                m_SceneAssets.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add"))
        {
            m_SceneAssets.Add(null);
        }
        GUILayout.Space(8);
        if(GUILayout.Button("Apply"))
        {
            var levels = (from scene in m_SceneAssets
                          select (LevelInfo)scene).ToArray();


            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var level in levels)
            {
                var scene = buildScenes.FirstOrDefault(item => item.path == level.path);
                if (scene == null)
                {
                    buildScenes.Add(new EditorBuildSettingsScene(level.path, true));
                }
                else if(scene.enabled == false)
                {
                    scene.enabled = true;
                }
                
            }
            EditorBuildSettings.scenes = buildScenes.ToArray();


            LevelInfoCollection.SaveLevels(levels, LevelInfoCollection.DefaultEditorFileCache);
            Close();
        }
    }

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathAToBuiltProject)
    {
        var dir = Path.GetDirectoryName(pathAToBuiltProject);
        if(Directory.Exists(dir + "\\Local War_Data"))
        {
            LevelInfoCollection.SaveLevels(LevelInfoCollection.LoadLevels(LevelInfoCollection.DefaultEditorFileCache),
                dir + "\\Local War_Data\\" + LevelInfoCollection.DefaultFileCacheName);
        }
        else if(Directory.Exists(dir + "\\LocalWar_Data"))
        {
            LevelInfoCollection.SaveLevels(LevelInfoCollection.LoadLevels(LevelInfoCollection.DefaultEditorFileCache),
                dir + "\\LocalWar_Data\\" + LevelInfoCollection.DefaultFileCacheName);
        }
        else
        {
            Debug.Log("Data folder does not exist:" + dir + "\\Local War_Data\\");
        }
    }
}
