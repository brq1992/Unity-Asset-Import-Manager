using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DDSMeshImportRule : ScriptableObject
{
   public float m_GlobalScale;
   public bool m_UseFileScale;
   public float m_FileScale;
   public ModelImporterMeshCompression m_MeshCompression;
   public bool m_IsReadable;
   public bool m_OptimizeMeshForGPU;
   public bool m_ImportBlendShapes;
   public bool m_AddColliders;
   public bool m_KeepQuads;
   public bool m_WeldVertices;
   public bool m_SwapUVChannels;
   public bool m_GenerateSecondaryUV;
    public float m_SecondaryUVAngleDistortion = 8f;
    public float m_SecondaryUVAreaDistortion = 15f;
    public float m_SecondaryUVHardAngle = 88f;
    public float m_SecondaryUVPackMargin = 4;
   public ModelImporterNormals m_NormalImportMode;
   public float m_NormalSmoothAngle;
   public ModelImporterTangents m_TangentImportMode;
    public bool m_SecondaryUVAdvancedOptions;
    public bool TipsForGenerate2UV;
    //SerializedProperty m_NormalCalculationMode;
    //SerializedProperty m_IndexFormat;
    //SerializedProperty m_ImportCameras;
    //SerializedProperty m_ImportLights;
    //SerializedProperty m_ImportVisibility;
    //SerializedProperty m_PreserveHierarchy;

    [MenuItem("Assets/Manager/Test/Create Mesh Import Rules")]
    public static void CreateAssetRule()
    {
        DDSMeshImportRule newRule = CreateInstance<DDSMeshImportRule>();

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
        string newRuleFileName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectionpath, "New Mesh Rule.asset"));
        newRuleFileName = newRuleFileName.Replace("\\", "/");
        AssetDatabase.CreateAsset(newRule, newRuleFileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRule;
    }

    ///todo:考虑模型是否支持切线
}
