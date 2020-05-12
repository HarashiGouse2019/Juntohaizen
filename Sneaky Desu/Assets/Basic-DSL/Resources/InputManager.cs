using System;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

namespace DSL.InputManagement
{

    public class InputManager : MonoBehaviour
    {
        static InputManager Instance;

        public static List<Input> Keys = new List<Input>();

        [SerializeField]
        private List<Input> keys;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }

            keys = Keys;
        }

        /// <summary>
        /// If the selected key is pressed once bsed on name. This doesn't count for holding the key.
        /// </summary>
        /// <param name="_buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonDown(string _buttonName)
            => UnityEngine.Input.GetKeyDown(GetKeyCodeByButtonName(_buttonName));

        /// <summary>
        /// If the selected key is pressed once based on functionality role. This doesn't count for holding the key.
        /// </summary>
        /// <param name="_functionality"></param>
        /// <returns></returns>
        public static bool GetButtonDown(Functionality _functionality)
            => UnityEngine.Input.GetKeyDown(GetInputByFunctionality(_functionality).key);

        /// <summary>
        /// If the slected key is pressed once.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public static bool GetButtonDown(KeyCode _key)
            => UnityEngine.Input.GetKeyDown(_key);

        /// <summary>
        /// If the selected key is pressed and held down based on name. This doesn't count for holding the key.
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
        /// If the selected key is pressed and held down.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public static bool GetButton(KeyCode _key)
            => UnityEngine.Input.GetKey(_key);

        /// <summary>
        /// If the selected key is not pressed based on name.
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
        /// If the selected key is not pressed.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public static bool GetButtonUp(KeyCode _key)
            => UnityEngine.Input.GetKeyUp(_key);

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
            }
            catch (CantRegisterException e)
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
            try
            {
                Keys.Add(new Input(_name, _descriptiveName, _newKeyCodeEntry, Functionality.NONE));
            }
            catch (CantRegisterException e)
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
            Keys.Add(new Input(_name, _descriptiveName, _newKeyCodeEntry, functionality));
        }
    }
}
