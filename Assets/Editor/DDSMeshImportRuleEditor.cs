using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DDSMeshImportRule))]
public class DDSMeshImportRuleEditor : Editor
{
    string[] m_TabNames = null;
    private int m_ActiveEditorIndex = 0;
    private DDSBaseAssetImporterTabUI activeTab;
    private DDSBaseAssetImporterTabUI[] m_Tabs = null;
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
        //ApplyRevertGUI();
    }
}
