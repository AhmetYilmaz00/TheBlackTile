using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FreeRotateHandle))]
[CanEditMultipleObjects]
public class FreeRotateEditor : Editor
{
    public void OnSceneGUI()
    {
        FreeRotateHandle t = (target as FreeRotateHandle);

        EditorGUI.BeginChangeCheck();
        Quaternion rot = Handles.FreeRotateHandle(t.rot, Vector3.zero, t.size);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Free Rotate");
            t.rot = rot;
            t.Update();
        }
    }
}
