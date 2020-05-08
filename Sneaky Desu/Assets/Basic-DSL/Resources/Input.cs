using System;
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

    [Serializable]
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
}
