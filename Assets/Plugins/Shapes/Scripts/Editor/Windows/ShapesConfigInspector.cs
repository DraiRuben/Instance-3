using Shapes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapesConfig))]
public class ShapesConfigInspector : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Settings"))
            MenuItems.OpenCsharpSettings();
        using (new EditorGUI.DisabledScope(true))
            DrawDefaultInspector();
    }
}