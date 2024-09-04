using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioConfiguration))]
public class AudioConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AudioConfiguration audioManager = (AudioConfiguration)target;

        int soundCount = ((Sound[])Enum.GetValues(typeof(Sound))).Length;
        serializedObject.Update();

        EditorGUILayout.LabelField("Sounds");
        int index = 0;
        foreach (Sound sound in (Sound[])Enum.GetValues(typeof(Sound)))
        {
            var array = audioManager.GetSoundArray(index);
            var property = serializedObject.FindProperty($"_clipsArray_{index}");
            EditorGUILayout.PropertyField(property, new GUIContent(sound.ToString() + " Clips"), true);
            index++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
