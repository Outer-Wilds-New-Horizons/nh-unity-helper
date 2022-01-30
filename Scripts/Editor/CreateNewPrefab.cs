using UnityEditor;
using UnityEngine;

public class CreateNewPrefab
{

    static bool AssetExists(string name, string folder)
    {
        string[] guidList = AssetDatabase.FindAssets(name, new []{ folder });
        return guidList.Length > 0;
    }
    
    private static void CreateTag(string tag) {
        var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null) { // sanity checking
            var so = new SerializedObject(asset);
            var tags = so.FindProperty("tags");

            var numTags = tags.arraySize;
            // do not create duplicates
            for (int i = 0; i < numTags; i++) {
                var existingTag = tags.GetArrayElementAtIndex(i);
                if (existingTag.stringValue == tag) return;
            }
            tags.InsertArrayElementAtIndex(numTags);
            tags.GetArrayElementAtIndex(numTags).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    
    [MenuItem("Assets/New Horizons/New Prop")]
    static void CreateNewPropPrefab()
    {
        CreateTag("Detail");
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

    [MenuItem("Assets/New Horizons/New Planet Details")]
    static void CreateNewDetailsPrefab()
    {
        int currentIter = 0;
        while (AssetExists("NewProp" + currentIter, "Assets"))
        {
            currentIter++;
        }
        string assetPath = "Assets/NewPlanetDetails" + currentIter + ".prefab";
        GameObject newGameObject = new GameObject();
        GameObject mockPlanet = new GameObject("Planet Guide", typeof(MockPlanet));
        mockPlanet.AddComponent<SphereCollider>().radius = 1;
        mockPlanet.transform.SetParent(newGameObject.transform);
        mockPlanet.tag = "EditorOnly";
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(newGameObject, assetPath);
        AssetDatabase.OpenAsset(newPrefab);
        Object.DestroyImmediate(newGameObject);
        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(PlayerSettings.productName, "");
    }
}
