using System.IO;
using UnityEditor;
using UnityEngine;

// TODO compile to dll
public class AssetImportCop : AssetPostprocessor
{
    static T FindRuleForMeshAsset<T>(string path, string assetFilter) where T : Object
    {
        return SearchRecursive<T>(path, assetFilter);
    }

    private static T SearchRecursive<T>(string path, string assetFilter) where T : Object
    {
        foreach (string findAsset in AssetDatabase.FindAssets(assetFilter, new[] { Path.GetDirectoryName(path) }))
        {
            string p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
            if (p == Path.GetDirectoryName(path))
            {
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
        //Debug.Log("Searching: " + path);
        if (path != "Assets")
            return SearchRecursive<T>(path, assetFilter);

        //no matches
        return null;
    }

    private static void ExcuteMeshRule(AssetImporter importer)
    {
        MeshAssetRule rule = FindRuleForMeshAsset<MeshAssetRule>(importer.assetPath, "t:MeshAssetRule");
        if (rule == null)
        {
            Debug.Log("No asset rules found for asset");
        }
        else
        {
            Debug.Log("Begin to Applay Mesh Settings");
            rule.ApplyMeshSettings(importer);
        }
    }

    private void OnPreprocessModel()
    {
        ExcuteMeshRule(assetImporter);
        
    }

    private static void ExcuteTextureRule(AssetImporter importer)
    {
        TextureAssetRule rule = FindRuleForMeshAsset<TextureAssetRule>(importer.assetPath, "t:TextureAssetRule");
        if (rule == null)
        {
            Debug.Log("No asset rules found for asset");
        }
        else
        {
            Debug.Log("Begin to Applay Mesh Settings");
            rule.ApplySettings(importer);
        }
    }

    private void OnPreprocessTexture()
    {
        ExcuteTextureRule(assetImporter);
        
    }

    private void OnPreprocessAnimation()
    {
        Debug.Log("no animation rules");
    }

    public void OnPreprocessAudio()
    {
        Debug.Log("no audio rules");
    }


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < movedAssets.Length; i++)
        {
            AssetImporter importer = AssetImporter.GetAtPath(movedAssets[i]);

            //model:
            ExcuteMeshRule(importer);

            //texture:
            ExcuteTextureRule(importer);


        }
    }
}
