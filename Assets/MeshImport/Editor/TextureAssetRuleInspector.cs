using System.IO;
using Boo.Lang;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TextureAssetRule))]
public class TextureAssetRuleInspector : Editor
{
    private static bool changed;
    private TextureAssetRule orig;

    [MenuItem("Assets/Manager/Create/Create Texture Import Rules")]
    public static void CreatAssetRule()
    {
        TextureAssetRule newRule = CreateInstance<TextureAssetRule>();
        newRule.ApplyDefaults();

        string selectionpath = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            selectionpath = AssetDatabase.GetAssetPath(obj);
            Debug.Log("selectionpath in foreach: " + selectionpath);
            if (File.Exists(selectionpath))
            {
                Debug.Log("File.Exists: " + selectionpath);
                selectionpath = Path.GetDirectoryName(selectionpath);
            }
            break;
        }
        Debug.Log("selectionpath: " + selectionpath);

        string newRuleFileName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectionpath, "New Asset Rule.asset"));
        newRuleFileName = newRuleFileName.Replace("\\", "/");
        AssetDatabase.CreateAsset(newRule, newRuleFileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRule;
        changed = true;

    }

    public override void OnInspectorGUI()
    {
        TextureAssetRule t = (TextureAssetRule) target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TextureImportRules");
        EditorGUILayout.EndHorizontal();


        DrawTextureSettings(t);

        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        if (changed)
        {
            if (GUILayout.Button("Apply"))
                Apply(t);
            EditorUtility.SetDirty(t);
        }
    }

    private void Apply(TextureAssetRule assetRule)
    {

        //get the directories that we do not want to apply changes to 
        List<string> dontapply = new List<string>();
        string assetrulepath = AssetDatabase.GetAssetPath(assetRule).Replace(assetRule.name + ".asset", "").TrimEnd('/');
        string projPath = Application.dataPath;
        projPath = projPath.Remove(projPath.Length - 6);

        string[] directories = Directory.GetDirectories(Path.GetDirectoryName(projPath + AssetDatabase.GetAssetPath(assetRule)), "*", SearchOption.AllDirectories);
        foreach (string directory in directories)
        {
            string d = directory.Replace(Application.dataPath, "Assets");
            string[] appDirs = AssetDatabase.FindAssets("t:AssetRule", new[] { d });
            if (appDirs.Length != 0)
            {
                d = d.TrimEnd('/');
                d = d.Replace('\\', '/');
                dontapply.Add(d);
            }
        }

        List<string> finalAssetList = new List<string>();
        foreach (string findAsset in AssetDatabase.FindAssets("", new[] { assetrulepath }))
        {
            string asset = AssetDatabase.GUIDToAssetPath(findAsset);
            if (!File.Exists(asset))
                continue;
            if (dontapply.Contains(Path.GetDirectoryName(asset)))
                continue;
            if (!assetRule.IsMatch(AssetImporter.GetAtPath(asset)))
                continue;
            if (finalAssetList.Contains(asset))
                continue;
            if (asset == AssetDatabase.GetAssetPath(assetRule))
                continue;
            finalAssetList.Add(asset);
        }

        foreach (string asset in finalAssetList)
        {
            AssetImporter.GetAtPath(asset).SaveAndReimport();
        }

        changed = false;
    }

    private void DrawTextureSettings(TextureAssetRule t)
    {
        t.IsReadable = EditorGUILayout.Toggle(new GUIContent("Read/Write Enabled","Enable to be able to access "), t.IsReadable);
        t.IsMipmap = EditorGUILayout.Toggle(new GUIContent("Generate Mip Maps"), t.IsMipmap);
    }

    void OnEnable()
    {
        changed = false;
        orig = (TextureAssetRule)target;

        Undo.RecordObject(target, "assetruleundo");
    }
}

[System.Serializable]
public class TextureAssetRule : ScriptableObject
{
    public bool IsReadable;
    public bool IsMipmap;
    private bool dirty;

    public void ApplyDefaults()
    {
        IsReadable = false;
        IsMipmap = false;
    }

    public bool IsMatch(AssetImporter importer)
    {
        if (importer is TextureImporter)
        {
            return true;
        }
        return false;
    }

    public void ApplySettings(AssetImporter assetImporter)
    {
        TextureImporter importer = assetImporter as TextureImporter;
        if (importer != null)
            ApplyMeshSettings((TextureImporter)assetImporter);
    }

    private void ApplyMeshSettings(TextureImporter assetImporter)
    {
        if (assetImporter.isReadable != IsReadable)
        {
            assetImporter.isReadable = IsReadable;
            dirty = true;
        }

        if (assetImporter.mipmapEnabled != IsMipmap)
        {
            assetImporter.mipmapEnabled = IsMipmap;
            dirty = true;
        }
    }
}
