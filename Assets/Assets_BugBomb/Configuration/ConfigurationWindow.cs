using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class ConfigurationWindow : EditorWindow
{
    private int _selectedConfiguration;
    private Vector2 _scrollPosition;

    [MenuItem("Window/Configuration")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConfigurationWindow window = (ConfigurationWindow)GetWindow(typeof(ConfigurationWindow));
        window.titleContent = new GUIContent("Configuration");
        window.Show();
    }

    private void OnEnable()
    {
        _selectedConfiguration = EditorPrefs.GetInt("confID", 0);
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt("confID", _selectedConfiguration);
    }

    void OnGUI()
    {
        GUILayout.Label("Select Configuration", EditorStyles.largeLabel);
        var configurations = LoadAll();
        var names = configurations.Select(c => GetTypeName(c)).ToArray();

        _selectedConfiguration = GUILayout.SelectionGrid(_selectedConfiguration, names, 5);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        var selectedObject = configurations[_selectedConfiguration];
        GUILayout.Label(GetTypeName(selectedObject) + " configuration", EditorStyles.largeLabel);

        if (selectedObject != null)
        {
            var editor = Editor.CreateEditor(selectedObject);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            editor.OnInspectorGUI();
            GUILayout.EndScrollView();
        }
    }

    private string GetTypeName(Configuration configurationType)
    {
        return configurationType.GetType().Name.Replace("Configuration", "");
    }

    private Configuration[] LoadAll()
    {
        return Resources.LoadAll<Configuration>(@"_Configuration");
    }
}
#endif
