#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace Trissiklikk.EditorTools
{
    public sealed class LoadSceneGUI : EditorWindow
    {
        /// <summary>
        /// Static mathod to show the window.
        /// </summary>
        [MenuItem("Window/Trissiklikk Editor Tools/Load Scene GUI")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LoadSceneGUI));
        }

        private bool m_isNeedSave;
        private int m_selectedTypeIndex;
        private string[] m_loadSceneTypes;
        private Vector2 m_scrollPosition;

        private LoadSceneGUI()
        {
            m_scrollPosition = Vector2.zero;
            m_isNeedSave = true;
            m_selectedTypeIndex = 0;
            m_loadSceneTypes = new string[]
            {
                "Single",
                "Additive",
                "Additive Without Loading",
            };
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            m_selectedTypeIndex = EditorGUILayout.Popup("Loading Types", m_selectedTypeIndex, m_loadSceneTypes);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            m_isNeedSave = GUILayout.Toggle(m_isNeedSave, "Save current scene before loading");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Edit Scene setting", GUILayout.Width(125)))
            {
                GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
            EditorGUILayout.EndHorizontal();
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
            GUILayout.Space(10);
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            List<EditorBuildSettingsScene> listScene = new List<EditorBuildSettingsScene>(scenes);
            #region This is system to check if the path is in the list and group it.
            //listScene.Sort((x, y) => x.path.CompareTo(y.path));
            //List<string> lablePathName = new List<string>();
            #endregion
            for (int i = 0; i < listScene.Count; i++)
            {
                EditorBuildSettingsScene scene = listScene[i];
                string[] splitArray = scene.path.Split(char.Parse("/"));
                string tempLablePathName = "";
                for (int j = 0; j < splitArray.Length - 1; j++)
                {
                    tempLablePathName += splitArray[j];
                    if (j < splitArray.Length - 2)
                        tempLablePathName += "/";
                }
                #region This is system to check if the path is in the list and group it.
                //if (!CheckPathInList(tempLablePathName, lablePathName))
                //{
                //    lablePathName.Add(tempLablePathName);
                //    GUILayout.Space(10);
                //    GUILayout.Label(tempLablePathName, EditorStyles.boldLabel);
                //}
                #endregion
                string sceneName = splitArray[splitArray.Length - 1].Replace(".unity", "");
                GUIStyle styleBtnChangeScene = GetStyleButtonChangeScene(listScene[i].enabled);
                GUIContent contentChangeScene = new GUIContent($"{sceneName}", $"Change Scene To {sceneName}");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(contentChangeScene, styleBtnChangeScene))
                {
                    if (m_isNeedSave)
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    switch (m_selectedTypeIndex)
                    {
                        case 0: // Single
                            EditorSceneManager.OpenScene(listScene[i].path);
                            break;
                        case 1: // Additive
                            EditorSceneManager.OpenScene(listScene[i].path, OpenSceneMode.Additive);
                            break;
                        case 2: // Additive Without Loading
                            EditorSceneManager.OpenScene(listScene[i].path, OpenSceneMode.AdditiveWithoutLoading);
                            break;
                    }
                }
                GUIStyle styleBtnFocusPath = GetStyleButtonFocusPath();
                GUIContent contentFocusPath = new GUIContent(">", "Focus scene path");
                if (GUILayout.Button(contentFocusPath, styleBtnFocusPath))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(scene.path, typeof(Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// This is style for button load path.
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetStyleButtonFocusPath()
        {
            GUIStyle styleBtnLoadPath = new GUIStyle(GUI.skin.button);
            styleBtnLoadPath.fontSize = 10;
            styleBtnLoadPath.normal.textColor = Color.white;
            styleBtnLoadPath.hover.textColor = Color.white;
            styleBtnLoadPath.stretchWidth = false;
            styleBtnLoadPath.stretchHeight = true;
            return styleBtnLoadPath;
        }

        /// <summary>
        /// This is style for button change scene.
        /// </summary>
        /// <param name="isEnable">
        /// Should be pass scene.enabled to this param.
        /// </param>
        /// <returns></returns>
        private GUIStyle GetStyleButtonChangeScene(bool isEnable)
        {
            GUIStyle styleBtnChangeScene = new GUIStyle(GUI.skin.button);
            styleBtnChangeScene.fontSize = 12;
            styleBtnChangeScene.fontStyle = FontStyle.Bold;
            styleBtnChangeScene.normal.textColor = isEnable ? Color.white : Color.red;
            styleBtnChangeScene.hover.textColor = isEnable ? Color.white : Color.red;
            return styleBtnChangeScene;
        }

        /// <summary>
        /// This function is used to check if the path is in the list.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool CheckPathInList(string path, List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == path)
                    return true;
            }
            return false;
        }
    }
}
#endif