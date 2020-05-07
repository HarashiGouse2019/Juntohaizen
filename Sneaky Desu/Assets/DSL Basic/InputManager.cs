using System.Collections.Generic;
using UnityEngine;

namespace DSL
{
    public enum Functionality
    {
        NONE,
        PROCEED,
        SAVE,
        LOAD,
        QUICK_SAVE,
        QUICK_LOAD,
        SHOW_LOG,
        CUSTOM
    }

    [System.Serializable]
    public struct Input
    {
        
        public string m_name;

        public string descriptiveName;

        public KeyCode key;

        public Functionality functionFor;

        public static readonly string NO_DESCRIPTIVE_NAME = "";

        public Input(string _name, string _decriptiveName, KeyCode _key, Functionality _useFor)
        {
            m_name = _name;
            key = _key;
            descriptiveName = "";
            functionFor = _useFor;
        }
    }
    
    public class InputManager : MonoBehaviour
    {
        static InputManager Instance;
        public static List<Input> Keys { get; private set; } = new List<Input>();
        public Input[] keys;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// If the selected key is pressed once. This doesn't count for holding the key.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonDown(string _buttonName) => UnityEngine.Input.GetKeyDown(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is pressed once based on functionality role. This doesn't count for holding the key.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButtonDown(Functionality _functionality) => UnityEngine.Input.GetKeyDown(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// If the selected key is pressed and held down.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButton(string _buttonName) => UnityEngine.Input.GetKey(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is pressed and held down based on functionality role.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButton(Functionality _functionality) => UnityEngine.Input.GetKey(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// If the selected key is not pressed.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonUp(string _buttonName) => UnityEngine.Input.GetKeyUp(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is not pressed based on functionality role.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButtonUp(Functionality _functionality) => UnityEngine.Input.GetKeyUp(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// Search for a key code based on it's name using DSL's Input Manager.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        static KeyCode GetKeyCodeByButtonName(string _buttonName)
        {
            foreach(Input key in Instance.keys)
            {
                if (_buttonName == key.m_name) return key.key; 
            }

            return KeyCode.None;
        }

        /// <summary>
        /// Get an Input passed on the functionality assigned.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        static Input GetInputByFunctionality(Functionality _functionality)
        {
            foreach(Input key in Instance.keys)
            {
                if (_functionality == key.functionFor) return key;
            }

            return default;
        }

        /// <summary>
        /// Register KeyCodes with a set name.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_newKeyCodeEntry"></param>
        public static void Register(string _name, KeyCode _newKeyCodeEntry)
        {
            Keys.Add(new Input(_name, Input.NO_DESCRIPTIVE_NAME, _newKeyCodeEntry, Functionality.NONE));
        }

        /// <summary>
        /// Register KeyCodes with a set name, and descriptive name.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_descriptiveName"></param>
        /// <param name="_newKeyCodeEntry"></param>
        public static void Register(string _name, string _descriptiveName, KeyCode _newKeyCodeEntry)
        {
            Keys.Add(new Input(_name, _descriptiveName, _newKeyCodeEntry, Functionality.NONE));
        }

        /// <summary>
        /// Regist KeyCodes with a set name, decriptive name, and can be binded to functions for basic dialogue system interaction.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_descriptiveName"></param>
        /// <param name="_newKeyCodeEntry"></param>
        /// <param name="functionality"></param>
        public static void Register(string _name, string _descriptiveName, KeyCode _newKeyCodeEntry, Functionality functionality)
        {
            Keys.Add(new Input(_name, Input.NO_DESCRIPTIVE_NAME, _newKeyCodeEntry, functionality));
        }

        /// <summary>
        /// Officially added the KeyCodes and Inputs into the InputManager
        /// </summary>
        public static void FinalizeKeyCodes()
        {
            int size = Keys.Count;
            Instance.keys = new Input[size];
            for(int index = 0; index < Keys.Count; index++)
            {

                Instance.keys[index] = Keys[index];
            }
        }
    }
}
