#if UNITY_EDITOR
#define UNITY_EDITOR
#endif

using UnityEngine;
using UnityEditor;
using DSL;

#if UNITY_EDITOR
[CustomEditor(typeof(InputManager))]
public class DSLInputManager : EditorWindow
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

    //Input Manager
    InputManager target;

    public static DSLInputManager Instance;
    [MenuItem("DSL/DSLInputManager")]
    public static void Initiate()
    {
        if (Instance == null)
        {
            Instance = GetWindow<DSLInputManager>("Dialogue System Language");
            Instance.Show();
            
        }
    }

    private void OnGUI()
    {
        // "target" can be any class derrided from ScriptableObject
        // (could be EditorWindo, monoBehaviour, etc)
        target = new InputManager();
        SerializedObject inputObj = new SerializedObject(target);
        SerializedProperty inputProperty = inputObj.FindProperty("Keys");

        EditorGUILayout.PropertyField(inputProperty, true); // True means show children
        inputObj.ApplyModifiedProperties(); //Remember to apply modified properties
    }
}
#endif