using UnityEditor;
using UnityEngine;

public class CreateNewPrefab
{

    static bool AssetExists(string name, string folder)
    {
        string[] guidList = AssetDatabase.FindAssets(name, new []{ folder });
        return guidList.Length > 0;
    }
    
    [MenuItem("Assets/New Horizons/New Prop or Detail")]
    static void CreateNewPropPrefab()
    {
        int currentIter = 0;
        while (AssetExists("NewProp" + currentIter, "Assets"))
        {
            currentIter++;
        }
        string assetPath = "Assets/NewProp" + currentIter + ".prefab";
        GameObject newGameObject = new GameObject();
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(newGameObject, assetPath);
        AssetDatabase.OpenAsset(newPrefab);
        Object.DestroyImmediate(newGameObject);
        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(PlayerSettings.productName, "");
    }
}
