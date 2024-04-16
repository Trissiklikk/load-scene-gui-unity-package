#if UNITY_EDITOR
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [Obsolete("This method is not used anymore. Now we used function SaveFavoriteWithPlayerPrefs()")]
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
        [Obsolete("This method is not used anymore. Now we used function LoadFavoriteWithPlayerPrefs()")]
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
        /// Method for save favorite scene with PlayerPrefs.
        /// </summary>
        public void SaveFavoriteWithPlayerPrefs(JToken jToken)
        {
            string json = jToken.ToString();
            PlayerPrefs.SetString(KEY_NAME, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Method for load favorite scene with PlayerPrefs.
        /// </summary>
        public JToken LoadFavoriteWithPlayerPrefs()
        {
            string json = PlayerPrefs.GetString(KEY_NAME);
            JToken jsonToken = JsonConvert.DeserializeObject(json) as JToken;

            return jsonToken;
        }
    }
}
#endif