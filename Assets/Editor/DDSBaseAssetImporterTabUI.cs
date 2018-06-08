using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DDSBaseAssetImporterTabUI
{
    DDSBaseAssetImporterTabUI m_PanelContainer = null;

    public SerializedObject serializedObject
    {
        get { return m_PanelContainer.serializedObject; }
    }

    protected DDSBaseAssetImporterTabUI(DDSMeshImportRuleEditor panelContainer)
    {
        
    }


    public virtual void OnInspectorGUI(DDSMeshImportRule ddsMeshImportRule)
    {
    }

    internal virtual void OnEnable()
    {
        
    }
}
