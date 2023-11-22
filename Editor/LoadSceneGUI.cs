#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class LoadSceneGUI : EditorWindow, IHasCustomMenu
    {
        /// <summary>
        /// Static mathod to show the window.
        /// </summary>
        [MenuItem("Window/Trissiklikk Editor Tools/Load Scene GUI")]
        [Shortcut("LoadSceneGUI", KeyCode.F11)]
        public static void ShowWindow()
        {
            GetWindow(typeof(LoadSceneGUI));
        }

        private const string SAVE_FAVORITE_PATH = "Load_Scene_GUI";
        private const string FILE_NAME = "FavoriteData.dat";

        private static List<string> s_scenePathList;

        private bool m_isNeedSave;
        private int m_selectedTypeIndex;
        private string[] m_loadSceneTypes;
        private Vector2 m_scrollPosition;

        private LoadSceneGUI()
        {
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
            LoadFavorite();
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
            //listScene = new List<EditorBuildSettingsScene>(SortList(listScene));
            for (int i = 0; i < s_scenePathList.Count; i++)
            {
                listScene = listScene.OrderByDescending(x => x.path.Contains("/" + s_scenePathList[i] + ".unity")).ToList();
            }
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
                GUIStyle styleBtnFavorite = GetStyleFavoriteButton(sceneName);
                GUIContent contentFavorite = new GUIContent("♥", "Favorite this scene");
                if (GUILayout.Button(contentFavorite, styleBtnFavorite))
                {
                    if (!s_scenePathList.Contains(sceneName))
                        s_scenePathList.Add(sceneName);
                    else
                        s_scenePathList.Remove(sceneName);
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
        /// Called when the window is closed.
        /// </summary>
        public void SaveFavorite()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FAVORITE_PATH);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filePath = Path.Combine(path, FILE_NAME);
            string json = FavoriteToJson().ToString();
            File.WriteAllText(filePath, json);
            LoadFavorite();
        }

        /// <summary>
        /// Called when the window is opened.
        /// </summary>
        public void LoadFavorite()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FAVORITE_PATH);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                string content = File.ReadAllText(file);
                JToken jsonToken = JsonConvert.DeserializeObject(content) as JToken;
                CreateFavoriteFormJson(jsonToken);
            }
        }

        /// <summary>
        /// This method is called when the window is closed.
        /// </summary>
        /// <param name="json"></param>
        private void CreateFavoriteFormJson(JToken json)
        {
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
        /// This is stlye for button favorite.
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetStyleFavoriteButton(string sceneName)
        {
            GUIStyle styleBtnFavorite = new GUIStyle(GUI.skin.button);
            styleBtnFavorite.fontSize = 12;
            styleBtnFavorite.normal.textColor = CheckIsFavorite(sceneName) ? Color.red : new Color(1, 1, 1, 0.25f);
            styleBtnFavorite.hover.textColor = CheckIsFavorite(sceneName) ? Color.red : new Color(1, 1, 1, 0.5f);
            styleBtnFavorite.stretchHeight = false;
            styleBtnFavorite.stretchWidth = false;
            return styleBtnFavorite;
        }

        /// <summary>
        /// This is style for button load path.
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetStyleButtonFocusPath()
        {
            GUIStyle styleBtnLoadPath = new GUIStyle(GUI.skin.button);
            styleBtnLoadPath.fontSize = 12;
            styleBtnLoadPath.normal.textColor = Color.white;
            styleBtnLoadPath.hover.textColor = Color.white;
            styleBtnLoadPath.stretchHeight = false;
            styleBtnLoadPath.stretchWidth = false;
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

        /// <summary>
        /// This is function for add menu to kebab button at top-right this gui editor.
        /// Should inherit IHasCustomMenu to used this method.
        /// </summary>
        /// <param name="menu"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void AddItemsToMenu(GenericMenu menu)
        {
            //menu.AddItem(new GUIContent("Refresh"), false, LoadFavorite);
            menu.AddItem(new GUIContent("Save Favorite"), false, SaveFavorite);
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