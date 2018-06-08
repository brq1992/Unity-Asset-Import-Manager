using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(DDSMeshImportRule))]
public class DDSMeshImportRuleEditor : Editor
{
    string[] m_TabNames = null;
    private int m_ActiveEditorIndex = 0;
    private DDSBaseAssetImporterTabUI activeTab;
    private DDSBaseAssetImporterTabUI[] m_Tabs = null;
    private bool changed;
    protected DDSBaseAssetImporterTabUI[] tabs { get { return m_Tabs; } set { m_Tabs = value; } }

    void OnEnable()
    {
        
        m_TabNames = new[] { "Model", "Rig", "Animation", "Materials" };
        tabs = new DDSBaseAssetImporterTabUI[] { new DDSModelImporterModelEditor(this), new DDSModelImporterRigEditor(this), new ModelImporterClipEditor(this), new DDSModelImporterMaterialEditor(this) };
        foreach (var tab in m_Tabs)
        {
            tab.OnEnable();
        }
        m_ActiveEditorIndex = 0;
        activeTab = m_Tabs[m_ActiveEditorIndex];
    }

    public override void OnInspectorGUI()
    {
        DDSMeshImportRule t = (DDSMeshImportRule)target;

        EditorGUI.BeginChangeCheck();


        // Always allow user to switch between tabs even when the editor is disabled, so they can look at all parts
        // of read-only assets
        using (new EditorGUI.DisabledScope(false)) // this doesn't enable the UI, but it seems correct to push the stack
        {
            GUI.enabled = true;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
                {
                    m_ActiveEditorIndex = GUILayout.Toolbar(m_ActiveEditorIndex, m_TabNames, "LargeButton");
                    if (check.changed)
                    {
                        EditorPrefs.SetInt(GetType().Name + "ActiveEditorIndex", m_ActiveEditorIndex);
                        activeTab = m_Tabs[m_ActiveEditorIndex];
                        activeTab.OnInspectorGUI(t);
                    }
                }
                GUILayout.FlexibleSpace();
            }
        }

        // the activeTab can get destroyed when opening particular sub-editors (such as the Avatar configuration editor on the Rig tab)
        if (activeTab != null)
        {
           activeTab.OnInspectorGUI(t);
        }

        //// show a single Apply/Revert set of buttons for all the tabs
        ApplyRevertGUI();


        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        if (changed)
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
                Apply(t);
            EditorUtility.SetDirty(t);
            GUILayout.EndHorizontal();
        }
    }

    private void Apply(DDSMeshImportRule assetRule)
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


    protected void ApplyRevertGUI()
    {

    }
}
