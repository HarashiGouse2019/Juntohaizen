using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using PARSER = DSLParser.DialogueSystemParser;

public class DialogueSystemEvents
{
    //Interface for events
    public interface IExecuteOnEnd
    {
        void ExecuteOnEnd();
    }

    public interface IExecuteOnCommand
    {
        void ExecuteOnCommand();
    }

    public interface IExecuteOnStart
    {
        void ExecuteOnStart();
    }

    public interface IExecute
    {
        void Execute();
    }
}

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;


    //These are valid speeds that you can use in DSL
    public enum TextSpeedValue
    {
        SLOWEST,
        SLOWER,
        SLOW,
        NORMAL,
        FAST,
        FASTER,
        FASTEST
    }

    //Determine if you want it this to be a singleton
    [SerializeField] private bool setToSingleton = true;

    //The file name. 
    [SerializeField]
    private string dslFileName = "";

    //Reference to TextMeshPro
    [SerializeField]
    private TextMeshProUGUI TMP_DIALOGUETEXT = null;

    //If there's a frame or dialgoue box, reference it.
    [SerializeField]
    private Image textBoxUI = null;

    private static TextSpeedValue SpeedValue;

    public static float TextSpeed { get; private set; }

    //Dialogue that is collected after reading file.
    public static List<string> Dialogue = new List<string>();

    //If dialouge is currently running
    public static bool RunningDialogue { get; private set; } = false;

    //Used to iterate through list of dialogue
    public static uint LineIndex { get; private set; } = 0;

    //Used to iterate through the dialogue text
    public static uint CursorPosition { get; private set; } = 0;

    //If a [HALT] command has been called
    private static bool OnDelay = false;

    //Allow the player to proceed to next dialogue after full text is displayed
    private static bool typeIn;

    //Dialogue set defined in the DSL file.
    public static int DialogueSet { get; private set; } = -1;

    //Nodes to be called
    [SerializeField]
    private List<DialogueNode> nodes = new List<DialogueNode>();

    //Current node that is being used.
    public static int CurrentNode { get; private set; } = -1;

    //A list of sprite changers to allow the changing of images of a character.
    public static List<DialogueSystemSpriteChanger> DialogueSystemSpriteChangers { get; private set; } = new List<DialogueSystemSpriteChanger>();

    //Reset value
    const int reset = 0;

    //Predefined styling tags that already exists.
    static readonly string BOLD = "<b>", BOLDEND = "</b>";
    static readonly string ITALIZE = "<i>", ITALIZEEND = "</i>";
    static readonly string UNDERLINE = "<u>", UNDERLINEEND = "</u>";


    //The formatting of custom-made tags
    static readonly Regex ACTION = new Regex(@"(<)+\w*action=\w*[a-zA-Z ]+(>$)");
    static readonly Regex INSERT = new Regex(@"(<)+\w*ins=\w*[a-zA-Z!@#$%^&*()_\-=\\/<>?,./{}[\|: ]+(>$)");
    static readonly Regex EXPRESSION = new Regex(@"(<)+\w*exp=\w*[A-Z0-9_-]+(>$)");
    static readonly Regex POSE = new Regex(@"(<)+\w*pose=\w*[A-Z0-9_-]+(>$)");
    static readonly Regex HALT = new Regex(@"(<)+\w*halt=\w*[0-9]+(>$)");
    static readonly Regex SPEED = new Regex(@"(<)+\w*sp=\w*[0-6](>$)");

    //DSL file extension
    static readonly string dslFileExtention = ".dsl";

    static readonly string STRINGNULL = "";

    //SUCCESS CONSTANT
    const bool SUCCESSFUL = true;

    //FAIL CONSTANT
    const bool FAILURE = false;

    /// <summary>
    /// Execute once instantiated 
    /// </summary>
    void Awake()
    {
        //Check if we want this dialogue system to be a singleton or not
        if (setToSingleton && Instance == null)
            MakeIntoSingleton(out Instance);
        else
            Destroy(gameObject);

        //We what to define all values in the .dsl
        //That includes the characters, the scenes, the expressions, poses, sounds, music, etc.
        DefineValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Find all sprite changers in the current scene.
        DialogueSystemSpriteChangers = FIND_ALL_SPRITECHANGERS();
    }

    void FixedUpdate()
    {
        //If [HALT] command ends, continue to exclude any tags that may be parsered
        if (!OnDelay && Dialogue.Count != 0)
        {
            ExcludeAllFunctionTags(Dialogue[(int)LineIndex]);
            ExcludeAllStyleTags(Dialogue[(int)LineIndex]);
        }
    }

    /// <summary>
    /// Turns this class or object into a singleton object
    /// </summary>
    /// <param name="_object"></param>
    void MakeIntoSingleton(out DialogueSystem _object)
    {
        _object = this;
        DontDestroyOnLoad(_object);
    }

    /// <summary>
    /// Dialogue being displayed on to the dialogue box.
    /// </summary>
    /// <returns></returns>
    static IEnumerator PrintCycle()
    {
        while (true)
        {
            if (OnDelay)
                continue;

            //If we are not allowed to respone, have it run dialgoue until the full sentence is displayed onto the screen
            if (IS_TYPE_IN() == false)
            {
                ENABLE_DIALOGUE_BOX();

                GET_TMPGUI().text = STRINGNULL;

                //If this is a new sentence, we want to clear the current text that is displaying
                var text = STRINGNULL;

                //If we haven't reached the end of the dialogue set, get the next line avaliable
                if (LineIndex < Dialogue.Count) text = Dialogue[(int)LineIndex];

                //This is our "type writer" effect for our dialogue system.
                //We'll go through each character one.
                //If a halt command is detected at a certain position, CursorPosition will not increment.
                for (CursorPosition = 0; CursorPosition < text.Length + 1; CursorPosition += (uint)((OnDelay) ? 0 : 1))
                {
                    try
                    {
                        //If we head to a next line, we want to assure that we are at the next line
                        if (LineIndex < Dialogue.Count) text = Dialogue[(int)LineIndex];

                        //We'll update the TMP text
                        GET_TMPGUI().text = text.Substring(0, (int)CursorPosition);

                        //And check what speed we are displaying the text
                        UPDATE_TEXT_SPEED(SpeedValue);
                    }
                    catch
                    {

                        Debug.LogWarning("Cursor Position is at: " + CursorPosition + ", but text is " + text.Length + " long.");

                    }

                    //We wait at the speed of our text speed
                    yield return new WaitForSecondsRealtime(TextSpeed);
                }

                //At this point, the full sentence is displayed. The player is allowed to hit space to continue
                SET_TYPE_IN_VALUE(true);

                //Wait for a response from the player.
                Instance.StartCoroutine(WaitForResponse());
            }

            yield return null;
        }

    }

    /// <summary>
    /// Extract all style tag when we come across them.
    /// </summary>
    /// <param name="_text"></param>
    static void ExcludeAllStyleTags(string _text)
    {
        //Bold
        ExcludeStyleTag(BOLD, BOLDEND, ref _text);

        //Italize tag
        ExcludeStyleTag(ITALIZE, ITALIZEEND, ref _text);

        //Underline tag
        ExcludeStyleTag(UNDERLINE, UNDERLINEEND, ref _text);
    }

    /// <summary>
    /// Extract all function tags that's been parsed from the commands defined in the .dsl when we come across them
    /// </summary>
    /// <param name="_text"></param>
    static void ExcludeAllFunctionTags(string _text)
    {

        PARSER.PARSER_LINE(Dialogue[(int)LineIndex]);

        //Action tag!
        ExecuteActionFunctionTag(ACTION, ref _text);

        //Insert tag!
        ExecuteInsertFunctionTag(INSERT, ref _text);

        //Speed Command Tag: It will consider all of the possible values.
        ExecuteSpeedFunctionTag(SPEED, ref _text);

        //Expression tag!
        ExecuteExpressionFunctionTag(EXPRESSION, ref _text);

        //Pose tag!
        ExecutePoseFunctionTag(POSE, ref _text);

        //Halt tage
        ExecuteHaltFunctionTag(HALT, ref _text);
    }

    /// <summary>
    /// Remove a style tag when dialogue is displayed.
    /// </summary>
    /// <param name="_openTag"></param>
    /// <param name="_closeTag"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExcludeStyleTag(string _openTag, string _closeTag, ref string _line)
    {
        try
        {
            if (_line.Substring((int)CursorPosition, _openTag.Length).Contains(_openTag))
            {
                ShiftCursorPosition(_openTag.Length - 1);
                return SUCCESSFUL;
            }
        }
        catch { }

        try
        {
            if (_line.Substring((int)CursorPosition, _closeTag.Length).Contains(_closeTag))
            {
                ShiftCursorPosition(_closeTag.Length - 1);
                return SUCCESSFUL;
            }
        }
        catch { }

        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for changing text speed, we'll change the speed value
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecuteSpeedFunctionTag(Regex _tagExpression, ref string _line)
    {
        try
        {
            string tag = _line.Substring((int)CursorPosition, "<sp=".Length);

            if (tag.Contains("<sp="))
            {
                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {

                        endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {

                            //<sp=3>
                            int speed = Convert.ToInt32(tag.Split(PARSER.Delimiters)[1].Split('=')[1]);

                            SpeedValue = (TextSpeedValue)speed;

                            _line = ReplaceFirst(_line, tag, "");

                            Dialogue[(int)LineIndex] = _line;

                            ShiftCursorPosition(-1);

                            return SUCCESSFUL;

                        }
                    }
                }
            }
        }
        catch { }

        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for displaying an action string, we'll add an asterisks around the action word
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecuteActionFunctionTag(Regex _tagExpression, ref string _line)
    {
        try
        {
            string tag = _line.Substring((int)CursorPosition, "<action=".Length);

            if (tag.Contains("<action="))
            {
                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {

                        endTagPos = (int)(Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {
                            Debug.Log("WOW FANTASTIC BABY ");
                            if (OnDelay == false)
                            {
                                string value = "*" + tag.Split(PARSER.Delimiters)[1].Split('=')[1] + "*";

                                _line = ReplaceFirst(_line, tag, value);

                                ShiftCursorPosition(value.Length);

                                Dialogue[(int)LineIndex] = _line;
                            }
                            return SUCCESSFUL;
                        }
                    }
                }


            }
        }
        catch { }

        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for inserting a word, we'll place in the word as it is, omitting the type-writer effect
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecuteInsertFunctionTag(Regex _tagExpression, ref string _line)
    {
        try
        {
            string tag = _line.Substring((int)CursorPosition, "<ins=".Length);

            if (tag.Contains("<ins="))
            {
                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {

                        endTagPos = (int)(Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {
                            if (OnDelay == false)
                            {
                                string value = tag.Split(PARSER.Delimiters)[1].Split('=')[1];

                                Debug.Log(value);

                                _line = ReplaceFirst(_line, tag, value);

                                ShiftCursorPosition(value.Length);

                                Dialogue[(int)LineIndex] = _line;
                            }
                            return SUCCESSFUL;
                        }
                    }
                }


            }
        }
        catch { }

        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for halting, we'll wait for a certain amount of milliseconds
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecuteHaltFunctionTag(Regex _tagExpression, ref string _line)
    {
        /*The wait command will take a 4 digit number.
         We will then convert this into a value that is easily understood
         by textSpeed. We'll be mainly affecting the textSpeed to create our
         WAIT function... unless...*/
        try
        {
            string tag = _line.Substring((int)CursorPosition, "<halt=".Length);
            if (tag.Contains("<halt="))
            {

                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);

                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {
                        endTagPos = (int)(Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {
                            if (OnDelay == false)
                            {
                                /*Now we do a substring from the current position to 4 digits.*/

                                int value = Convert.ToInt32(tag.Split(PARSER.Delimiters)[1].Split('=')[1]);

                                int millsecs = Convert.ToInt32(value);

                                Instance.StartCoroutine(DelaySpan(millsecs));

                                _line = ReplaceFirst(_line, tag, "");

                                Dialogue[(int)LineIndex] = _line;

                                return SUCCESSFUL;

                            }
                        }

                    }
                }
            }
        }
        catch { }

        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for changing expression, we'll access the SpriteChanger that handles expressions, and change the value.
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecuteExpressionFunctionTag(Regex _tagExpression, ref string _line)
    {
        try
        {
            bool notFlaged = true;

            string tag = _line.Substring((int)CursorPosition, "<exp=".Length);
            if (tag.Contains("<exp="))
            {
                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {
                        endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {
                            /*The system will now take this information, from 0 to the current position
                             and split it down even further, taking all the information.*/

                            string value = tag.Split('<')[1].Split('=')[1].Split('>')[0];

                            _line = ReplaceFirst(_line, tag, "");

                            Dialogue[(int)LineIndex] = _line;

                            return ValidateExpressionsAndChange(value, ref notFlaged);
                        }
                    }
                }
            }
        }
        catch { }
        return FAILURE;
    }

    /// <summary>
    /// If we come across the tag for changing poses, we'll access the SpriteChanger that handles poses, and change the value.
    /// </summary>
    /// <param name="_tagExpression"></param>
    /// <param name="_line"></param>
    /// <returns></returns>
    static bool ExecutePoseFunctionTag(Regex _tagExpression, ref string _line)
    {
        try
        {
            bool notFlaged = true;

            string tag = _line.Substring((int)CursorPosition, "<pose=".Length);
            if (tag.Contains("<pose="))
            {
                int startTagPos = (int)CursorPosition;
                int endTagPos = 0;
                string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                foreach (char letter in stringRange)
                {
                    if (letter == '>')
                    {
                        endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                        tag = Dialogue[(int)LineIndex].Substring(startTagPos, endTagPos + 1);

                        if (_tagExpression.IsMatch(tag))
                        {
                            /*The system will now take this information, from 0 to the current position
                             and split it down even further, taking all the information.*/

                            string value = tag.Split('<')[1].Split('=')[1].Split('>')[0];

                            _line = ReplaceFirst(_line, tag, "");

                            Dialogue[(int)LineIndex] = _line;

                            return ValidatePosesAndChange(value, ref notFlaged);
                        }
                    }
                }
            }
        }
        catch { }
        return FAILURE;
    }

    /// <summary>
    /// Validate if the Expression is defined in the .dsl file
    /// </summary>
    /// <param name="value"></param>
    /// <param name="_notFlag"></param>
    /// <returns></returns>
    static bool ValidateExpressionsAndChange(string value, ref bool _notFlag)
    {
        //Check if a key matches
        string data = STRINGNULL;

        if (PARSER.DefinedExpressions.ContainsKey(value))
        {
            if (value.GetType() == typeof(string))
            {
                data = FindKeyAndConvertToString(value, PARSER.DefinedExpressions);
                _notFlag = false;
            }
        }

        else if (PARSER.DefinedExpressions.ContainsValue(Convert.ToInt32(value)))
        {
            if (Convert.ToInt32(value).GetType() == typeof(int))
            {

                data = FindValueAndConvertToString(Convert.ToInt32(value), PARSER.DefinedExpressions);

                _notFlag = false;
            }
        }

        if (_notFlag)
        {
            //Otherwise, we'll through an error saying this hasn't been defined.
            Debug.LogError(value + " has not been defined. Perhaps declaring it in the associated .dsl File.");
            return FAILURE;
        }

        //We get the name, keep if it's EXPRESSION or POSE, and the emotion value
        string characterName = data.Split('_')[0];
        string changeType = data.Split('_')[1];
        string characterState = data.Split('_')[2];

        //Now we see if we can grab the image, and have it change...
        DialogueSystemSpriteChanger changer = Find_Sprite_Changer(characterName + "_" + changeType);

        changer.CHANGE_IMAGE(characterState);

        ShiftCursorPosition(-1);

        return SUCCESSFUL;
    }

    /// <summary>
    /// Validate if the Pose is defined in the .dsl file
    /// </summary>
    /// <param name="value"></param>
    /// <param name="_notFlag"></param>
    /// <returns></returns>
    static bool ValidatePosesAndChange(string value, ref bool _notFlag)
    {
        //Check if a key matches
        string data = STRINGNULL;

        if (PARSER.DefinedExpressions.ContainsKey(value))
        {
            if (value.GetType() == typeof(string))
            {
                data = FindKeyAndConvertToString(value, PARSER.DefinedPoses);
                _notFlag = false;
            }
        }

        else if (PARSER.DefinedExpressions.ContainsValue(Convert.ToInt32(value)))
        {
            if (Convert.ToInt32(value).GetType() == typeof(int))
            {

                data = FindValueAndConvertToString(Convert.ToInt32(value), PARSER.DefinedPoses);

                _notFlag = false;
            }
        }

        if (_notFlag)
        {
            //Otherwise, we'll through an error saying this hasn't been defined.
            Debug.LogError(value + " has not been defined. Perhaps declaring it in the associated " + dslFileExtention + " File.");
            return FAILURE;
        }

        //We get the name, keep if it's EXPRESSION or POSE, and the emotion value
        string characterName = data.Split('_')[0];
        string changeType = data.Split('_')[1];
        string characterState = data.Split('_')[2];

        Debug.Log(characterState);

        //Now we see if we can grab the image, and have it change...
        DialogueSystemSpriteChanger changer = Find_Sprite_Changer(characterName + "_" + changeType);

        changer.CHANGE_IMAGE(characterState);

        ShiftCursorPosition(-1);

        return SUCCESSFUL;
    }

    /// <summary>
    /// Find the key value from a defined Dictionary
    /// </summary>
    /// <param name="_key"></param>
    /// <param name="_dictionary"></param>
    /// <returns></returns>
    static string FindKeyAndConvertToString(string _key, Dictionary<string, int> _dictionary)
    {
        while (true)
        {
            List<string> keys = new List<string>(_dictionary.Keys);

            foreach (string key in keys)
            {
                if (key == _key)
                    return key;
            }

            return STRINGNULL;
        }
    }

    /// <summary>
    /// Find the value from a defined Dictionary and converts it to a string
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_dictionary"></param>
    /// <returns></returns>
    static string FindValueAndConvertToString(int _value, Dictionary<string, int> _dictionary)
    {
        while (true)
        {
            List<string> keys = new List<string>(_dictionary.Keys);

            int index = 1;

            foreach (string key in keys)
            {
                if (_value == index)
                    return keys[index - 1];

                index++;
            }

            return STRINGNULL;
        }
    }

    /// <summary>
    /// Check if the Dialogue Index is in bounds
    /// </summary>
    /// <param name="index"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    static bool InBounds(int index, List<string> array) => (index >= reset) && (index < array.Count);

    /// <summary>
    /// The coroutine for waiting for a certain amount of milliseconds
    /// </summary>
    /// <param name="_millseconds"></param>
    /// <returns></returns>
    static IEnumerator DelaySpan(float _millseconds)
    {

        OnDelay = true;

        while (OnDelay)
        {
            yield return new WaitForSeconds(_millseconds / 1000f);
            ShiftCursorPosition(-1);
            OnDelay = false;
        }
    }

    /// <summary>
    /// Returns a SpriteChanger based on the value passed through
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    static DialogueSystemSpriteChanger Find_Sprite_Changer(string _name)
    {
        foreach (DialogueSystemSpriteChanger instance in DialogueSystemSpriteChangers)
        {
            if (_name == instance.Get_Prefix())
                return instance;
        }

        Debug.LogError("The SpriteChange by the name of " + _name + " doesn't exist.");
        return null;
    }

    /// <summary>
    /// Returns a list of all existing Sprite Changers in the Unity Scene
    /// </summary>
    /// <returns></returns>
    static List<DialogueSystemSpriteChanger> FIND_ALL_SPRITECHANGERS()
    {
        DialogueSystemSpriteChanger[] instances = FindObjectsOfType<DialogueSystemSpriteChanger>();

        List<DialogueSystemSpriteChanger> list = new List<DialogueSystemSpriteChanger>();

        foreach (DialogueSystemSpriteChanger instance in instances)
        {
            list.Add(instance);
        }

        return list;
    }

    /// <summary>
    /// Collect all the dialogue written out based on the Dialgoue set specified
    /// </summary>
    /// <param name="_dialogueSet"></param>
    /// <param name="dsPath"></param>
    /// <param name="line"></param>
    /// <param name="position"></param>
    /// <param name="foundDialogueSet"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
    static void CollectDialogue(int _dialogueSet, string dsPath, out string line, ref int position, ref bool foundDialogueSet)
    {
        using (StreamReader fileReader = new StreamReader(dsPath))
        {
            while (true)
            {
                line = fileReader.ReadLine();

                if (line == null)
                {
                    if (foundDialogueSet)
                        return;
                    else
                    {
                        Debug.Log("Dialogue Set " + _dialogueSet.ToString("D3", CultureInfo.InvariantCulture) + " does not exist. Try adding it to the .dsf referenced.");
                        return;
                    }
                }

                line.Split(PARSER.Delimiters);

                if (line.Contains("<DIALOGUE_SET_" + _dialogueSet.ToString("D3", CultureInfo.InvariantCulture) + ">"))
                {
                    foundDialogueSet = true;

                    DialogueSet = _dialogueSet;

                    PARSER.GetDialogue(position);
                }

                position++;
            }
        }
    }

    /// <summary>
    /// Replace the first occuring string or text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="search"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    static string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    /// <summary>
    /// Request to read through a Dialogue Set in the associated .dsl file
    /// </summary>
    /// <param name="_dialogueSet"></param>
    public static void REQUEST_DIALOGUE_SET(int _dialogueSet)
    {
        string dsPath = Application.streamingAssetsPath + @"/" + GET_DIALOGUE_SCRIPTING_FILE();

        int position = 0;

        bool foundDialogueSet = false;

        if (File.Exists(dsPath))
        {
            string line = null;
            CollectDialogue(_dialogueSet, dsPath, out line, ref position, ref foundDialogueSet);
            return;
        }
        Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
    }

    /// <summary>
    /// The coroutine for waiting for the response of the player
    /// </summary>
    /// <returns></returns>
    public static IEnumerator WaitForResponse()
    {
        while (IS_TYPE_IN())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (LineIndex < Dialogue.Count)
                {
                    Proceed();
                    CursorPosition = reset;
                }

            }

            yield return null;
        }
    }

    /// <summary>
    /// In the .dsl file, you have reached the end of the Dialgoue Set being read
    /// </summary>
    public static void End()
    {
        RunningDialogue = false;
        LineIndex = 0;
        SET_TYPE_IN_VALUE(false);
        DISABLE_DIALOGUE_BOX();
        Dialogue.Clear();
        Instance.StopAllCoroutines();
        CursorPosition = reset;
        if (CurrentNode != -1 || CurrentNode < Instance.nodes.Count)
            Instance.nodes[CurrentNode].ExecuteOnEnd();
    }

    /// <summary>
    /// Proceed to the next dialogue if there's any more.
    /// </summary>
    public static void Proceed()
    {
        if (LineIndex < Dialogue.Count - 1 && IS_TYPE_IN() == true)
        {
            LineIndex += 1;

            GET_TMPGUI().text = STRINGNULL;
            SET_TYPE_IN_VALUE(false);

            CursorPosition = reset;

            //We'll parse the next dialogue that is ready to be displayed
            Dialogue[(int)LineIndex] = PARSER.PARSER_LINE(Dialogue[(int)LineIndex]);
        }
        else
        {
            End();
        }
    }

    /// <summary>
    /// Shift the cursor position when reading dialgoue.
    /// </summary>
    /// <param name="_newPosition"></param>
    /// <returns></returns>
    public static uint ShiftCursorPosition(int _newPosition)
    {
        try
        {
            CursorPosition += (uint)_newPosition;
        }
        catch { }
        return CursorPosition;
    }

    /// <summary>
    /// Shift the cursor position when reading dialgoue, and remove a piece of a string
    /// </summary>
    /// <param name="_newPosition"></param>
    /// <param name="_tag"></param>
    /// <param name="_removeFrom"></param>
    /// <returns></returns>
    public static uint ShiftCursorPosition(int _newPosition, string _tag, string _removeFrom)
    {
        try
        {
            CursorPosition += (uint)_newPosition;
            _removeFrom = _removeFrom.Replace(_tag, "");
        }
        catch { }
        return CursorPosition;
    }

    /// <summary>
    /// Update the speed of the text
    /// </summary>
    /// <param name="_textSpeed"></param>
    public static void UPDATE_TEXT_SPEED(TextSpeedValue _textSpeed)
    {
        switch (_textSpeed)
        {
            case TextSpeedValue.SLOWEST: TextSpeed = 0.25f; return;
            case TextSpeedValue.SLOWER: TextSpeed = 0.1f; return;
            case TextSpeedValue.SLOW: TextSpeed = 0.05f; return;
            case TextSpeedValue.NORMAL: TextSpeed = 0.025f; return;
            case TextSpeedValue.FAST: TextSpeed = 0.01f; return;
            case TextSpeedValue.FASTER: TextSpeed = 0.005f; return;
            case TextSpeedValue.FASTEST: TextSpeed = 0.0025f; return;
        }
    }

    /// <summary>
    /// Defines all values that are typed explicitly in the .dsl file
    /// </summary>
    public static void DefineValues()
    {
        //Go into file, and check for all defined values
        try
        {
            //Define the expressions used
            PARSER.Define_Expressions();

            //Define the poses used
            PARSER.Define_Poses();

            //Define the characters in the story
            PARSER.Define_Characters();
        }
        catch { }
    }

    /// <summary>
    /// Runs a dialogue set in the associated .dsl file
    /// </summary>
    /// <param name="_nodeValue"></param>
    public static void Run(int _nodeValue = -1)
    {
        //Check if we are not passed a index value
        if (InBounds((int)LineIndex, Dialogue) && IS_TYPE_IN() == false)
        {
            Dialogue[(int)LineIndex] = Dialogue[(int)LineIndex].Replace("@ ", "").Replace("<< ", "");

            RunningDialogue = true;

            CurrentNode = _nodeValue;

            //We'll parse the very first dialogue that is ready to be displayed
            Dialogue[(int)LineIndex] = PARSER.PARSER_LINE(Dialogue[(int)LineIndex]);

            Instance.StartCoroutine(PrintCycle());

        }
    }

    /// <summary>
    /// Get the file that will be read from the Dialogue System
    /// </summary>
    /// <returns></returns>
    public static string GET_DIALOGUE_SCRIPTING_FILE() => Instance.dslFileName + dslFileExtention;

    /// <summary>
    /// Get the nodes that are being used by the Dialogue System
    /// </summary>
    /// <returns></returns>
    public static List<DialogueNode> GET_NODES() => Instance.nodes;

    /// <summary>
    /// Return if player can respond with input
    /// </summary>
    /// <returns></returns>
    public static bool IS_TYPE_IN() => typeIn;

    /// <summary>
    /// Changes the typeIn value to either true or false
    /// </summary>
    /// <param name="value"></param>
    public static void SET_TYPE_IN_VALUE(bool value) { typeIn = value; }

    /// <summary>
    /// Get the TextMeshPro that's being used for the Dialgoue System
    /// </summary>
    /// <returns></returns>
    public static TextMeshProUGUI GET_TMPGUI() => Instance.TMP_DIALOGUETEXT;

    /// <summary>
    /// Enable the Dialogue Box referred in the Dialgoue System
    /// </summary>
    public static void ENABLE_DIALOGUE_BOX() => Instance.textBoxUI.gameObject.SetActive(true);

    /// <summary>
    /// Disable the Dialogue Box referred in the Dialogue System
    /// </summary>
    public static void DISABLE_DIALOGUE_BOX() => Instance.textBoxUI.gameObject.SetActive(false);

    /// <summary>
    /// When disabled, stop all coroutines
    /// </summary>
    public void OnDisable()
    {
        StopAllCoroutines();
    }
}