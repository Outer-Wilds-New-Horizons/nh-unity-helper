using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateAssetBundles
{
    public static List<T> FindAssetsByTypeInBundle<T>(string bundle) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:GameObject", new[]{"Assets"});
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            if (AssetImporter.GetAtPath(assetPath).assetBundleName == bundle.ToLower())
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
        }
        return assets;
    }

    static void BuildAll()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }

    static List<GameObject> GetPrefabs()
    {
        return FindAssetsByTypeInBundle<GameObject>(PlayerSettings.productName);
    }

    static List<float> RemoveAllGuides(List<GameObject> prefabs)
    {
        List<float> sizes = new List<float>();
        foreach (GameObject prefab in prefabs)
        {
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);
            MockPlanet mockPlanet = contentsRoot.GetComponentInChildren<MockPlanet>();
            if (mockPlanet != null)
            {
                sizes.Add(mockPlanet.planetSize);

                Object.DestroyImmediate(mockPlanet.gameObject);

                PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
                PrefabUtility.UnloadPrefabContents(contentsRoot);
            }
            else
            {
                sizes.Add(-1);
            }
        }

        return sizes;
    }

    static void ReAddGuides(List<GameObject> prefabs, List<float> sizes)
    {
        int count = 0;
        foreach (GameObject prefab in prefabs)
        {
            if (sizes[count] != -1)
            {
                string assetPath = AssetDatabase.GetAssetPath(prefab);
                GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);
                GameObject newGuide = new GameObject("Planet Guide", typeof(MockPlanet), typeof(SphereCollider));
                newGuide.GetComponent<MockPlanet>().planetSize = sizes[count];
                newGuide.GetComponent<SphereCollider>().radius = 1;
                newGuide.transform.SetParent(contentsRoot.transform);
                PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
                PrefabUtility.UnloadPrefabContents(contentsRoot);
            }
            count++;
        }
    }
    
    [MenuItem("Assets/New Horizons/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        
        List<GameObject> prefabs = GetPrefabs();
        List<float> sizes = RemoveAllGuides(prefabs);
        BuildAll();
        ReAddGuides(prefabs, sizes);
    }
}
