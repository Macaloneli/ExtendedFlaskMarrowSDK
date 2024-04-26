using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.Warehouse
{
    public class FlaskLabel : ScriptableObject
    {
#if UNITY_EDITOR
        public MonoScript[] Elixirs {
            get {
                if (_elixirCache == null || _elixirCache.Length == 0 && (elixirGUIDs != null && elixirGUIDs.Length != 0))
                {
                    if (elixirGUIDs != null)
                    {
                        SetCacheToNames();
                    }
                    else
                    {
                        _elixirCache = new MonoScript[0];
                    }
                }
                return _elixirCache;
            }
            set
            {
                _elixirCache = value;
                
                List<string> fullNames = new List<string>();
                foreach (MonoScript mscript in _elixirCache)
                {
                    if (mscript == null)
                        continue;

                    fullNames.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mscript)));
                }
                elixirGUIDs = fullNames.ToArray();
            }
        }

        public MonoScript[] LoadCacheFromNames()
        {
            List<MonoScript> types = new List<MonoScript>();

            foreach (string typeName in elixirGUIDs)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(typeName));
                if (script == null)
                {
                    Debug.Log($"{typeName} could not be found");
                }
                types.Add(script);
            }
            return types.ToArray();
        }

        public void SetCacheToNames()
        {
            MonoScript[] scripts = LoadCacheFromNames();
            if (scripts != null)
            {
                _elixirCache = scripts;
            }
        }

        private MonoScript[] _elixirCache;
#endif
        public string[] elixirGUIDs;
        public bool useDefaultIngredients = true;
        public string[] ingredients;
        public string[] additionalIngredients;
#if UNITY_EDITOR
        [MenuItem("Stress Level Zero/Alchemy/Create Flask Label Based on Open Scenes")]
        public static void CreateFlaskInfo()
        {
            FlaskLabel asset = ScriptableObject.CreateInstance<FlaskLabel>();
            asset.useDefaultIngredients = true;
            asset.Elixirs = Elixir.GetAllElixirsFromScene();
            AssetDatabase.CreateAsset(asset, $"Assets/Flask Label {SceneManager.GetActiveScene().name}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
#endif

    }
}
