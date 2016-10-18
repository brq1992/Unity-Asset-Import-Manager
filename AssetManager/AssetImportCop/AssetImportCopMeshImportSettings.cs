using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct MeshImporterSettings
{
    public bool readWriteEnabled;
    public bool optimiseMesh;
    public bool ImportBlendShapes;
    public ModelImporterNormals NormalsType;
    public ModelImporterTangents TangentsType;
    public ModelImporterMeshCompression MeshCompression;
    public bool GenerateColliders;
    public float GlobalScale; 
    public static MeshImporterSettings Extract(ModelImporter importer)
    {
        if (importer == null)
            throw new ArgumentException();

        MeshImporterSettings settings = new MeshImporterSettings();
        settings.readWriteEnabled = importer.isReadable;
        settings.optimiseMesh = importer.optimizeMesh;
        settings.ImportBlendShapes = importer.importBlendShapes;
        settings.NormalsType = importer.importNormals;
        settings.TangentsType = importer.importTangents;
        settings.MeshCompression = importer.meshCompression;
        settings.GenerateColliders = importer.addCollider;
        settings.GlobalScale = importer.globalScale;
        return settings;
    }

    public static bool Equal(MeshImporterSettings a, MeshImporterSettings b)
    {
        return (a.readWriteEnabled == b.readWriteEnabled) && (a.optimiseMesh == b.optimiseMesh) && ( a.ImportBlendShapes == b.ImportBlendShapes);
    }

    public void ApplyDefaults()
    {
        Debug.Log("mesh setting defaults");
        readWriteEnabled = false;
        optimiseMesh = true;
        ImportBlendShapes = false;
        NormalsType = ModelImporterNormals.Import;
        TangentsType = ModelImporterTangents.Import;
        MeshCompression = ModelImporterMeshCompression.Off;
        GenerateColliders = false;
        GlobalScale = 1;
    }
}