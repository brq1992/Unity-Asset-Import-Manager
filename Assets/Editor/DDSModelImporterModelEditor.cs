using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DDSModelImporterModelEditor : DDSBaseAssetImporterTabUI
{
    public DDSModelImporterModelEditor(DDSMeshImportRuleEditor panelContainer)
        : base(panelContainer)
    {
        
    }

    public override void OnInspectorGUI(DDSMeshImportRule t)
    {
        if (styles == null)
            styles = new Styles();

        MeshesGUI(t);
        NormalsAndTangentsGUI(t);
    }

    private void MeshesGUI(DDSMeshImportRule t)
    { 
        GUILayout.Label(styles.Meshes, EditorStyles.boldLabel);

        t.m_GlobalScale = EditorGUILayout.FloatField(styles.ScaleFactor, t.m_GlobalScale,new GUILayoutOption[0]);

        // File Scale Factor
        t.m_UseFileScale = EditorGUILayout.Toggle(styles.UseFileScale, t.m_UseFileScale);

        if (t.m_UseFileScale)
        {
            EditorGUI.indentLevel++;
            t.m_FileScale = EditorGUILayout.FloatField(styles.UseFileScale, t.m_FileScale, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }

        //// mesh compression
        t.m_MeshCompression = (ModelImporterMeshCompression)
                EditorGUILayout.EnumPopup(styles.MeshCompressionLabel, t.m_MeshCompression);

        t.m_IsReadable = EditorGUILayout.Toggle(styles.IsReadable, t.m_IsReadable);
        t.m_OptimizeMeshForGPU = EditorGUILayout.Toggle(styles.OptimizeMeshForGPU, t.m_OptimizeMeshForGPU);
        t.m_ImportBlendShapes = EditorGUILayout.Toggle(styles.ImportBlendShapes, t.m_ImportBlendShapes);
        t.m_AddColliders = EditorGUILayout.Toggle(styles.MeshCompressionLabel, t.m_AddColliders);

        using (new EditorGUI.DisabledScope(true))
        {
            t.m_KeepQuads = EditorGUILayout.Toggle(styles.KeepQuads, t.m_KeepQuads);
        }


        //EditorGUILayout.Popup(m_IndexFormat, styles.IndexFormatOpt, styles.IndexFormatLabel);

        //// Weld Vertices
        t.m_WeldVertices = EditorGUILayout.Toggle(styles.WeldVertices, t.m_WeldVertices);

        //// Import visibility
        //EditorGUILayout.PropertyField(m_ImportVisibility, styles.ImportVisibility);

        //// Import Cameras
        //EditorGUILayout.PropertyField(m_ImportCameras, styles.ImportCameras);

        //// Import Lights
        //EditorGUILayout.PropertyField(m_ImportLights, styles.ImportLights);

        //// Preserve Hierarchy
        //EditorGUILayout.PropertyField(m_PreserveHierarchy, styles.PreserveHierarchy);

        //// Swap uv channel
        t.m_SwapUVChannels = EditorGUILayout.Toggle(styles.SwapUVChannels, t.m_SwapUVChannels);

        // Secondary UV generation
        EditorGUILayout.BeginHorizontal();
        t.m_GenerateSecondaryUV = EditorGUILayout.Toggle(styles.GenerateSecondaryUV, t.m_GenerateSecondaryUV);
        if (t.m_GenerateSecondaryUV)
        {
            t.TipsForGenerate2UV = EditorGUILayout.Toggle(styles.TipsForGenerate2UV,t.TipsForGenerate2UV);
        }
        EditorGUILayout.EndHorizontal();
        if (t.m_GenerateSecondaryUV && t.TipsForGenerate2UV)
        {
            EditorGUI.indentLevel++;
            t.m_SecondaryUVAdvancedOptions = EditorGUILayout.Foldout(t.m_SecondaryUVAdvancedOptions,
                styles.GenerateSecondaryUVAdvanced, EditorStyles.foldout);
            if (t.m_SecondaryUVAdvancedOptions)
            {
                t.m_SecondaryUVHardAngle = Mathf.Max(EditorGUILayout.Slider(styles.secondaryUVHardAngle,
                    t.m_SecondaryUVHardAngle, 0f, 180f, new GUILayoutOption[0]), 0.1f);
                t.m_SecondaryUVPackMargin = Mathf.Max(EditorGUILayout.Slider(styles.secondaryUVPackMargin,
                    t.m_SecondaryUVPackMargin, 0f, 180f, new GUILayoutOption[0]), 0.1f);
                t.m_SecondaryUVAngleDistortion = Mathf.Max(EditorGUILayout.Slider(styles.secondaryUVAngleDistortion,
                    t.m_SecondaryUVAngleDistortion, 0f, 180f, new GUILayoutOption[0]), 0.1f);
                t.m_SecondaryUVAreaDistortion = Mathf.Max(EditorGUILayout.Slider(styles.secondaryUVAreaDistortion,
                    t.m_SecondaryUVAreaDistortion, 0f, 180f, new GUILayoutOption[0]), 0.1f);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void NormalsAndTangentsGUI(DDSMeshImportRule rule)
    {
        // Tangent space
        GUILayout.Label(styles.TangentSpace, EditorStyles.boldLabel);
        // TODO : check if normal import is supported!
        //normalImportMode = styles.TangentSpaceModeOptEnumsAll[EditorGUILayout.Popup(styles.TangentSpaceNormalLabel, (int)normalImportMode, styles.TangentSpaceModeOptLabelsAll)];
        EditorGUI.BeginChangeCheck();
        rule.m_NormalImportMode = (ModelImporterNormals)EditorGUILayout.EnumPopup(styles.TangentSpaceNormalLabel, rule.m_NormalImportMode);
        if (EditorGUI.EndChangeCheck())
        {
            // Let the tangent mode follow the normal mode - that's a sane default and it's needed
            // because the tangent mode value can't be lower than the normal mode.
            // We make the tangent mode follow in BOTH directions for consistency
            // - so that if you change the normal mode one way and then back, the tangent mode will also go back again.
            if (rule.m_NormalImportMode == ModelImporterNormals.None)
            {
                rule.m_TangentImportMode = ModelImporterTangents.None;
            }
            else if (rule.m_NormalImportMode == ModelImporterNormals.Import)
            {
                rule.m_TangentImportMode = ModelImporterTangents.Import;
            }
            else
            {
                rule.m_TangentImportMode = ModelImporterTangents.CalculateMikk;
            }
        }

        // Normal split angle
        using (new EditorGUI.DisabledScope(rule.m_NormalImportMode != ModelImporterNormals.Calculate))
        {
            rule.m_NormalSmoothAngle = EditorGUILayout.Slider(styles.SmoothingAngle, rule.m_NormalSmoothAngle, 0f, 180f);
        }
        GUIContent[] displayedOptions = styles.TangentSpaceModeOptLabelsAll;
        ModelImporterTangents[] array = styles.TangentSpaceModeOptEnumsAll;
        if (rule.m_NormalImportMode == ModelImporterNormals.Calculate)
        {
            displayedOptions = styles.TangentSpaceModeOptLabelsCalculate;
            array = styles.TangentSpaceModeOptEnumsCalculate;
        }
        else if (rule.m_NormalImportMode == ModelImporterNormals.None)
        {
            displayedOptions = styles.TangentSpaceModeOptLabelsNone;
            array = styles.TangentSpaceModeOptEnumsNone;
        }
        using (new EditorGUI.DisabledScope(rule.m_NormalImportMode == ModelImporterNormals.None))
        {
            int num = Array.IndexOf(array, rule.m_TangentImportMode);
            EditorGUI.BeginChangeCheck();
            num = EditorGUILayout.Popup(styles.TangentSpaceTangentLabel, num, displayedOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                rule.m_TangentImportMode = array[num];
            }
        }
    }

    static Styles styles;

    class Styles
    {
        public GUIContent Meshes = new GUIContent("Meshes", "These options control how geometry is imported.");
        public GUIContent ScaleFactor = new GUIContent("Scale Factor", "How much to scale the models compared to what is in the source file.");
        public GUIContent UseFileUnits = new GUIContent("Use File Units", "Detect file units and import as 1FileUnit=1UnityUnit, otherwise it will import as 1cm=1UnityUnit. See ModelImporter.useFileUnits for more details.");
        public GUIContent UseFileScale = new GUIContent("Use File Scale", "Use File Scale when importing.");
        public GUIContent FileScaleFactor = new GUIContent("File Scale", "Scale defined by source file, or 1 if Use File Scale is disabled. Click Apply to update.");
        public GUIContent ImportBlendShapes = new GUIContent("Import BlendShapes", "Should Unity import BlendShapes.");
        public GUIContent GenerateColliders = new GUIContent("Generate Colliders", "Should Unity generate mesh colliders for all meshes.");
        public GUIContent SwapUVChannels = new GUIContent("Swap UVs", "Swaps the 2 UV channels in meshes. Use if your diffuse texture uses UVs from the lightmap.");
        public GUIContent TipsForGenerate2UV = new GUIContent("是否启用统一修改","使用统一修改以后，所有的2uv值将一致！");
        public GUIContent GenerateSecondaryUV = new GUIContent("Generate Lightmap UVs", "Generate lightmap UVs into UV2.");
        public GUIContent GenerateSecondaryUVAdvanced = new GUIContent("Advanced");
        public GUIContent secondaryUVAngleDistortion = new GUIContent("Angle Error", "Measured in percents. Angle error measures deviation of UV angles from geometry angles. Area error measures deviation of UV triangles area from geometry triangles if they were uniformly scaled.");
        public GUIContent secondaryUVAreaDistortion = new GUIContent("Area Error");
        public GUIContent secondaryUVHardAngle = new GUIContent("Hard Angle", "Angle between neighbor triangles that will generate seam.");
        public GUIContent secondaryUVPackMargin = new GUIContent("Pack Margin", "Measured in pixels, assuming mesh will cover an entire 1024x1024 lightmap.");
        public GUIContent secondaryUVDefaults = new GUIContent("Set Defaults");

        public GUIContent TangentSpace = new GUIContent("Normals & Tangents");
        public GUIContent TangentSpaceNormalLabel = new GUIContent("Normals");
        public GUIContent TangentSpaceTangentLabel = new GUIContent("Tangents");

        public GUIContent TangentSpaceOptionImport = new GUIContent("Import");
        public GUIContent TangentSpaceOptionCalculateLegacy = new GUIContent("Calculate Legacy");
        public GUIContent TangentSpaceOptionCalculateLegacySplit = new GUIContent("Calculate Legacy - Split Tangents");
        public GUIContent TangentSpaceOptionCalculate = new GUIContent("Calculate Tangent Space");
        public GUIContent TangentSpaceOptionNone = new GUIContent("None");
        public GUIContent TangentSpaceOptionNoneNoNormals = new GUIContent("None - (Normals required)");

        public GUIContent NormalOptionImport = new GUIContent("Import");
        public GUIContent NormalOptionCalculate = new GUIContent("Calculate");
        public GUIContent NormalOptionNone = new GUIContent("None");

        public GUIContent RecalculateNormalsLabel = new GUIContent("Normals Mode");
        public GUIContent[] RecalculateNormalsOpt =
            {
                new GUIContent("Unweighted Legacy"),
                new GUIContent("Unweighted"),
                new GUIContent("Area Weighted"),
                new GUIContent("Angle Weighted"),
                new GUIContent("Area and Angle Weighted")
            };

        public GUIContent[] TangentSpaceModeOptLabelsAll;
        public GUIContent[] TangentSpaceModeOptLabelsCalculate;
        public GUIContent[] TangentSpaceModeOptLabelsNone;

        public GUIContent[] NormalModeLabelsAll;


        public ModelImporterTangents[] TangentSpaceModeOptEnumsAll;
        public ModelImporterTangents[] TangentSpaceModeOptEnumsCalculate;
        public ModelImporterTangents[] TangentSpaceModeOptEnumsNone;

        public GUIContent SmoothingAngle = new GUIContent("Smoothing Angle", "Normal Smoothing Angle");

        public GUIContent MeshCompressionLabel = new GUIContent("Mesh Compression", "Higher compression ratio means lower mesh precision. If enabled, the mesh bounds and a lower bit depth per component are used to compress the mesh data.");
        public GUIContent[] MeshCompressionOpt =
            {
                new GUIContent("Off"),
                new GUIContent("Low"),
                new GUIContent("Medium"),
                new GUIContent("High")
            };

        public GUIContent IndexFormatLabel = new GUIContent("Index Format", "Format of mesh index buffer. Auto mode picks 16 or 32 bit depending on mesh vertex count.");
        public GUIContent[] IndexFormatOpt =
            {
                new GUIContent("Auto"),
                new GUIContent("16 bit"),
                new GUIContent("32 bit")
            };

        public GUIContent OptimizeMeshForGPU = new GUIContent("Optimize Mesh", "The vertices and indices will be reordered for better GPU performance.");
        public GUIContent KeepQuads = new GUIContent("Keep Quads", "If model contains quad faces, they are kept for DX11 tessellation.");
        public GUIContent WeldVertices = new GUIContent("Weld Vertices", "Combine vertices that share the same position in space.");
        public GUIContent ImportVisibility = new GUIContent("Import Visibility", "Use visibility properties to enable or disable MeshRenderer components.");
        public GUIContent ImportCameras = new GUIContent("Import Cameras");
        public GUIContent ImportLights = new GUIContent("Import Lights");
        public GUIContent PreserveHierarchy = new GUIContent("Preserve Hierarchy", "Always create an explicit prefab root, even if the model only has a single root.");
        public GUIContent IsReadable = new GUIContent("Read/Write Enabled", "Allow vertices and indices to be accessed from script.");

        public Styles()
        {
            NormalModeLabelsAll = new GUIContent[] { NormalOptionImport, NormalOptionCalculate, NormalOptionNone };

            TangentSpaceModeOptLabelsAll = new GUIContent[] { TangentSpaceOptionImport, TangentSpaceOptionCalculate, TangentSpaceOptionCalculateLegacy, TangentSpaceOptionCalculateLegacySplit, TangentSpaceOptionNone };
            TangentSpaceModeOptLabelsCalculate = new GUIContent[] { TangentSpaceOptionCalculate, TangentSpaceOptionCalculateLegacy, TangentSpaceOptionCalculateLegacySplit, TangentSpaceOptionNone };
            TangentSpaceModeOptLabelsNone = new GUIContent[] { TangentSpaceOptionNoneNoNormals };

            TangentSpaceModeOptEnumsAll = new ModelImporterTangents[] { ModelImporterTangents.Import, ModelImporterTangents.CalculateMikk, ModelImporterTangents.CalculateLegacy, ModelImporterTangents.CalculateLegacyWithSplitTangents, ModelImporterTangents.None };
            TangentSpaceModeOptEnumsCalculate = new ModelImporterTangents[] { ModelImporterTangents.CalculateMikk, ModelImporterTangents.CalculateLegacy, ModelImporterTangents.CalculateLegacyWithSplitTangents, ModelImporterTangents.None };
            TangentSpaceModeOptEnumsNone = new ModelImporterTangents[] { ModelImporterTangents.None };
        }
    }
}
