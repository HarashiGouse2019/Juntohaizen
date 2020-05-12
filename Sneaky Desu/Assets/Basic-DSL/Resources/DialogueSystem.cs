using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

using DSL.Parser;
using DSL.Behaviour;
using DSL.Styling;
using DSL.InputManagement;
using DSL.Core;
using DSL.PromptOptionCase;

namespace DSL
{
    //Interface for Dialogue System Events
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

    //The actually dialogue system itself.
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

        private static TextSpeedValue SpeedValue;

        public static float TextSpeed { get; private set; }

        //Dialogue that is collected after reading file.
        public static List<Dialogue> DialogueList = new List<Dialogue>();

        //If dialouge is currently running
        public static bool RunningDialogue { get; private set; } = false;

        //If the each dialogue will print out automatically
        public static bool IsAutomatic { get; private set; } = false;

        //If an object should continue operating as the dialogue runs
        public static bool IsDontDisturb { get; private set; } = false;

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

        //A list of sprite changers to allow the changing of images of a character.
        public static List<DialogueSystemSpriteChanger> DialogueSystemSpriteChangers { get; private set; } = new List<DialogueSystemSpriteChanger>();

        //List of all controls for the dialogue
        public static List<Input> DialogueKeyCodes { get; private set; }

        //Fully dedicated objects
        public static DSLBehaviour[] DedicatedObjects { get; private set; }

        //Reset value
        const int reset = 0;

        //Predefined styling tags that already exists.
        static readonly string BOLD = "<b>", BOLDEND = "</b>";
        static readonly string ITALIZE = "<i>", ITALIZEEND = "</i>";
        static readonly string UNDERLINE = "<u>", UNDERLINEEND = "</u>";

        //The formatting of custom-made tags
        static readonly Regex actionRegex = new Regex(@"(<)+\w*action=\w*[a-zA-Z ]+(>$)");
        static readonly Regex insertRegex = new Regex(@"(<)+\w*ins=\w*[a-zA-Z !@#$%^&*()_\-=\\/<>?,./{}[\|:]+(>$)");
        static readonly Regex expressionRegex = new Regex(@"(<)+\w*exp=\w*[A-Z0-9_-]+(>$)");
        static readonly Regex poseRegex = new Regex(@"(<)+\w*pose=\w*[A-Z0-9_-]+(>$)");
        static readonly Regex haltRegex = new Regex(@"(<)+\w*halt=\w*[0-9]+(>$)");
        static readonly Regex speedRegex = new Regex(@"(<)+\w*sp=\w*[0-6](>$)");
        static readonly Regex soundRegex = new Regex(@"(<)+\w*ps=\w*[A-Z0-9_-]+(>$)");
        static readonly Regex voiceRegex = new Regex(@"(<)+\w*v=\w*[A-Z0-9_-]+(>$)");
        static readonly Regex triggerRegex = new Regex(@"(<)+\w*tri=\w*[A-Z0-9_-]+(>$)");

        //DSL file extension
        public static readonly string DSLFileExtention = ".dsl";

        static readonly string STRINGNULL = "";

        //SUCCESS CONSTANT
        const bool SUCCESSFUL = true;

        //FAIL CONSTANT
        const bool FAILURE = false;

        //DSL Layer Constant
        //Objects will be controlled by the Dialogue Scripting System if under this layer.
        public static readonly string DSL_LAYER = "DSL";

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

            //Run the compiler
            new Compiler(GET_DIALOGUE_SCRIPTING_FILE());
        }

        // Start is called before the first frame update
        void Start()
        {
            //Find all sprite changers in the current scene.
            DialogueSystemSpriteChangers = FIND_ALL_SPRITECHANGERS();
        }

        void Update()
        {
            //If [HALT] command ends, continue to exclude any tags that may be parsered
            if (!OnDelay && DialogueList.Count != 0)
            {
                ExcludeAllFunctionTags(DialogueList[(int)LineIndex].Content);
                ExcludeAllStyleTags(DialogueList[(int)LineIndex].Content);
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
                //If we are not allowed to respone, have it run dialgoue until the full sentence is displayed onto the screen
                if (IS_TYPE_IN() == false)
                {
                    Styler.EnableDialogueBox();

                    Styler.GetText().text = STRINGNULL;

                    //If this is a new sentence, we want to clear the current text that is displaying
                    var text = Styler.GetText().text;

                    //If we haven't reached the end of the dialogue set, get the next line avaliable
                    if (LineIndex < DialogueList.Count) text = DialogueList[(int)LineIndex].Content;

                    //This is our "type writer" effect for our dialogue system.
                    //We'll go through each character one.
                    //If a halt command is detected at a certain position, CursorPosition will not increment.
                    for (CursorPosition = 0; CursorPosition < text.Length + 1; CursorPosition += (uint)((OnDelay) ? 0 : 1))
                    {
                        try
                        {
                            //This helps guide us to taking apart the tags in the string.
                            if (LineIndex < DialogueList.Count) text = DialogueList[(int)LineIndex].Content;

                            //We'll update the TMP text
                            Styler.GetText().text = text.Substring(0, (int)CursorPosition);

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

            DialogueSystemParser.ParseLine(_text);
            //Action tag!
            ExecuteActionFunctionTag(actionRegex, ref _text);

            //Insert tag!
            ExecuteInsertFunctionTag(insertRegex, ref _text);

            //Speed Command Tag: It will consider all of the possible values.
            ExecuteSpeedFunctionTag(speedRegex, ref _text);

            //Expression tag!
            ExecuteExpressionFunctionTag(expressionRegex, ref _text);

            //Pose tag!
            ExecutePoseFunctionTag(poseRegex, ref _text);

            //Halt tage
            ExecuteHaltFunctionTag(haltRegex, ref _text);
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

                if (tag.Contains("sp="))
                {
                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {

                            endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                            tag = DialogueList[(int)LineIndex].Content.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {

                                //<sp=3>
                                int speed = Convert.ToInt32(tag.Split(Compiler.Delimiters)[1].Split('=')[1]);

                                SpeedValue = (TextSpeedValue)speed;

                                _line = ReplaceFirst(_line, tag, STRINGNULL);

                                DialogueList[(int)LineIndex].AddContent(_line);

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
                if (tag.Contains("action="))
                {
                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                    string value = STRINGNULL;
                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {

                            endTagPos = Array.IndexOf(stringRange.ToCharArray(), letter);

                            tag = DialogueList[(int)LineIndex].Content.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {
                                if (OnDelay == false)
                                {
                                    string actionWord = tag.Trim(Compiler.Delimiters[0], Compiler.Delimiters[1]).Split('=')[1];

                                    value = "*" + actionWord + "*";

                                    _line = ReplaceFirst(_line, tag, value);

                                    ShiftCursorPosition(value.Length);

                                    DialogueList[(int)LineIndex].AddContent(_line);
                                }
                            }
                            return SUCCESSFUL;
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

                if (tag.Contains("ins="))
                {
                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);

                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {

                            endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                            tag = _line.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {
                                if (OnDelay == false)
                                {
                                    string value = tag.Trim(Compiler.Delimiters[0], Compiler.Delimiters[1]).Split('=')[1];

                                    _line = ReplaceFirst(_line, tag, value);

                                    ShiftCursorPosition(value.Length);

                                    DialogueList[(int)LineIndex].AddContent(_line);

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

                if (tag.Contains("halt="))
                {

                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);

                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {
                            endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                            tag = DialogueList[(int)LineIndex].Content.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {
                                if (OnDelay == false)
                                {
                                    /*Now we do a substring from the current position to 4 digits.*/

                                    int value = Convert.ToInt32(tag.Trim(Compiler.Delimiters[0], Compiler.Delimiters[1]).Split('=')[1]);

                                    int millsecs = Convert.ToInt32(value);

                                    Instance.StartCoroutine(DelaySpan(millsecs));

                                    _line = ReplaceFirst(_line, tag, STRINGNULL);

                                    DialogueList[(int)LineIndex].AddContent(_line);

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
                if (tag.Contains("exp="))
                {
                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {
                            endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                            tag = DialogueList[(int)LineIndex].Content.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {
                                /*The system will now take this information, from 0 to the current position
                                 and split it down even further, taking all the information.*/

                                string value = tag.Trim(Compiler.Delimiters[0], Compiler.Delimiters[1]).Split('=')[1];

                                _line = ReplaceFirst(_line, tag, STRINGNULL);

                                DialogueList[(int)LineIndex].AddContent(_line);

                                return Validater.ValidateExpressionsAndChange(value, ref notFlaged);
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
                if (tag.Contains("pose="))
                {
                    int startTagPos = (int)CursorPosition;
                    int endTagPos = 0;
                    string stringRange = _line.Substring((int)CursorPosition, _line.Length - (int)CursorPosition);
                    foreach (char letter in stringRange)
                    {
                        if (letter == '>')
                        {
                            endTagPos = (Array.IndexOf(stringRange.ToCharArray(), letter));

                            tag = DialogueList[(int)LineIndex].Content.Substring(startTagPos, endTagPos + 1);

                            if (_tagExpression.IsMatch(tag))
                            {
                                /*The system will now take this information, from 0 to the current position
                                 and split it down even further, taking all the information.*/

                                string value = tag.Trim(Compiler.Delimiters[0], Compiler.Delimiters[1]).Split('=')[1];

                                _line = ReplaceFirst(_line, tag, STRINGNULL);

                                DialogueList[(int)LineIndex].AddContent(_line);

                                return Validater.ValidatePosesAndChange(value, ref notFlaged);
                            }
                        }
                    }
                }
            }
            catch { }
            return FAILURE;
        }

        /// <summary>
        /// Find the key value from a defined Dictionary
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_dictionary"></param>
        /// <returns></returns>
        public static string FindKeyAndConvertToString(string _key, Dictionary<string, int> _dictionary)
        {
            while (true)
            {
                //Take the keys from the dictionary, and convert to a list
                List<string> keys = new List<string>(_dictionary.Keys);

                //Iterate through the list of keys until we find what we're looking for
                foreach (string key in keys)
                {
                    if (key == _key)
                        return key;
                }

                //Fail to return the key value, we return null
                return STRINGNULL;
            }
        }

        /// <summary>
        /// Find the value from a defined Dictionary and converts it to a string
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_dictionary"></param>
        /// <returns></returns>
        public static string FindValueAndConvertToString(int _value, Dictionary<string, int> _dictionary)
        {
            while (true)
            {
                //Take keys from dictionary, and convert to a list
                List<string> keys = new List<string>(_dictionary.Keys);

                //Set an index of one. This will be used to find the index.
                int index = 1;

                //Iterate through the list of Keys, until we know what index it is.
                foreach (string key in keys)
                {
                    if (_value == index)
                        return keys[index - 1];

                    index++;
                }

                //Fail to find the index, we return string null
                return STRINGNULL;
            }
        }

        /// <summary>
        /// Check if the Dialogue Index is in bounds
        /// </summary>
        /// <param name="index"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        static bool InBounds(int index, List<Dialogue> array) => (index >= reset) && (index < array.Count);

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
        public static DialogueSystemSpriteChanger Find_Sprite_Changer(string _name)
        {
            try
            {
                foreach (DialogueSystemSpriteChanger instance in DialogueSystemSpriteChangers)
                {
                    if (_name == instance.Get_Prefix())
                        return instance;
                }
                throw new UnknownSpriteChangerException("The SpriteChange by the name of " + _name + " doesn't exist.");
            }
            catch (UnknownSpriteChangerException unknownSpriteChangerException) { Debug.LogException(unknownSpriteChangerException); }
            return null;
        }

        /// <summary>
        /// Returns a list of all existing Sprite Changers in the Unity Scene
        /// </summary>
        /// <returns></returns>
        public static List<DialogueSystemSpriteChanger> FIND_ALL_SPRITECHANGERS()
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
        /// Try to go to the next dialogue
        /// </summary>
        static void TryNext()
        {
            if (LineIndex < DialogueList.Count)
            {
                CursorPosition = reset;
                Proceed();
            }
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

            foreach (string data in Compiler.CompiledData)
            {
                //We'll read a line
                line = data;

                //If the line is null...
                if (line == null)
                {
                    //Return the dialogue set if we've found it.
                    if (foundDialogueSet)
                        return;

                    //Otherwise, we did not find the dialogue set.
                    else
                    {
                        Debug.Log("Dialogue Set " + _dialogueSet.ToString("D3", CultureInfo.InvariantCulture) + " does not exist. Try adding it to the .dsl referenced.");
                        return;
                    }
                }

                //If we find anything resemblins <DIALOGUE_SET_###, we found the dialogue set.
                if (line.Contains("DIALOGUE_SET_" + _dialogueSet.ToString("D3", CultureInfo.InvariantCulture)))
                {
                    string[] expressions = null;
                    try { expressions = line.Replace(" ", "").Split(Compiler.Delimiters); } catch { }

                    //Found the dialogue set
                    foundDialogueSet = true;

                    //Set the set to this one
                    DialogueSet = _dialogueSet;

                    //We'll also check additional information, like to auto play dialouge, or don't disturb the player
                    //By using a foreach loop, those who are using DSL can have the expressions in any order, and still
                    //have it read by the system.
                    try
                    {
                        foreach (string expression in expressions)
                        {
                            //If the dialogue is automatic
                            if (expression == Compiler.Keywords[10]) IsAutomatic = true;

                            //if the dialogue can't disturb the player
                            if (expression == Compiler.Keywords[11]) IsDontDisturb = true;

                        }
                    }
                    catch { /*This just means there was nothing that we could of done.*/}

                    //Now, we then collect all the dialogue knowing how this is going to execute.

                    //This is the function that'll need some modifying.
                    //We need to make sure it can get all @, no matter how many tabs there are.
                    Compiler.GetDialogue(position);

                    return;
                }

                //Move to the next line in file
                position++; 
            }
            line = null;
        }

        /// <summary>
        /// Replace the first occuring string or text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(string text, string search, string replace)
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
            //While retrieving the Dialogue_Set, we need to know
            //if there's additional information, such as if it's automatic,
            //and if you don't want to disturb the player and the environment.

            //We get the DSL file from streaming assets
            string dsPath = Application.streamingAssetsPath + @"/" + GET_DIALOGUE_SCRIPTING_FILE();

            //We'll use this to keep track of the position in file
            int position = 0;

            //And check if we've found the requested dialogue set.
            bool foundDialogueSet = false;

            if (File.Exists(dsPath))
            {
                string line = null;

                //We'll collect dialogue based on the dialogue set requested and the path of the dsl
                CollectDialogue(_dialogueSet, dsPath, out line, ref position, ref foundDialogueSet);

                return;
            }

            Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
        }

        /// <summary>
        /// The coroutine for waiting for the response of the player
        /// </summary>
        /// <returns></returns>
        public static IEnumerator WaitForResponse(bool _isPromptRunning = false)
        {
            while (IS_TYPE_IN())
            {
                //Check if prompt is running
                switch (_isPromptRunning)
                {
                    case false:
                        //Check if we pressed the PROCEED button
                        //We'll also check if it's running automatically or not
                        if ((InputManager.GetButtonDown(Functionality.PROCEED) && !IsAutomatic) || IsAutomatic)
                            TryNext();
                        break;

                    case true:

                        break;

                    default:
                        break;

                }

                yield return null;
            }
        }

        /// <summary>
        /// Jump to an index in the Dialogue List
        /// </summary>
        /// <param name="_index"></param>
        public static void JumpToLineIndex(int _index) => LineIndex = (uint)_index;

        /// <summary>
        /// In the .dsl file, you have reached the end of the Dialgoue Set being read
        /// </summary>
        public static void End()
        {
            RunningDialogue = false;

            LineIndex = 0;

            SET_TYPE_IN_VALUE(false);

            Styler.DisableDialogueBox();

            DialogueList.Clear();

            Instance.StopAllCoroutines();

            CursorPosition = reset;

            //Rerun all behaviours
            foreach (DSLBehaviour objectToResume in DSLBehaviour.FindAllObjectsInDSLLayer())
            {
                try
                {
                    objectToResume.ResumeBehaviour();
                }
                catch { }
            }
        }

        /// <summary>
        /// Proceed to the next dialogue if there's any more.
        /// </summary>
        public static void Proceed()
        {
            if (LineIndex < DialogueList.Count - 1 && IS_TYPE_IN() == true)
            {
                SET_TYPE_IN_VALUE(false);

                LineIndex += 1;

                Styler.GetText().text = STRINGNULL;

                CursorPosition = reset;
                //We'll parse the next dialogue that is ready to be displayed
                DialogueList[(int)LineIndex].AddContent(DialogueSystemParser.ParseLine(DialogueList[(int)LineIndex].Content));
            }
            else
                End();
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
        /// Runs a dialogue set in the associated .dsl file
        /// </summary>
        /// <param name="_nodeValue"></param>
        /// <param name="_isAuto"></param>
        /// <param name="_dontInterrupt"></param>
        public static void Run()
        {
            //When the cycle starts, we need to know if we should stop all the activity of any objects
            //deriving from DSLBehaviour
            if (!IsDontDisturb)
            {

                foreach (DSLBehaviour objectToStop in DedicatedObjects)
                    objectToStop.StopBehaviour();
            }

            //Check if we are not passed a index value
            if (InBounds((int)LineIndex, DialogueList) && IS_TYPE_IN() == false)
            {
                //If there is no character or "???" operator, we suspect it to just have "@ ", thus we'll remove it.
                //We will also remove the marker for the end of sentence
                DialogueList[(int)LineIndex].AddContent(DialogueList[(int)LineIndex].Content.Replace("@ ", "").Replace("<< ", ""));

                //We'll say that we want to run the dialogue
                RunningDialogue = true;

                //We'll parse the very first dialogue that is ready to be displayed
                DialogueList[(int)LineIndex].AddContent(DialogueSystemParser.ParseLine(DialogueList[(int)LineIndex].Content));

                //Start the Printing Cycle
                Instance.StartCoroutine(PrintCycle());
            }
        }

        /// <summary>
        /// Get the file that will be read from the Dialogue System
        /// </summary>
        /// <returns></returns>
        public static string GET_DIALOGUE_SCRIPTING_FILE() => Instance.dslFileName + DSLFileExtention;

        /// <summary>
        /// Return if player can respond with input
        /// </summary>
        /// <returns></returns>
        public static bool IS_TYPE_IN() => typeIn;

        /// <summary>
        /// Changes the typeIn value to either true or false
        /// </summary>
        /// <param name="value"></param>
        public static void SET_TYPE_IN_VALUE(bool value) => typeIn = value;

        /// <summary>
        /// On Scene Loaded Event
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Find all objects after a new scene has been loaded
            DedicatedObjects = DSLBehaviour.FindAllObjectsInDSLLayer();
        }

        public void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// When disabled, stop all coroutines
        /// </summary>
        public void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StopAllCoroutines();
        }
    }
}