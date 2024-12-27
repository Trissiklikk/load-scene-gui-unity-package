#if UNITY_EDITOR
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class LoadSceneSaveHandler
    {
        private const string SAVE_FAVORITE_PATH = "Load_Scene_GUI";
        private const string FILE_NAME = "FavoriteData.dat";
        private const string KEY_NAME = "FavoriteData";

        /// <summary>
        /// Method for save favorite scene.
        /// </summary>
        [Obsolete("This method is not used anymore. Now we used function SaveFavoriteWithEditorPrefs()")]
        public void SaveFavoriteWithPersistentData(JToken jToken)
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FAVORITE_PATH);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filePath = Path.Combine(path, FILE_NAME);
            string json = jToken.ToString();
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Method for load favorite scene.
        /// </summary>
        [Obsolete("This method is not used anymore. Now we used function LoadFavoriteWithEditorPrefs()")]
        public JToken LoadFavoriteWithPersistentData()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FAVORITE_PATH);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (Path.GetFileName(file) != FILE_NAME)
                    continue;

                string content = File.ReadAllText(file);
                JToken jsonToken = JsonConvert.DeserializeObject(content) as JToken;

                return jsonToken;
            }

            return null;
        }

        /// <summary>
        /// Method for save favorite scene with EditorPrefs.
        /// </summary>
        public void SaveFavoriteWithEditorPrefs(JToken jToken)
        {
            string json = jToken.ToString();
            EditorPrefs.SetString(KEY_NAME, json);
        }

        /// <summary>
        /// Method for load favorite scene with EditorPrefs.
        /// </summary>
        public JToken LoadFavoriteWithEditorPrefs()
        {
            string json = EditorPrefs.GetString(KEY_NAME);
            JToken jsonToken = JsonConvert.DeserializeObject(json) as JToken;

            return jsonToken;
        }

        /// <summary>
        /// Method for clear all favorite scene.
        /// </summary>
        public void ClearAllFavorite()
        {
            EditorPrefs.DeleteKey(KEY_NAME);
        }
    }
}
#endif