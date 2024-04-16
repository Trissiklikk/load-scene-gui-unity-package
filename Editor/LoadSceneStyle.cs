#if UNITY_EDITOR
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class LoadSceneStyle
    {
        /// <summary>
        /// This is stlye for button favorite.
        /// </summary>
        /// <returns></returns>
        public GUIStyle GetStyleFavoriteButton(bool isFavorite)
        {
            GUIStyle styleBtnFavorite = new GUIStyle(GUI.skin.button);
            styleBtnFavorite.fontSize = 12;
            styleBtnFavorite.normal.textColor = isFavorite ? Color.red : new Color(1, 1, 1, 0.25f);
            styleBtnFavorite.hover.textColor = isFavorite ? Color.red : new Color(1, 1, 1, 0.5f);
            styleBtnFavorite.stretchHeight = false;
            styleBtnFavorite.stretchWidth = false;
            return styleBtnFavorite;
        }

        /// <summary>
        /// This is style for button load path.
        /// </summary>
        /// <returns></returns>
        public GUIStyle GetStyleButtonFocusPath()
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
        public GUIStyle GetStyleButtonChangeScene(bool isEnable)
        {
            GUIStyle styleBtnChangeScene = new GUIStyle(GUI.skin.button);
            styleBtnChangeScene.fontSize = 12;
            styleBtnChangeScene.fontStyle = FontStyle.Bold;
            styleBtnChangeScene.normal.textColor = isEnable ? Color.white : Color.red;
            styleBtnChangeScene.hover.textColor = isEnable ? Color.white : Color.red;
            return styleBtnChangeScene;
        }
    }
}
#endif