#if UNITY_EDITOR
#define UNITY_EDITOR
#endif

using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DSLEditor : EditorWindow
{
    //This is how all the words, numbers, tokens, delimiter, and editor background have their color
    public struct Styler
    {
        //Max Value
        public const float MAX_COLOR_VALUE = 255;

        public static Color EditorBackgroundColor { get; private set; } =
            new Color(255f / MAX_COLOR_VALUE, 255f / MAX_COLOR_VALUE, 255f / MAX_COLOR_VALUE, 255f / MAX_COLOR_VALUE);
    }

    //This variable is to read what's on the text area
    string text = "";

    //The directory to grab the file
    string fileDirectory = "";

    //A text asset
    TextAsset textAsset;

    //And setting up for scrolling
    Vector2 scroll;

    //The source of our Text Asset
    Object source = null;

    public static DSLEditor Instance;
    [MenuItem("DSL/DSLEditor")]
    public static void Initiate()
    {
        if (Instance == null)
        {
            Instance = GetWindow<DSLEditor>("Dialogue System Language");
            Instance.Show();
        }
    }

    private void OnGUI()
    {
        //Graph the sources of new text
        EditorGUILayout.BeginHorizontal();
        fileDirectory = EditorGUILayout.TextField("Insert a .dsl directory...", fileDirectory);
        GUI.Button(new Rect(50, 0, 50, 20), "Import");
        this.Repaint();
        EditorGUILayout.EndHorizontal();

        TextAsset newTextAsset = (TextAsset)source;

        if (newTextAsset != textAsset)
            ReadTextAsset(newTextAsset);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUI.backgroundColor = Styler.EditorBackgroundColor;
        text = EditorGUILayout.TextArea(text, GUILayout.Height(position.height - 30));
        this.Repaint();
        EditorGUILayout.EndScrollView();
    }

    void ReadTextAsset(TextAsset _text)
    {
        text = _text.text;
        textAsset = _text;
    }
}
#endif