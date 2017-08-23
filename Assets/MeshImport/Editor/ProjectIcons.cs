using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProjectIcons : Editor
{
    [MenuItem("EDITORS/ProjectIcons/Enable")]
    static void EnableIcons()
    {
        EditorApplication.projectWindowItemOnGUI -= ProjectIcons.MyCallback();
        EditorApplication.projectWindowItemOnGUI += ProjectIcons.MyCallback();
    }

    [MenuItem("EDITORS/ProjectIcons/Disable")]
    static void DisableIcons()
    {
        EditorApplication.projectWindowItemOnGUI -= ProjectIcons.MyCallback();
    }

    static EditorApplication.ProjectWindowItemCallback MyCallback()
    {
        EditorApplication.ProjectWindowItemCallback myCallback = new EditorApplication.ProjectWindowItemCallback(IconGUI);
        return myCallback;
    }

    static void IconGUI(string s, Rect r)
    {
        string fileName = AssetDatabase.GUIDToAssetPath(s);
        int index = fileName.LastIndexOf('.');
        if (index == -1) return;
        string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
        r.width = r.height;
        switch (fileType)
        {
            case "cs":
                //Put your icon images somewhere in the project, and refer to them with a string here
                GUI.DrawTexture(r, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/Icon1.psd", typeof(Texture2D)));
                break;
            case "psd":
                GUI.DrawTexture(r, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/Icon2.psd", typeof(Texture2D)));
                break;
            case "png":
                GUI.DrawTexture(r, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/Icon3.psd", typeof(Texture2D)));
                break;
        }
    }
}