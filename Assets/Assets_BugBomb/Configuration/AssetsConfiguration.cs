using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "Assets", menuName = "Configuration/Assets", order = 2)]
[ExecuteInEditMode]
public class AssetsConfiguration : Configuration
{
    [Header("Prefabs")]
    public Block DefenderBlockPrefab;
    public Block BlockPrefab;
    public Block MinusBlockPrefab;
    public Block MultiplierBlockPrefab;

    [Header("Materials")]
    public ColorThemeSO ColorTheme;
    public int[] DefenderBlockNumbersToChangeMaterial;


#if UNITY_EDITOR
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }
#endif


    private static AssetsConfiguration _instance;

    public static AssetsConfiguration instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load<AssetsConfiguration>();
            }
            return _instance;
        }
        set {
            _instance = value;
        }
    }
}
