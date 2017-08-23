using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;


[CustomEditor(typeof(MeshAssetRule))]
public class MeshAssetRuleInspector : Editor 
{
    private class Styles
    {
        public GUIContent Meshes = new GUIContent("Meshes", "These options control how geometry is imported.");

        public GUIContent ScaleFactor = new GUIContent("Scale Factor", "How much to scale the models compared to what is in the source file.");

        public GUIContent UseFileUnits = new GUIContent("Use File Units", "Detect file units and import as 1FileUnit=1UnityUnit, otherwise it will import as 1cm=1UnityUnit. See ModelImporter.useFileUnits for more details.");

        public GUIContent FileScaleFactor = new GUIContent("File Scale", "暂时不需要打开"/*"Model scale defined in the source file. If available."*/);

        public GUIContent ImportBlendShapes = new GUIContent("Import BlendShapes", "Should Unity import BlendShapes.");

        public GUIContent GenerateColliders = new GUIContent("Generate Colliders", "Should Unity generate mesh colliders for all meshes.");

        public GUIContent SwapUVChannels = new GUIContent("Swap UVs", "Swaps the 2 UV channels in meshes. Use if your diffuse texture uses UVs from the lightmap.");

        public GUIContent GenerateSecondaryUV = new GUIContent("Generate Lightmap UVs", "Generate lightmap UVs into UV2.");

        public GUIContent GenerateSecondaryUVAdvanced = new GUIContent("Advanced");

        public GUIContent GenerateMeshes = new GUIContent("Meshes","Fold Mesh Settings!");

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


        public GUIContent[] TestNormalOption = new GUIContent[]
        {
            new GUIContent("Import"),
            new GUIContent("Calculate"),
            new GUIContent("None")
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

        public GUIContent[] MeshCompressionOpt = new GUIContent[]
			{
				new GUIContent("Off"),
				new GUIContent("Low"),
				new GUIContent("Medium"),
				new GUIContent("High")
			};

        public GUIContent OptimizeMeshForGPU = new GUIContent("Optimize Mesh", "The vertices and indices will be reordered for better GPU performance.");

        public GUIContent KeepQuads = new GUIContent("Keep Quads", "If model contains quad faces, they are kept for DX11 tessellation.");

        public GUIContent WeldVertices = new GUIContent("Weld Vertices", "Combine vertices that share the same position in space.");

        public GUIContent IsReadable = new GUIContent("Read/Write Enabled", "Allow vertices and indices to be accessed from script.");

        public GUIContent Materials = new GUIContent("Materials");

        public GUIContent ImportMaterials = new GUIContent("Import Materials");

        public GUIContent MaterialName = new GUIContent("Material Naming");

        public GUIContent MaterialNameTex = new GUIContent("By Base Texture Name");

        public GUIContent MaterialNameMat = new GUIContent("From Model's Material");

        public GUIContent[] MaterialNameOptMain = new GUIContent[]
			{
				new GUIContent("By Base Texture Name"),
				new GUIContent("From Model's Material"),
				new GUIContent("Model Name + Model's Material")
			};

        public GUIContent[] MaterialNameOptAll = new GUIContent[]
			{
				new GUIContent("By Base Texture Name"),
				new GUIContent("From Model's Material"),
				new GUIContent("Model Name + Model's Material"),
				new GUIContent("Texture Name or Model Name + Model's Material (Obsolete)")
			};

        public GUIContent MaterialSearch = new GUIContent("Material Search");

        public GUIContent[] MaterialSearchOpt = new GUIContent[]
			{
				new GUIContent("Local Materials Folder"),
				new GUIContent("Recursive-Up"),
				new GUIContent("Project-Wide")
			};

        public GUIContent MaterialHelpStart = new GUIContent("For each imported material, Unity first looks for an existing material named %MAT%.");

        public GUIContent MaterialHelpEnd = new GUIContent("If it doesn't exist, a new one is created in the local Materials folder.");

        public GUIContent MaterialHelpDefault = new GUIContent("No new materials are generated. Unity's Default-Diffuse material is used instead.");

        public GUIContent[] MaterialNameHelp = new GUIContent[]
			{
				new GUIContent("[BaseTextureName]"),
				new GUIContent("[MaterialName]"),
				new GUIContent("[ModelFileName]-[MaterialName]"),
				new GUIContent("[BaseTextureName] or [ModelFileName]-[MaterialName] if no base texture can be found")
			};

        public GUIContent[] MaterialSearchHelp = new GUIContent[]
			{
				new GUIContent("Unity will look for it in the local Materials folder."),
				new GUIContent("Unity will do a recursive-up search for it in all Materials folders up to the Assets folder."),
				new GUIContent("Unity will search for it anywhere inside the Assets folder.")
			};

        public GUIContent GenerateRigs = new GUIContent("Rigs","Fold Out Rigs!");

        public Styles()
        {
            this.NormalModeLabelsAll = new GUIContent[]
				{
					this.NormalOptionImport,
					this.NormalOptionCalculate,
					this.NormalOptionNone
				};
            this.TangentSpaceModeOptLabelsAll = new GUIContent[]
				{
					this.TangentSpaceOptionImport,
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionCalculateLegacy,
					this.TangentSpaceOptionCalculateLegacySplit,
					this.TangentSpaceOptionNone
				};
            this.TangentSpaceModeOptLabelsCalculate = new GUIContent[]
				{
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionCalculateLegacy,
					this.TangentSpaceOptionCalculateLegacySplit,
					this.TangentSpaceOptionNone
				};
            this.TangentSpaceModeOptLabelsNone = new GUIContent[]
				{
					this.TangentSpaceOptionNoneNoNormals
				};
            this.TangentSpaceModeOptEnumsAll = new ModelImporterTangents[]
				{
					ModelImporterTangents.Import,
					ModelImporterTangents.CalculateMikk,
					ModelImporterTangents.CalculateLegacy,
					ModelImporterTangents.CalculateLegacyWithSplitTangents,
					ModelImporterTangents.None
				};
            this.TangentSpaceModeOptEnumsCalculate = new ModelImporterTangents[]
				{
					ModelImporterTangents.CalculateMikk,
					ModelImporterTangents.CalculateLegacy,
					ModelImporterTangents.CalculateLegacyWithSplitTangents,
					ModelImporterTangents.None
				};
            this.TangentSpaceModeOptEnumsNone = new ModelImporterTangents[]
				{
					ModelImporterTangents.None
				};
        }


        public GUIContent AnimationType = new GUIContent("Animation Type|The type of animation to support / import.");

        public GUIContent[] AnimationTypeOpt = new GUIContent[]
			{
				new GUIContent("None|No animation present."),
				new GUIContent("Legacy|Legacy animation system."),
				new GUIContent("Generic|Generic Mecanim animation."),
				new GUIContent("Humanoid|Humanoid Mecanim animation system.")
			};

        public GUIContent AnimLabel = new GUIContent("Generation|Controls how animations are imported.");

        public GUIContent[] AnimationsOpt = new GUIContent[]
			{
				new GUIContent("Don't Import|No animation or skinning is imported."),
				new GUIContent("Store in Original Roots (Deprecated)|Animations are stored in the root objects of your animation package (these might be different from the root objects in Unity)."),
				new GUIContent("Store in Nodes (Deprecated)|Animations are stored together with the objects they animate. Use this when you have a complex animation setup and want full scripting control."),
				new GUIContent("Store in Root (Deprecated)|Animations are stored in the scene's transform root objects. Use this when animating anything that has a hierarchy."),
				new GUIContent("Store in Root (New)")
			};

        public GUIStyle helpText = new GUIStyle(EditorStyles.helpBox);

        public GUIContent avatar = new GUIContent("Animator");

        public GUIContent configureAvatar = new GUIContent("Configure...");

        public GUIContent avatarValid = new GUIContent("✓");

        public GUIContent avatarInvalid = new GUIContent("✕");

        public GUIContent avatarPending = new GUIContent("...");

        public GUIContent UpdateMuscleDefinitionFromSource = new GUIContent("Update|Update the copy of the muscle definition from the source.");

        public GUIContent RootNode = new GUIContent("Root node|Specify the root node used to extract the animation translation.");

        public GUIContent AvatarDefinition = new GUIContent("Avatar Definition|Choose between Create From This Model or Copy From Other Avatar. The first one create an Avatar for this file and the second one use an Avatar from another file to import animation.");

        public GUIContent[] AvatarDefinitionOpt = new GUIContent[]
			{
				new GUIContent("Create From This Model|Create an Avatar based on the model from this file."),
				new GUIContent("Copy From Other Avatar|Copy an Avatar from another file to import muscle clip. No avatar will be created.")
			};

        public GUIContent UpdateReferenceClips = new GUIContent("Update reference clips|Click on this button to update all the @convention file referencing this file. Should set all these files to Copy From Other Avatar, set the source Avatar to this one and reimport all these files.");

        //public Styles()
        //{
        //    this.helpText.normal.background = null;
        //    this.helpText.alignment = TextAnchor.MiddleLeft;
        //    this.helpText.padding = new RectOffset(0, 0, 0, 0);
        //}
        public GUIContent GenerateAnimation  = new GUIContent("Animations");

        public GUIContent ImportAnimation = new GUIContent("Import Animation");
    }

    private static bool changed = false;

    private bool m_SecondaryUVAdvancedOptions = false;

    private bool m_MeshFold = false;

    private bool m_ShowAllMaterialNameOptions = true;

    private static Styles styles;

    private MeshAssetRule orig;
    private bool m_RigFold;
    private bool m_AnimationsFold;

    [MenuItem("Assets/Manager/Create/Create Mesh Import Rules")]
    public static void CreateAssetRule()
    {
        MeshAssetRule newRule = CreateInstance<MeshAssetRule>();
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
        MeshAssetRule t = (MeshAssetRule) target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("MeshImportRules");
        EditorGUILayout.EndHorizontal();

        DrawMeshSettings(t);

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

    private void Apply(MeshAssetRule assetRule)
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

    /// <summary>
    /// inspector panel
    /// </summary>
    /// <param name="assetRule"></param>
    private void DrawMeshSettings(MeshAssetRule assetRule)
    {

        //GUILayout.Label(styles.Meshes, EditorStyles.boldLabel, new GUILayoutOption[0]);


        m_MeshFold = EditorGUILayout.Foldout(this.m_MeshFold,
                styles.GenerateMeshes, EditorStyles.foldout);

        if (m_MeshFold)
        {
            CommomUI(assetRule);
            NormalsAndTangentsGUI(assetRule);
            MaterialsGUI(assetRule);
        }

        m_RigFold = EditorGUILayout.Foldout(this.m_RigFold,
                styles.GenerateRigs, EditorStyles.foldout);

        if (m_RigFold)
        {
            assetRule.AnimationType = (ModelImporterAnimationType)
                EditorGUILayout.EnumPopup(styles.AnimationType, assetRule.AnimationType);

            if (assetRule.AnimationType == ModelImporterAnimationType.Generic ||
                assetRule.AnimationType == ModelImporterAnimationType.Human)
            {
                assetRule.isOptimizeObject = true;
            }
            else
            {
                assetRule.isOptimizeObject = false;
            }
        }

        m_AnimationsFold = EditorGUILayout.Foldout(this.m_AnimationsFold,
               styles.GenerateAnimation, EditorStyles.foldout);

        if (m_AnimationsFold)
        {
            assetRule.ImportAnimation = EditorGUILayout.Toggle(styles.ImportAnimation, assetRule.ImportAnimation);
        }

        //CommomUI(assetRule);
        //NormalsAndTangentsGUI(assetRule);
        //MaterialsGUI(assetRule);


    }

    private void CommomUI(MeshAssetRule assetRule)
    {
        //using (new EditorGUI.DisabledScope(base.targets.Length > 1))
        //{
        //    assetRule.m_GlobalScale = EditorGUILayout.FloatField(styles.ScaleFactor, assetRule.m_GlobalScale,
        //        new GUILayoutOption[0]);
        //}

        //using (new EditorGUI.DisabledScope(true))
        //{
        //    assetRule.m_FileScale = EditorGUILayout.FloatField(styles.FileScaleFactor, assetRule.m_FileScale);
        //}

        assetRule.m_MeshCompression =
            (ModelImporterMeshCompression)
                EditorGUILayout.EnumPopup(styles.MeshCompressionLabel, assetRule.m_MeshCompression);
        assetRule.m_IsReadable = EditorGUILayout.Toggle(styles.IsReadable, assetRule.m_IsReadable);
        assetRule.optimizeMeshForGPU = EditorGUILayout.Toggle(styles.OptimizeMeshForGPU, assetRule.optimizeMeshForGPU);
        assetRule.m_ImportBlendShapes = EditorGUILayout.Toggle(styles.ImportBlendShapes, assetRule.m_ImportBlendShapes);
        assetRule.m_AddColliders = EditorGUILayout.Toggle(styles.MeshCompressionLabel, assetRule.m_AddColliders);

        using (new EditorGUI.DisabledScope(true))
        {
            assetRule.keepQuads = EditorGUILayout.Toggle(styles.KeepQuads, assetRule.keepQuads);
        }
        assetRule.m_weldVertices = EditorGUILayout.Toggle(styles.WeldVertices, assetRule.m_weldVertices);
        assetRule.swapUVChannels = EditorGUILayout.Toggle(styles.SwapUVChannels, assetRule.swapUVChannels);
        assetRule.generateSecondaryUV = EditorGUILayout.Toggle(styles.GenerateSecondaryUV, assetRule.generateSecondaryUV);
        if (assetRule.generateSecondaryUV)
        {
            //EditorGUI.indentLevel++;
            //this.m_SecondaryUVAdvancedOptions = EditorGUILayout.Foldout(this.m_SecondaryUVAdvancedOptions,
            //    styles.GenerateSecondaryUVAdvanced, EditorStyles.foldout);
            //if (this.m_SecondaryUVAdvancedOptions)
            //{
            //    assetRule.secondaryUVHardAngle = EditorGUILayout.Slider(styles.secondaryUVHardAngle,
            //        assetRule.secondaryUVHardAngle, 0f, 180f, new GUILayoutOption[0]);
            //    assetRule.secondaryUVPackMargin = EditorGUILayout.Slider(styles.secondaryUVPackMargin,
            //        assetRule.secondaryUVPackMargin, 0f, 180f, new GUILayoutOption[0]);
            //    assetRule.secondaryUVAngleDistortion = EditorGUILayout.Slider(styles.secondaryUVAngleDistortion,
            //        assetRule.secondaryUVAngleDistortion, 0f, 180f, new GUILayoutOption[0]);
            //    assetRule.secondaryUVAreaDistortion = EditorGUILayout.Slider(styles.secondaryUVAreaDistortion,
            //        assetRule.secondaryUVAreaDistortion, 0f, 180f, new GUILayoutOption[0]);
            //}
            //EditorGUI.indentLevel--;
        }
    }

    private void NormalsAndTangentsGUI(MeshAssetRule rule)
    {
        GUILayout.Label(styles.TangentSpace, EditorStyles.boldLabel, new GUILayoutOption[0]);
        rule.normalImportMode = (ModelImporterNormals)EditorGUILayout.EnumPopup(styles.TangentSpaceNormalLabel, rule.normalImportMode);
        using (new EditorGUI.DisabledScope(rule.normalImportMode != ModelImporterNormals.Calculate))
        {
            rule.normalSmoothAngle = EditorGUILayout.Slider(styles.SmoothingAngle, rule.normalSmoothAngle, 0f, 180f);
        }
        GUIContent[] displayedOptions = styles.TangentSpaceModeOptLabelsAll;
        ModelImporterTangents[] array = styles.TangentSpaceModeOptEnumsAll;
        if (rule.normalImportMode == ModelImporterNormals.Calculate)
        {
            displayedOptions = styles.TangentSpaceModeOptLabelsCalculate;
            array = styles.TangentSpaceModeOptEnumsCalculate;
        }
        else if (rule.normalImportMode == ModelImporterNormals.None)
        {
            displayedOptions = styles.TangentSpaceModeOptLabelsNone;
            array = styles.TangentSpaceModeOptEnumsNone;
        }
        using (new EditorGUI.DisabledScope(rule.normalImportMode == ModelImporterNormals.None))
        {
            int num = Array.IndexOf(array, rule.tangentImportMode);
            EditorGUI.BeginChangeCheck();
            num = EditorGUILayout.Popup(styles.TangentSpaceTangentLabel, num, displayedOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                rule.tangentImportMode = array[num];
            }
        }
    }

    private void MaterialsGUI(MeshAssetRule rule)
    {
        GUILayout.Label(styles.Materials, EditorStyles.boldLabel, new GUILayoutOption[0]);
        rule.m_ImportMaterials = EditorGUILayout.Toggle(styles.ImportMaterials, rule.m_ImportMaterials);
        string text;
        if (rule.m_ImportMaterials)
        {
            rule.m_MaterialName = (ModelImporterMaterialName)EditorGUILayout.EnumPopup(styles.MaterialName, rule.m_MaterialName);
            rule.m_MaterialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup(styles.MaterialSearch, rule.m_MaterialSearch);
            text = string.Concat(styles.MaterialHelpStart.text.Replace("%MAT%", styles.MaterialNameHelp[(int)rule.m_MaterialName].text), "\n", styles.MaterialSearchHelp[(int)rule.m_MaterialSearch].text, "\n", styles.MaterialHelpEnd.text);
        }
        else
        {
            text = styles.MaterialHelpDefault.text;
        }
        GUILayout.Label(new GUIContent(text), EditorStyles.helpBox, new GUILayoutOption[0]);
    }

    void OnEnable()
    {

        if (styles == null)
        {
            styles = new Styles();
        }

        changed = false;
        orig = (MeshAssetRule)target;

        Undo.RecordObject(target, "assetruleundo");
    }
}
