using System.IO;
using UnityEditor;
using UnityEngine;

// TODO compile to dll
public class AssetImportCop : AssetPostprocessor
{
    MeshAssetRule FindRuleForMeshAsset(string path)
    {
        return SearchRecursive(path);
    }

    private MeshAssetRule SearchRecursive(string path)
    {
        foreach (string findAsset in AssetDatabase.FindAssets("t:MeshAssetRule", new[] { Path.GetDirectoryName(path) }))
        {
            string p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
            if (p == Path.GetDirectoryName(path))
            {
                Debug.Log("Found AssetRule for Asset Rule" + AssetDatabase.GUIDToAssetPath(findAsset));
                {
                    return AssetDatabase.LoadAssetAtPath<MeshAssetRule>(AssetDatabase.GUIDToAssetPath(findAsset));
                }
            }
        }
        //no match so go up a level
        path = Directory.GetParent(path).FullName;
        path = path.Replace('\\','/');
        path = path.Remove(0, Application.dataPath.Length);
        path = path.Insert(0, "Assets");
        Debug.Log("Searching: " + path);
        if (path != "Assets")
            return SearchRecursive(path);

        //no matches
        return null;
    }

    //private void OnPreprocessTexture()
    //{
    //    AssetRule rule = FindRuleForAsset(assetImporter.assetPath);

    //    if (rule == null)
    //    {
    //        Debug.Log("No asset rules found for asset");
    //        return;
    //    }

    //    Debug.Log("Modifying Texture settings");
    //    rule.ApplySettings(assetImporter);
    //}

    private void OnPreprocessModel()
    {
        MeshAssetRule rule = FindRuleForMeshAsset(assetImporter.assetPath);
        if (rule == null)
        {
            Debug.Log("No asset rules found for asset");
        }
        else
        {
            Debug.Log("Begin to Applay Mesh Settings");
            rule.ApplyMeshSettings(assetImporter);
        }
    }
}
