using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using DSL.PromptOptionCase;
using DSL.InputManagement;
using DSL.Text;

namespace DSL.Core
{
    public static class Compiler
    {
        public static List<string> CompiledData { get; private set; } = new List<string>();

        public static List<CommandCallLocation> commandCallLocations = new List<CommandCallLocation>();

        public static Dictionary<string, int> DefinedExpressions { get; private set; } = new Dictionary<string, int>();

        public static Dictionary<string, int> DefinedPoses { get; private set; } = new Dictionary<string, int>();

        public static List<string> DefinedCharacters { get; private set; } = new List<string>();

        public static List<Prompt> DefinedPrompts { get; private set; } = new List<Prompt>();

        public static List<Point> JumpPoints { get; private set; } = new List<Point>();

        public static List<int> DialogueReferenceValue { get; private set; } = new List<int>();

        //Using this to bitmask data for reading if using Input Manager, and if using DSL's or Unity's
        public static bool Is_Using_Input_Manager = false;

        public static char[] Delimiters { get; } = {
                '<',
                '>',
                '[',
                ']',
                ',',
                '|',
                '.',
                '{',
                '}'};

        public static string[] Tokens { get; } = {
                "<<",
                "::",
                "END",
                "@",
                "???",
                "=",
                "is",
                "from",
                "insert",
                "to",
                "and",
                ">>"};

        public static string[] Keywords { get; } = {
                "speed",
                "bold",
                "italize",
                "underline",
                "sound",
                "expression",
                "action",
                "halt",
                "pose",
                "inser",
                "auto",
                "dontDetain",
                "capacity",
                "CBIM",
                "jump",
                "max",
                "min",
                "prompt",
                "option",
                "format",
                "type",
                "list",
                "grid",
                "call",
                "omit",
                "parent",
                "case",
                "break",
                "out",
                "set",
                "this",
                "exit"
                };

        public static string[] ValidTextSpeeds { get; } = {
                "SLOWEST",
                "SLOWER",
                "SLOW",
                "NORMAL",
                "FAST",
                "FASTER",
                "FASTEST" };

        const bool SUCCESSFUL = true;
        const bool FAILURE = false;

        const string STRINGNULL = "";
        const string WHITESPACE = " ";
        const string UNDERSCORE = "_";
        const string TAB = "\t";
        const string RETURN = "\r";
        const string END = "<END>";

        static Compiler()
        {
            string source = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            using (StreamReader fileReader = new StreamReader(source))
            {
                while (!fileReader.EndOfStream)
                {
                    string line = fileReader.ReadLine();
                    line = line.Trim('\t', ' ');
                    CompiledData.Add(line);
                }

                //Tokenize our long text
                new Lexer();
            }

            //Initialize all Data
            try { Init(); }
            catch { }
        }

        /// <summary>
        /// Defines all values that are typed explicitly in the .dsl file
        /// </summary>
        public static void Init()
        {
            //Go into file, and check for all defined values
            //Define the expressions used
            DefineExpressions();

            //Define the poses used
            DefinePoses();

            //Define the characters in the story
            DefineCharacters();

            //Define all key codes
            DefineKeyCodes();

            //Define all prompts and their options
            DefinePrompts();
        }

        /// <summary>
        /// Define expressions listed underneath <EXPRESSIONS> in the .dsl file
        /// </summary>
        public static void DefineExpressions()
        {
            try
            {
                int position = 0;

                bool foundExpression = false;

                foreach (string line in CompiledData)
                {

                    string dataLine = line.Trim('\t', ' ');

                    if (Has(dataLine, "</expressions>") && foundExpression)
                        return;

                    if (Has(dataLine, "<expressions>"))
                    {
                        foundExpression = true;
                        try { GetExpressions(position); } catch { }
                    }

                    position++;
                }
            }
            catch { }
        }

        /// <summary>
        /// Define poses listed underneath <POSES> in the .dsl file
        /// </summary>
        public static void DefinePoses()
        {
            try
            {
                int position = 0;

                bool foundPose = false;

                foreach (string line in CompiledData)
                {
                    string dataLine = line.Trim('\t', ' ');

                    if (Has(dataLine, "</poses>") && foundPose)
                        return;

                    if (Has(dataLine, "<poses>"))
                    {
                        foundPose = true;
                        try { GetPoses(position); } catch { }
                    }

                    position++;
                }

            }
            catch { }
        }

        /// <summary>
        /// Define characters listed underneath <CHARACTERS> in the .dsl file
        /// </summary>
        public static void DefineCharacters()
        {
            try
            {
                int position = 0;

                bool foundCharacter = false;

                foreach (string line in CompiledData)
                {

                    string dataLine = line.Trim('\t', ' ');

                    if (Has(dataLine, "</characters>") && foundCharacter)
                        return;

                    if (Has(dataLine, "<characters>"))
                    {
                        foundCharacter = true;
                        try { GetCharacterNames(position); } catch { }
                    }

                    position++;
                }

            }
            catch { }
        }

        /// <summary>
        /// This will read the <KEYCODES> tag, and res
        /// </summary>
        public static void DefineKeyCodes()
        {
            try
            {
                int position = 0;

                bool foundKeyCode = false;



                foreach (string line in CompiledData)
                {
                    string dataLine = line.Trim('\t', ' ');

                    if (Has(dataLine, "</keycodes>") && foundKeyCode)
                        return;

                    if (Has(dataLine, "<keycodes>"))
                    {
                        string[] expressions = null;
                        try { expressions = dataLine.Replace(WHITESPACE, STRINGNULL).Split(Delimiters); } catch { }

                        //Check for addition inforamtion
                        foreach (string expression in expressions)
                        {
                            //If KeyCodes will be controlled by the InputManager in DSLNative or Unity
                            //If controlled by input managers, all the input will be read through those
                            //instead of being read in the .dsl directly.
                            try { if (expression == Keywords[13]) Is_Using_Input_Manager = true; } catch { };
                        }

                        foundKeyCode = true;
                        try { GetKeyCodes(position); } catch { }
                    }

                    position++;

                }
            }
            catch { }
        }

        /// <summary>
        /// This will read all Prompts
        /// </summary>
        public static void DefinePrompts()
        {
            try
            {
                int position = 0;

                bool foundPrompt = false;

                foreach (string line in CompiledData)
                {

                    string dataLine = line.Trim('\t', ' ');

                    if (Has(dataLine, "</prompt>") && foundPrompt)
                        return;

                    if (Has(line, "<prompt"))
                    {

                        //First, we want to know what prompt number it is,
                        //as well as the capacity of the prompt
                        int promptNumber = 0;
                        int promptCapacity = Prompt.DEFAULT_CAPACITY;

                        //Now we split the numbers
                        string[] dataSet = null;
                        try
                        {
                            //Try to split the information
                            try { dataSet = dataLine.Trim(Delimiters[0], Delimiters[1]).Split(Delimiters[5]); }
                            catch { };

                            promptNumber = Convert.ToInt32(dataSet[0].Split('_')[1], CultureInfo.InvariantCulture);

                            foreach (string keyword in Keywords)
                            {
                                try
                                {
                                    //If CAPACITY is defined
                                    if (keyword == GetKeyWord("capacity"))
                                        foreach (string token in Tokens)
                                        {
                                            if (token == Tokens[5] || token == Tokens[6])
                                                promptCapacity = Convert.ToInt32(dataSet[1].Split(Tokens[6].ToCharArray())[2]);
                                        }

                                    //IF FORMAT is defined
                                    if (keyword == GetKeyWord("format"))
                                        foreach (string token in Tokens)
                                        {
                                            if (token == Tokens[5] || token == Tokens[6])
                                            {
                                                //TODO: retrieve the formating data of how the options will display.

                                            }
                                        }
                                }
                                catch { }
                            }

                            List<Option> options = null;

                            //Knowing that we got the information of our prompt, we can collect the options underneath it.
                            GetOptions(position, promptNumber, promptCapacity, ref options);

                            Prompt newPrompt = new Prompt(promptNumber, options, promptCapacity);

                            DefinedPrompts.Add(newPrompt);
                        }
                        catch { }
                    }
                    position++;
                }
            }
            catch { }
        }

        /// <summary>
        /// Get defined expressions underneath <EXPRESSIONS> in the .dsl file
        /// </summary>
        /// <param name="_position">Where to start collecting data</param>
        static void GetExpressions(int _position)
        {
            bool atTargetLine = false;

            int position = 0;

            foreach (string line in CompiledData)
            {
                string dataLine = line.Trim('\t', ' ');

                if (Has(dataLine, "</expressions>") && atTargetLine)
                    return;


                if (position > _position)
                {
                    atTargetLine = true;

                    if (dataLine != STRINGNULL)
                    {
                        string[] data = dataLine.Split('=');
                        DefinedExpressions.Add(data[0].Replace(WHITESPACE, STRINGNULL), Convert.ToInt32(data[1].Replace(WHITESPACE, STRINGNULL)));
                    }
                }

                position++;
            }

        }

        /// <summary>
        /// Get poses defined underneath <POSES> inside the .dsl file.
        /// </summary>
        /// <param name="_position">Where to start collecting data.</param>
        static void GetPoses(int _position)
        {
            bool atTargetLine = false;

            int position = 0;

            foreach (string line in CompiledData)
            {
                string dataLine = line.Trim('\t', ' ');

                if (Has(dataLine, "</poses>") && atTargetLine)
                    return;

                if (position > _position)
                {
                    atTargetLine = true;

                    if (dataLine != STRINGNULL)
                    {
                        string[] data = dataLine.Split('=');
                        DefinedPoses.Add(data[0].Replace(WHITESPACE, STRINGNULL), Convert.ToInt32(data[1].Replace(WHITESPACE, STRINGNULL)));
                    }

                }

                position++;

            }
        }

        /// <summary>
        /// Get Character names defined underneath <CHARACTERS> inside the .dsl file
        /// </summary>
        /// <param name="_position">Where to start collecting data.</param>
        static void GetCharacterNames(int _position)
        {
            bool atTargetLine = false;

            int position = 0;

            foreach (string line in CompiledData)
            {

                string dataLine = line.Trim('\t', ' ');

                if (atTargetLine)
                {
                    if (Has(dataLine, "</characters>"))
                        return;
                }


                if (position > _position)
                {
                    atTargetLine = true;

                    if (dataLine != STRINGNULL)
                        DefinedCharacters.Add(dataLine);

                }

                position++;
            }

        }

        /// <summary>
        /// Get Key Codes underneath inside the .dsl file
        /// </summary>
        /// <param name="_position"></param>
        static void GetKeyCodes(int _position)
        {
            int position = 0;
            bool atTargetLine = false;


            foreach (string line in CompiledData)
            {
                string dataLine = line.Trim('\t', ' ');

                if (Has(dataLine, "</keycodes>") && atTargetLine)
                    return;

                if (position > _position)
                {
                    atTargetLine = true;

                    if (dataLine != STRINGNULL)
                    {
                        //We'll have to do a little something different
                        //We split KEYCODE.I (is or =) PROCEED (just an example)
                        string[] declaration = null;

                        string keyCodeValue = STRINGNULL;

                        string functionalityValue = STRINGNULL;

                        string inputName = STRINGNULL;

                        string inputDescriptiveName = STRINGNULL;

                        KeyCode newKeyCode = KeyCode.None;

                        bool multiWord = false;

                        try
                        {

                            // Retrieve KeyCodeValue and FunctionalityValue as strings
                            RetrieveKeyCodeAndFunctionality(dataLine, ref declaration, ref keyCodeValue, ref functionalityValue, ref multiWord);

                            // Parse string keyCodeValue to actual KeyCode enumerator value
                            newKeyCode = ParseStringToKeyCode(keyCodeValue, newKeyCode, multiWord, out bool result);

                            // Retrieve name of input
                            inputName = GetInputNameFromDSL(dataLine, inputName);

                            // Retrieve descriptive name of input
                            inputDescriptiveName = GetDescriptiveNameFromDSL(dataLine, inputDescriptiveName);

                            // Register new input
                            InvokeRegistrationToInputManager(keyCodeValue, functionalityValue, inputName, inputDescriptiveName, newKeyCode);

                            //Validate
                            if (!result) throw new CantRegisterException("Input registration failed on line " + (position + 1));
                        }
                        catch (CantRegisterException cantRegisterException)
                        {
                            Debug.LogException(cantRegisterException);
                            break;
                        }
                    }
                }
                position++;
            }

        }

        /// <summary>
        /// Calls InputManager Register Method
        /// </summary>
        /// <param name="keyCodeValue"></param>
        /// <param name="functionalityValue"></param>
        /// <param name="inputName"></param>
        /// <param name="inputDescriptiveName"></param>
        /// <param name="newKeyCode"></param>
        private static void InvokeRegistrationToInputManager(string keyCodeValue, string functionalityValue, string inputName, string inputDescriptiveName, KeyCode newKeyCode)
        {
            try
            {


                //Decide rather we're explicitly setting KeyCodes in .dsl or in the actual InputManager
                //If the Input Manager is not being used, all Input will be registed from .dsl
                //Otherwise, those values may already had been set by the user.
                if (Is_Using_Input_Manager == false)
                {
                    //Parse string into Functionality
                    Functionality func = (Functionality)Enum.Parse(typeof(Functionality), functionalityValue);

                    //Register our keys for them to be finalized
                    InputManager.Register((inputName == STRINGNULL) ? keyCodeValue : inputName, inputDescriptiveName, newKeyCode, func);
                };
            }
            catch { }
        }

        /// <summary>
        /// Return the descriptive name of the input
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inputDescriptiveName"></param>
        /// <returns></returns>
        private static string GetDescriptiveNameFromDSL(string line, string inputDescriptiveName)
        {
            //And I'll also try to get the descriptiveName
            try
            {
                //Remove quotes
                inputDescriptiveName = line.Split(Delimiters)[3].Replace("\"", STRINGNULL);
                inputDescriptiveName = inputDescriptiveName.Trim(WHITESPACE.ToCharArray());

            }
            catch { }

            return inputDescriptiveName;
        }

        /// <summary>
        /// Return the name of the input
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inputName"></param>
        /// <returns></returns>
        private static string GetInputNameFromDSL(string line, string inputName)
        {
            //I'm going to try grabbing the name
            //That will be after the declaration
            try
            {
                //Remove quotes
                inputName = line.Split(Delimiters)[2].Replace("\"", STRINGNULL);
                inputName = inputName.Trim(WHITESPACE.ToCharArray());
            }
            catch { }

            return inputName;
        }

        /// <summary>
        /// Try to convert a string into a valid KeyCode
        /// </summary>
        /// <param name="keyCodeValue"></param>
        /// <param name="newKeyCode"></param>
        /// <param name="multiWord"></param>
        /// <returns></returns>
        private static KeyCode ParseStringToKeyCode(string keyCodeValue, KeyCode newKeyCode, bool multiWord, out bool _result)
        {
            try
            {
                //Parse string into KeyCode
                //If it's something like LeftArrow, it has already
                //set to the right format, so don't Capticalize
                newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), multiWord ? keyCodeValue : StringManip.Capitalize(keyCodeValue));
                _result = SUCCESSFUL;
            }
            catch { _result = FAILURE; }

            return newKeyCode;
        }

        /// <summary>
        /// Read the declaration, and pair KeyCode and its functionality
        /// </summary>
        /// <param name="line"></param>
        /// <param name="declaration"></param>
        /// <param name="keyCodeValue"></param>
        /// <param name="functionalityValue"></param>
        /// <param name="multiWord"></param>
        private static void RetrieveKeyCodeAndFunctionality(string line, ref string[] declaration, ref string keyCodeValue, ref string functionalityValue, ref bool multiWord)
        {

            try
            {
                //Then, we continue with the declaration itself
                //Split off operators first
                declaration = line.Replace(WHITESPACE, STRINGNULL).Split(Delimiters);

                //Replace any tokens with
                foreach (string token in Tokens)
                {
                    declaration[1] = declaration[1].Replace(token, WHITESPACE);
                }

                //Then this time, split with a space character
                string[] value = declaration[1].Split(WHITESPACE.ToCharArray());

                keyCodeValue = value[0];

                //Check if there's a keycode with underscore
                if (Has(keyCodeValue, UNDERSCORE))
                {
                    multiWord = true;
                    keyCodeValue = StringManip.PascalCase(keyCodeValue);
                }

                functionalityValue = value[1];
            }
            catch { } // We should just get "KEYCODE" "I" "is" or "=" and "PROCEED" 

        }

        /// <summary>
        /// Get all dialogue in a specified Dialogue_Set inside a .dsl file
        /// </summary>
        /// <param name="_position">Position to start collecting data</param>
        public static void GetDialogue(int _position)
        {
            //TODO: For this function only, collect information such as $ and @, $ stating that it's not dialogue-centric, and @ meaning
            //that this is actual dialogue.

            //That way, commands like CALL, and the start of other code can happen through those points

            //Toggle if we are at a desired position in the file.
            bool atTargetLine = false;

            int position = 0;

            foreach (string line in CompiledData)
            {

                string dataLine = line.Trim('\t', ' '); //Remove all tabs and spaces, so that we can always get to @

                //If we reach the end of the dialogue set, we are done reading it
                if (Has(dataLine, "</dialogueSet>") && atTargetLine)
                    return;

                //However, if we are at the DialogueSet tag with a specified number, we collect all the dialogue that starts with "@"
                if (position > _position)
                {
                    atTargetLine = true; //Toggle on

                    //Collecting Dialogue
                    #region Dialogue Collection ("@")
                    //Make sure that we are specifically encountering "<...>"
                    if (dataLine != STRINGNULL && Validater.ValidateLineEndOperartor(dataLine, out dataLine))
                    {
                        //Validate the use of a character
                        dataLine = Validater.ValidateCharacterUsage(dataLine, position);

                        DialogueSystem.DialogueData.Add(dataLine);
                        //Get the Dialogue Reference Number for use
                        DialogueReferenceValue.Add(position + 1);
                    }
                    #endregion

                }
                position++;
            }
        }

        /// <summary>
        /// Grabs a keyword defined in the array.
        /// </summary>
        /// <param name="_keyword"></param>
        /// <returns></returns>
        public static string GetKeyWord(string _keyword)
        {
            foreach (string keyword in Keywords)
            {
                if (_keyword == keyword) return keyword;
            }

            return STRINGNULL;
        }

        /// <summary>
        /// Collected the total amount of options.
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_number"></param>
        /// <param name="_capacity"></param>
        /// <param name="_options"></param>
        public static void GetOptions(int _position, int _number, int _capacity, ref List<Option> _options)
        {
            _options = new List<Option>();

            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            string dataLine = null;

            bool atTargetLine = false;

            int position = 0;

            Option newOption = null;

            try
            {
                foreach (string line in CompiledData)
                {

                    dataLine = line.Trim('\t', ' ');

                    if (dataLine == END && atTargetLine)
                    {
                        if (Prompt.ValidateCapacity(_capacity, _options.Count))
                            return;
                        else throw new ExceedsCapacityException("The number of options in Prompt " + _number + " exceeds its capacity of " + _capacity + ": Line " + position);
                    }

                    if (position > _position)
                    {
                        atTargetLine = true;

                        //If OPTION is defined before the numerical representation
                        if (Has(dataLine, GetKeyWord("OPTION")))
                        {
                            //We have reached the point where we can add our new options for creating a prompt
                            //Options are shown as OPTION 1 >> Do a thing.
                            string[] optionData = dataLine.Replace(Keywords[18], STRINGNULL).Split('>');

                            int optionID = Convert.ToInt32(optionData[0].Replace(WHITESPACE, STRINGNULL));

                            string optionContent = optionData[2].Trim(WHITESPACE.ToCharArray());

                            newOption = new Option(optionID, optionContent);

                            //Create a new option, and add it to our options list
                            _options.Add(newOption);
                        }
                    }

                    position++;

                }
            }
            catch (ExceedsCapacityException e) { Debug.LogError(e.Message); }
            _options = null;
        }

        /// <summary>
        /// Get a defined prompt.
        /// </summary>
        /// <param name="_promptNumber"></param>
        public static Prompt GetDefinedPrompt(int _promptNumber)
        {
            //Iterate through our prompt list.
            foreach (Prompt prompt in DefinedPrompts)
            {
                if (_promptNumber == prompt.Number) return prompt;
            }

            return null;
        }

        public static Option GetDefinedOption(int _optionID, Prompt _originPrompt)
        {
            foreach (Prompt prompt in DefinedPrompts)
                if (prompt == _originPrompt) foreach (Option option in _originPrompt.Options)
                        if (option.ID == _optionID) return option;

            return null;
        }

        /// <summary>
        /// Looks for the marker ($) either if is stated on the line of a dialogue
        /// or if in between dialogue.
        /// </summary>
        /// <param name="_currentLine"></param>
        public static bool PromptCall(int _position, out Prompt _currentPrompt)
        {
            Prompt parentPrompt = null;

            Prompt calledPrompt = null;

            int position = 0;

            bool callMade = false;

            //This will help specify the scope of each prompt
            bool dontSearch = false;

            foreach (string dataLine in CompiledData)
            {
                string line = dataLine.Trim('\t', ' ');

                #region CALL PROMPT
                if (position >= _position)
                {
                    #region CASE OPTION occurs
                    if (Has(line, GetKeyWord("CASE") + " " + GetKeyWord("OPTION")))
                        dontSearch = true;
                    #endregion

                    if (parentPrompt != null && dontSearch == false && PromptStack.StackedPrompts.Count == 0 && Validater.ValidateLineEndOperartor(dataLine, out string moddedLine))
                    {

                        moddedLine = Validater.ValidateCharacterUsage(moddedLine.Trim('\t', ' '), _position);

                        calledPrompt.SetDialogueReference(moddedLine);
                        calledPrompt.FindDialoguePosition();

                        _currentPrompt = calledPrompt;

                        JumpPoints.Add(new Point(parentPrompt.gotoLine));

                        parentPrompt = null;
                        return SUCCESSFUL;
                    }

                    #region Validate Call Prompt
                    if (Validater.ValidateCallFunction(line, out int continuation))
                        try
                        {
                            //Get the entire call mehtod "CALL --- ---"
                            string[] data = ExtractCallFrom(continuation, line);

                            //Get what you are calling, and the value
                            string callingTarget = data[1];

                            //Check for any of these keywords
                            foreach (string keyword in Keywords)
                            {

                                //TODO: Have it where you iterate through all prompts,
                                //But you find out which one is the parent prompt, so it doesn't hit into 
                                //One of the options.

                                //If you are calling to show the options from a prompt
                                if (callingTarget == GetKeyWord("PROMPT"))
                                {
                                    //Get what prompt number it is, and disply choices
                                    //We expect only 1 value, which is a number;
                                    int promptNumber = Convert.ToInt32(data[2]);

                                    //Get the prompt being called
                                    Prompt targetPrompt = GetDefinedPrompt(promptNumber);

                                    if (calledPrompt == null) calledPrompt = targetPrompt;

                                    targetPrompt.SetCallingLine(position);

                                    PromptStack.Push(targetPrompt);

                                    //TODO: Enhance the syling portion of DSL, especially for choices.
                                    //TODO: When prompt is called, have it search for OUT.
                                    //OUT will find either @, or if it can't find any, dialogue set ends

                                    callMade = true;
                                    break;
                                }
                            }
                        }
                        catch { }
                    else if (callMade == false)
                    {
                        _currentPrompt = null;
                        return FAILURE;
                    }
                    #endregion

                    // If an OUT occurs
                    #region PROMPT OUT CALL
                    if (Has(line, GetKeyWord("OUT")) && PromptStack.StackedPrompts.Count != 0)
                    {
                        //Get the prompt from the stack, and find the next dialogue avaliable
                        parentPrompt = PromptStack.Pop();
                        dontSearch = false;
                    }
                    #endregion
                }
                #endregion

                position++;
            }
            _currentPrompt = null;
            return FAILURE;
        }

        public static bool Break(int _position)
        {
            int position = 0;
            foreach (string line in CompiledData)
            {
                if (position >= _position)
                {
                    if (Has(line, GetKeyWord("BREAK")))
                    {
                        DialogueSystem.ReadyToBreak = true;
                        return true;
                    }

                    return false;
                }
                position++;
            }

            return false;
        }

        /// <summary>
        /// Take the collected dialogue, and find the position in compiled data.
        /// </summary>
        /// <param name="_line"></param>
        /// <returns></returns>
        public static int ConvertToDRV(string _line)
        {
            int position = 0;

            foreach (string dialogue in DialogueSystem.DialogueData)
            {
                if (dialogue == _line)
                    return DialogueReferenceValue[position];

                position++;
            }

            return -1;
        }

        /// <summary>
        /// You're able to retrieve all except a single character
        /// </summary>
        /// <param name="_character"></param>
        /// <returns></returns>
        public static char[] ExcludeDelimiters(params char[] _character)
        {
            //Have a list to hold the modded delimiters
            List<char> returnedDelimiters = new List<char>();

            //The first foreach is to iterate through our params char
            foreach (char removeCharacter in _character)
            {
                //This second foreach is the actual Delimiters
                foreach (char character in Delimiters)
                {
                    //If we don't see the character we want to remove...
                    //we add that character into our list.
                    //That way, we have the character that we want seperated from the characters we don't want to return.
                    if (removeCharacter != character)
                        returnedDelimiters.Add(character);
                }
            }

            //Return the delimiters that we want to use.
            return returnedDelimiters.ToArray();
        }

        /// <summary>
        /// Extract CALL method in .dsl file
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_line"></param>
        /// <returns></returns>
        public static string[] ExtractCallFrom(int _position, string _line)
        {

            string callMethod = STRINGNULL;

            for (int pos = 0; pos < _line.Length; pos++)
                try { callMethod = _line.Substring(_position, pos); } catch { };

            return callMethod.Split(' ');
        }

        public static string[] ExtractCaseOptionFrom(int _position, string _line)
        {
            string caseOption = STRINGNULL;

            for (int pos = 0; pos < _line.Length; pos++)
                try { caseOption = _line.Substring(_position, pos); } catch { };

            return caseOption.Split(' ');
        }

        /// <summary>
        /// Checks if string contains another string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Has(string line, string token) => line.Contains(token);
    }
}