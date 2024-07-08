#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Trissiklikk.EditorTools
{
    public sealed class LoadSceneGUI : EditorWindow, IHasCustomMenu
    {
        /// <summary>
        /// Static mathod to show the window.
        /// </summary>
        [MenuItem("Window/Trissiklikk Editor Tools/Load Scene GUI Edit %#F11")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LoadSceneGUI));
        }

        private static List<string> s_scenePathList;

        private LoadSceneStyle m_loadSceneStyle;
        private LoadSceneSaveHandler m_loadSceneSaveHandler;
        private bool m_isNeedSave;
        private int m_selectedTypeIndex;
        private string[] m_loadSceneTypes;
        private Vector2 m_scrollPosition;

        private LoadSceneGUI()
        {
            m_loadSceneStyle = new LoadSceneStyle();
            m_loadSceneSaveHandler = new LoadSceneSaveHandler();
            s_scenePathList = new List<string>();
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

        private void OnEnable()
        {
            LoadAndInitFavorite();
        }

        private void OnDestroy()
        {
            SaveFavorite();
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

            for (int i = 0; i < s_scenePathList.Count; i++)
            {
                listScene = listScene.OrderByDescending(x => x.path.Contains("/" + s_scenePathList[i] + ".unity")).ToList();
            }

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

                string sceneName = splitArray[splitArray.Length - 1].Replace(".unity", "");
                GUIStyle styleBtnChangeScene = m_loadSceneStyle.GetStyleButtonChangeScene(listScene[i].enabled);
                GUIContent contentChangeScene = new GUIContent($"{sceneName}", $"Change Scene To {sceneName}");
                EditorGUILayout.BeginHorizontal();
                GUIStyle styleBtnFavorite = m_loadSceneStyle.GetStyleFavoriteButton(CheckIsFavorite(sceneName));
                GUIContent contentFavorite = new GUIContent("♥", "Favorite this scene");

                if (GUILayout.Button(contentFavorite, styleBtnFavorite))
                {
                    if (!s_scenePathList.Contains(sceneName))
                        s_scenePathList.Add(sceneName);
                    else
                        s_scenePathList.Remove(sceneName);

                    SaveFavorite();
                }

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

                GUIStyle styleBtnFocusPath = m_loadSceneStyle.GetStyleButtonFocusPath();
                GUIContent contentFocusPath = new GUIContent(">", "Focus scene path");

                if (GUILayout.Button(contentFocusPath, styleBtnFocusPath))
                {
                    UnityObject obj = AssetDatabase.LoadAssetAtPath(scene.path, typeof(UnityObject));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// This is function for add menu to kebab button at top-right this gui editor.
        /// Should inherit IHasCustomMenu to used this method.
        /// </summary>
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Save Favorite"), false, SaveFavorite);
            menu.AddItem(new GUIContent("Clear Favorite"), false, () =>
            {
                ClearFavorite();
                LoadAndInitFavorite();
            });
        }

        /// <summary>
        /// This function is used to save favorite scene.
        /// </summary>
        private void SaveFavorite()
        {
            JToken json = FavoriteToJson();
            m_loadSceneSaveHandler.SaveFavoriteWithPlayerPrefs(json);
        }

        /// <summary>
        /// This function is used to clear all favorite scene.
        /// </summary>
        private void ClearFavorite()
        {
            m_loadSceneSaveHandler.ClearAllFavorite();
        }

        /// <summary>
        /// This method is called when the window is closed.
        /// </summary>
        private void LoadAndInitFavorite()
        {
            JToken json = m_loadSceneSaveHandler.LoadFavoriteWithPlayerPrefs();

            s_scenePathList = new List<string>();
            if (json is JArray)
            {
                JArray jArray = (JArray)json;
                for (int i = 0; i < jArray.Count; i++)
                {
                    s_scenePathList.Add(jArray[i].Value<string>());
                }
            }
        }

        /// <summary>
        /// This for save favorite data to json.
        /// </summary>
        /// <returns></returns>
        private JToken FavoriteToJson()
        {
            JArray jArray = new JArray();
            for (int i = 0; i < s_scenePathList.Count; i++)
            {
                jArray.Add(s_scenePathList[i]);
            }
            return jArray;
        }

        /// <summary>
        /// This function is used to check if the scene is in the favorite list.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool CheckIsFavorite(string sceneName)
        {
            for (int i = 0; i < s_scenePathList.Count; i++)
            {
                if (s_scenePathList[i] == sceneName)
                    return true;
            }
            return false;
        }
    }
}
#endif