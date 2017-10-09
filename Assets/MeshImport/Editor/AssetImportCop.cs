using System.IO;
using UnityEditor;
using UnityEngine;

// TODO compile to dll
public class AssetImportCop : AssetPostprocessor
{
    T FindRuleForMeshAsset<T>(string path, string assetFilter) where T : Object
    {
        return SearchRecursive<T>(path, assetFilter);
    }

    private T SearchRecursive<T>(string path,string assetFilter) where  T: Object
    {
        foreach (string findAsset in AssetDatabase.FindAssets(assetFilter, new[] { Path.GetDirectoryName(path) }))
        {
            string p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
            if (p == Path.GetDirectoryName(path))
            {
                Debug.Log("Found AssetRule for Asset Rule" + AssetDatabase.GUIDToAssetPath(findAsset));
                {
                    return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(findAsset));
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
            return SearchRecursive<T>(path, assetFilter);

        //no matches
        return null;
    }

    private void OnPreprocessModel()
    {
        MeshAssetRule rule = FindRuleForMeshAsset<MeshAssetRule>(assetImporter.assetPath, "t:MeshAssetRule");
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

    private void OnPreprocessTexture()
    {
        TextureAssetRule rule = FindRuleForMeshAsset<TextureAssetRule>(assetImporter.assetPath, "t:TextureAssetRule");
        if (rule == null)
        {
            Debug.Log("No asset rules found for asset");
        }
        else
        {
            Debug.Log("Begin to Applay Mesh Settings");
            rule.ApplySettings(assetImporter);
        }
    }

    private void OnPreprocessAnimation()
    {
        Debug.Log("no animation rules");
    }

    public void OnPreprocessAudio()
    {
        Debug.Log("no audio rules");
    }
}
