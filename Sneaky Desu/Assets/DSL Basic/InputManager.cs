using System;
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
        PAUSE,
        RESUME,
        SHOW_LOG,
        MAIN_MENU,
        NAVIGATE_LEFT,
        NAVIGATE_RIGHT,
        NAVIGATE_UP,
        NAVIGATE_DOWN,
        PRESET_0,
        PRESET_1,
        PRESET_2,
        PRESET_3,
        PRESET_4,
        PRESET_5,
        PRESET_6,
        PRESET_7,
        PRESET_8,
        PRESET_9,
        PRESET_10,
        PRESET_11,
        PRESET_12,
        PRESET_13,
        PRESET_14,
        PRESET_15,
        PRESET_16,
        PRESET_17,
        PRESET_18,
        PRESET_19,
        PRESET_20,
        PRESET_21,
        PRESET_22,
        PRESET_23,
        PRESET_24,
        PRESET_25,
        PRESET_26,
        PRESET_27,
        PRESET_28,
        PRESET_29,
        PRESET_30,
        PRESET_31,
        PRESET_32
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
            descriptiveName = _decriptiveName;
            functionFor = _useFor;
        }
    }

    public class CantRegister: Exception
    {
        public CantRegister() { }
        public CantRegister(string message) : base(message) { }
        public CantRegister(string message, Exception inner) : base(message, inner) { }
    }

    public class InputManager : MonoBehaviour
    {
        static InputManager Instance;
        public static List<Input> Keys { get; private set; }
        public Input[] keys;

        private void Awake()
        {
            Instance = this;
            Keys = new List<Input>();
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
        public static bool GetButtonDown(Functionality _functionality)
            => UnityEngine.Input.GetKeyDown(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// If the selected key is pressed and held down.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButton(string _buttonName)
            => UnityEngine.Input.GetKey(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is pressed and held down based on functionality role.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButton(Functionality _functionality)
            => UnityEngine.Input.GetKey(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// If the selected key is not pressed.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonUp(string _buttonName)
            => UnityEngine.Input.GetKeyUp(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is not pressed based on functionality role.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButtonUp(Functionality _functionality)
            => UnityEngine.Input.GetKeyUp(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// Search for a key code based on it's name using DSL's Input Manager.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        static KeyCode GetKeyCodeByButtonName(string _buttonName)
        {
            foreach (Input key in Instance.keys)
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
            foreach (Input key in Instance.keys)
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
            try
            {
                Keys.Add(new Input(_name, Input.NO_DESCRIPTIVE_NAME, _newKeyCodeEntry, Functionality.NONE));
            }catch(CantRegister e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Register KeyCodes with a set name, and descriptive name.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_descriptiveName"></param>
        /// <param name="_newKeyCodeEntry"></param>
        public static void Register(string _name, string _descriptiveName, KeyCode _newKeyCodeEntry)
        {
            try{
                Keys.Add(new Input(_name, _descriptiveName, _newKeyCodeEntry, Functionality.NONE));
            } catch(CantRegister e)
            {
                Debug.LogError(e.Message);
            }
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
            try
            {
                Keys.Add(new Input(_name, _descriptiveName, _newKeyCodeEntry, functionality));
            } catch(CantRegister e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Officially added the KeyCodes and Inputs into the InputManager
        /// </summary>
        public static void FinalizeKeyCodes()
        {
            int size = Keys.Count;

            Instance.keys = new Input[size];

            for (int index = 0; index < Keys.Count; index++)
                Instance.keys[index] = Keys[index];
        }
    }
}
