using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using DSL.Core;
using DSL.InputManagement;
using DSL.PromptOptionCase;

namespace DSL.Parser
{
    public static class DialogueSystemParser
    {

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
                "SPEED",
                "BOLD",
                "ITALIZE",
                "UNDERLINE",
                "SOUND",
                "EXPRESSION",
                "ACTION",
                "HALT",
                "POSE",
                "INSERT",
                "AUTO",
                "DONT_DISTURB",
                "CAPACITY",
                "CONTROLED_BY_INPUT_MANAGER",
                "JUMP",
                "MAX",
                "MIN",
                "PROMPT",
                "OPTION",
                "FORMAT",
                "TYPE",
                "LIST",
                "GRID",
                "CALL",
                "OMIT",
                "PARENT",
                "CASE",
                "BREAK",
                "OUT"
                };

        public static string[] ValidTextSpeeds { get; } = {
                "SLOWEST",
                "SLOWER",
                "SLOW",
                "NORMAL",
                "FAST",
                "FASTER",
                "FASTEST" };

        public static List<CommandCallLocation> commandCallLocations = new List<CommandCallLocation>();

        public static Dictionary<string, int> DefinedExpressions { get; private set; } = new Dictionary<string, int>();

        public static Dictionary<string, int> DefinedPoses { get; private set; } = new Dictionary<string, int>();

        public static List<string> DefinedCharacters { get; private set; } = new List<string>();

        public static List<Prompt> DefinedPrompts { get; private set; } = new List<Prompt>();

        public static int skipValue = 0;

        public static object returnedValue = null;

        //Using this to bitmask data for reading if using Input Manager, and if using DSL's or Unity's
        public static bool Is_Using_Input_Manager = false;

        const bool SUCCESSFUL = true;
        const bool FAILURE = false;

        const string STRINGNULL = "";
        const string WHITESPACE = " ";
        const string UNDERSCORE = "_";
        const string TAB = "\t";

        const string END = "<END>";

        static DialogueSystemParser()
        {
            new Compiler(DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE());
        }

        /// <summary>
        /// Parse an entire line into tags based on commands calls.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns></returns>
        public static string ParseLine(string line)
        {
            /*The parsering process in the scripting language will be the most challenging thing!!! So challenging that
             I'm not even scared about the whole hand-drawn animations! I'm more scared of getting this parser system completed.
            Here's what I want to do, 


            We will most definitely be passing in a line in order to take each detected command off from that line, and 
            pass it back so that it can be displayed!

            We'll need 2 integers; one for the start of [, the other is the start of ], removing it from the string and
            adding it to our list of commands to execute. We should then take what's in between [] and use the Split method.
            (SPEED::SLOW) would be split into SPEED, ::, and SLOW. We check the 3rd element after we split, and check that value.

            This is just for implementing one command! We'll be needing to implement all of the other commands.
             */
            List<string> foundCommands = new List<string>();

            int startingBracketPos = 0;

            for (int value = 0; value < line.Length; value++)
            {
                //Now, how will we get the position of [ and ]?
                if (line[value] == Delimiters[2])
                {
                    startingBracketPos = value;
                }

                if (line[value] == Delimiters[3])
                {


                    /*At this point, we want to see if a command is actually
                     in between the brackets. If there is, then we can remove
                     from the starting point to the end point, and add our new
                     string to our found commands list.*/

                    string command = line.Substring(startingBracketPos, (value - startingBracketPos) + 1);

                    DialogueSystem.ShiftCursorPosition(value);

                    /*Now we have to see if it contains one of the commands.*/
                    foreach (string keyword in Keywords)
                    {
                        //If the parser command is a value one, we can remove it.
                        //This will allow the person
                        if (Has(command, keyword))
                        {
                            foundCommands.Add(command);

                            CommandCallLocation newCall =
                                CommandCallLocation.New(command, DialogueSystem.DialogueSet, (int)DialogueSystem.LineIndex, startingBracketPos);

                            commandCallLocations.Add(newCall);
                        }
                    }
                }
            }

            foreach (string commands in foundCommands)
            {
                /*For stuff like [BOLD] and [ITALIZE], and [UNDERLINE], we want to replace those with
                 <b>, <i>, and <u>*/

                bool tagsParser =
                    ParseToBoldTag(commands, ref line) ||
                    ParseToItalizeTag(commands, ref line) ||
                    ParseToUnderlineTag(commands, ref line) ||
                    ParseToSpeedTag(commands, ref line) ||
                    ParseToActionTag(commands, ref line) ||
                    ParserToInsertTag(commands, ref line) ||
                    ParseToExpressionTag(commands, ref line) ||
                    ParseToPoseTag(commands, ref line) ||
                    ParseToHaltag(commands, ref line);

                if (tagsParser != SUCCESSFUL)
                    line = line.Replace(commands + " ", "");
            }
            /*We finally got it to work!!!*/

            return line;
        }

        /// <summary>
        /// Defines all values that are typed explicitly in the .dsl file
        /// </summary>
        public static void DefineValues()
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
                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string dataLine = null;

                int position = 0;

                bool foundExpression = false;

                foreach (string line in Compiler.CompiledData)
                {

                    dataLine = line.Trim('\t', ' ');

                    if (dataLine == END && foundExpression)
                        return;

                    if (Has(dataLine, "EXPRESSIONS"))
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
                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string dataLine = null;

                int position = 0;

                bool foundPose = false;


                foreach (string line in Compiler.CompiledData)
                {
                    dataLine = line.Trim('\t', ' ');

                    if (dataLine == END && foundPose)
                        return;

                    if (Has(dataLine, "POSES"))
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
                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string dataLine = null;

                int position = 0;

                bool foundCharacter = false;

                foreach (string line in Compiler.CompiledData)
                {

                    dataLine = line.Trim('\t', ' ');

                    if (dataLine == END && foundCharacter)
                        return;

                    if (Has(dataLine, "CHARACTERS"))
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
                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string dataLine = null;

                int position = 0;

                bool foundKeyCode = false;



                foreach (string line in Compiler.CompiledData)
                {
                    dataLine = line.Trim('\t', ' ');

                    if (line == END && foundKeyCode)
                        return;

                    if (Has(dataLine, "KEYCODES"))
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
                Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
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
                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string dataLine = null;

                int position = 0;

                bool foundPrompt = false;

                foreach (string line in Compiler.CompiledData)
                {

                    dataLine = line.Trim('\t', ' ');

                    if (dataLine == END && foundPrompt)
                        return;

                    if (Has(line, "<PROMPT"))
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
                                    if (keyword == GetKeyWord("CAPACITY"))
                                        foreach (string token in Tokens)
                                        {
                                            if (token == Tokens[5] || token == Tokens[6])
                                                promptCapacity = Convert.ToInt32(dataSet[1].Split(Tokens[6].ToCharArray())[2]);
                                        }

                                    //IF FORMAT is defined
                                    if (keyword == GetKeyWord("FORMAT"))
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
            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            string dataLine = null;

            bool atTargetLine = false;

            int position = 0;

            foreach (string line in Compiler.CompiledData)
            {
                dataLine = line.Trim('\t', ' ');

                if (dataLine == END && atTargetLine)
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
            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            string dataLine = null;

            bool atTargetLine = false;

            int position = 0;

            foreach (string line in Compiler.CompiledData)
            {
                dataLine = line.Trim('\t', ' ');

                if (line == STRINGNULL && atTargetLine)
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
            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            string line = null;

            bool atTargetLine = false;

            if (File.Exists(dsPath))
            {
                using (StreamReader fileReader = new StreamReader(dsPath))
                {
                    int position = 0;

                    while (true)
                    {
                        line = fileReader.ReadLine();

                        line = line.Trim('\t', ' ');

                        if (atTargetLine)
                        {
                            if (line == STRINGNULL)
                                return;
                        }


                        if (position > _position)
                        {
                            atTargetLine = true;

                            if (line != STRINGNULL)
                                DefinedCharacters.Add(line);

                        }

                        position++;
                    }
                }
            }
        }

        /// <summary>
        /// Get Key Codes underneath inside the .dsl file
        /// </summary>
        /// <param name="_position"></param>
        static void GetKeyCodes(int _position)
        {
            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            string dataLine = null;
            int position = 0;
            bool atTargetLine = false;


            foreach (string line in Compiler.CompiledData)
            {
                dataLine = line.Trim('\t', ' ');

                if (dataLine == STRINGNULL && atTargetLine)
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

                        // Retrieve KeyCodeValue and FunctionalityValue as strings
                        RetrieveKeyCodeAndFunctionality(dataLine, ref declaration, ref keyCodeValue, ref functionalityValue, ref multiWord);

                        // Parse string keyCodeValue to actual KeyCode enumerator value
                        newKeyCode = ParseStringToKeyCode(keyCodeValue, newKeyCode, multiWord);

                        // Retrieve name of input
                        inputName = GetInputNameFromDSL(dataLine, inputName);

                        // Retrieve descriptive name of input
                        inputDescriptiveName = GetDescriptiveNameFromDSL(dataLine, inputDescriptiveName);

                        // Register new input
                        InvokeRegistrationToInputManager(keyCodeValue, functionalityValue, inputName, inputDescriptiveName, newKeyCode);
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
        private static KeyCode ParseStringToKeyCode(string keyCodeValue, KeyCode newKeyCode, bool multiWord)
        {
            try
            {
                //Parse string into KeyCode
                //If it's something like LeftArrow, it has already
                //set to the right format, so don't Capticalize
                newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), multiWord ? keyCodeValue : Capitalize(keyCodeValue));
            }
            catch { }

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
                    keyCodeValue = PascalCase(keyCodeValue);
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
            //Access the DSL Path
            string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

            //This is used to read a line in the file when iterating
            string dataLine = null;

            //Toggle if we are at a desired position in the file.
            bool atTargetLine = false;

            Prompt latestPrompt = null;

            /*Before we step into the field of hardship, we gotta set a couple of things:
             First of all, sinze there's only going to be 1 Call Prompt thing after a dialogue,
             you'll look for that specific one.

             Next, we split with "<<" and get the 2nd element.
             The 2nd element should then be "CALL PROMPT 0"

             We go it with the 2nd element and make a new string "promptCall = line.Split('<')[2]

             promptCall will then be splitted into string[] to take in "CALL" "PROMPT" and "0"

             CALL will be a function that displays the prompts options, which is something we'll have to figure out along the way.
             Not only that, but the nesting part of the program; we need to GetPromptByNumber, and SetLineReference to the current lin*/

            int position = 0;

            foreach (string line in Compiler.CompiledData)
            {

                dataLine = line.Trim('\t', ' '); //Remove all tabs and spaces, so that we can always get to @

                //If we reach the end of the dialogue set, we are done reading it
                if (dataLine == END && atTargetLine)
                {
                    return;
                }

                //However, if we are at the DialogueSet tag with a specified number, we collect all the dialogue that starts with "@"
                if (position > _position)
                {
                    atTargetLine = true; //Toggle on


                    //Collecting Dialogue
                    #region Dialogue Collection
                    //Make sure that we are specifically encountering "<...>"
                    if (dataLine != STRINGNULL && Validater.ValidateLineEndOperartor(dataLine))
                    {
                        //Validate the use of a character
                        dataLine = Validater.ValidateCharacterUsage(dataLine, position);

                        #region CALL PROMPT
                        if (Validater.ValidateCallFunction(dataLine, out int continuation))
                        {
                            //Get the entire call mehtod "CALL --- ---"
                            string[] data = ExtractCallFrom(continuation, dataLine);

                            //Get what you are calling, and the value
                            string callingTarget = data[1];

                            //Check for any of these keywords
                            foreach (string keyword in Keywords)
                            {

                                //If you are calling to show the options from a prompt
                                if (callingTarget == GetKeyWord("PROMPT"))
                                {
                                    //Get what prompt number it is, and disply choices
                                    //We expect only 1 value, which is a number;
                                    int promptNumber = Convert.ToInt32(data[2]);

                                    //Get the prompt being called
                                    Prompt targetPrompt = GetDefinedPrompt(promptNumber);

                                    targetPrompt.SetCallingLine(position + 1);

                                    PromptStack.Push(targetPrompt);

                                    //TODO: Enhance the syling portion of DSL, especially for choices.
                                    //TODO: When prompt is called, have it search for OUT.
                                    //OUT will find either @, or if it can't find any, dialogue set ends



                                    break;
                                }

                            }
                        }
                        Dialogue newDialogue = new Dialogue(dataLine);
                        #endregion
                        DialogueSystem.DialogueList.Add(newDialogue);

                        if (latestPrompt != null)
                        {
                            //This would be the next avaliable dialogue for the prompt in the stack
                            latestPrompt.SetDialogueReference(dataLine);
                            latestPrompt.FindDialoguePosition();

                            latestPrompt = null;
                        }
                    }
                    #endregion

                    //If an OUT is sceen
                    #region PROMPT OUT CALL
                    if (line == GetKeyWord("OUT"))
                    {
                        //Get the prompt from the stack, and find the next dialogue avaliable
                        latestPrompt = PromptStack.Pop();
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
                foreach (string line in Compiler.CompiledData)
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

        /// <summary>
        /// Convert speed command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToSpeedTag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[0]))
            {
                var speedValue = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Split(':')[2];

                /*The Dialogue System's ChangeSpeed function used enumerators,
                 so we need to use the array that we have in the parser, and get there indexes*/
                foreach (string speed in ValidTextSpeeds)
                {
                    if (speedValue == speed)
                    {
                        _line = _line.Replace(_styleCommand, "<" + "sp=" + Array.IndexOf(ValidTextSpeeds, speed) + ">");
                        return SUCCESSFUL;
                    }
                }
            }
            return FAILURE;
        }

        /// <summary>
        /// Convert action command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToActionTag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[6]))
            {
                var actionString = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Split(':')[2].Trim('"');

                /*The action function is simply to add two asteriks between a word.
                 For example: [ACTION::"Sighs"] will be replaced by *Sigh* in the text. 

                 Very easy function to do.*/

                /*The skip tag will do the shift of the cursor for use one the system sees this
                 parsed information.*/
                _line = _line.Replace(_styleCommand, "<action=" + actionString + ">");

                return SUCCESSFUL;
            }
            return FAILURE;
        }

        /// <summary>
        /// Convert expression command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToExpressionTag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[5]))
            {
                var value = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Split(':')[2];

                /*The Expression Action is going to be a bit complicated.
                 We'll have to create a expression tag, and just have the expression we are looking for.
                 The expression will act exactly like skip, but this is to let the system know that we need
                 it to use the SpriteChanger, and change the image.*/

                /*The skip tag will do the shift of the cursor for use one the system sees this
                 parsed information.*/
                _line = _line.Replace(_styleCommand, "<exp=" + value + ">");

                return SUCCESSFUL;
            }
            return FAILURE;
        }

        /// <summary>
        /// Convert pose command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToPoseTag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[8]))
            {
                var value = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Split(':')[2];

                /*The Expression Action is going to be a bit complicated.
                 We'll have to create a expression tag, and just have the expression we are looking for.
                 The expression will act exactly like skip, but this is to let the system know that we need
                 it to use the SpriteChanger, and change the image.*/

                /*The skip tag will do the shift of the cursor for use one the system sees this
                 parsed information.*/
                _line = _line.Replace(_styleCommand, "<pose=" + value + ">");

                return SUCCESSFUL;
            }
            return FAILURE;
        }

        /// <summary>
        /// Convert insert command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParserToInsertTag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[9]))
            {
                var word = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Replace(Tokens[1], STRINGNULL).Split('"')[1];
                /*The action function is simply to add two asteriks between a word.
                 For example: [ACTION::"Sighs"] will be replaced by *Sigh* in the text. 

                 Very easy function to do.*/

                /*The skip tag will do the shift of the cursor for use one the system sees this
                 parsed information.*/
                _line = _line.Replace(_styleCommand, "<ins=" + word + ">");

                return SUCCESSFUL;
            }
            return FAILURE;
        }

        /// <summary>
        /// Convert halt command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToHaltag(string _styleCommand, ref string _line)
        {
            if (Has(_styleCommand, Delimiters[2] + Keywords[7]))
            {

                var value = _styleCommand.Trim(Delimiters[2], Delimiters[3]).Split(':')[2];

                /*The Wait should be easy enough. We'll be doing inserting a tag that
                  and then add in the number. At that point, the DialogueSystem will update
                  the textSpeed based on the duration. */

                _line = _line.Replace(_styleCommand, "<halt=" + value + ">");

                return SUCCESSFUL;
            }

            return FAILURE;
        }

        /// <summary>
        /// Convert bold command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToBoldTag(string _styleCommand, ref string _line)
        {
            if (_styleCommand == Delimiters[2] + Keywords[1] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<b>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Delimiters[2] + Keywords[1] + Tokens[1] + Tokens[2] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "</b>");
                return SUCCESSFUL;
            }

            return FAILURE;
        }

        /// <summary>
        /// Convert italize command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToItalizeTag(string _styleCommand, ref string _line)
        {
            if (_styleCommand == Delimiters[2] + Keywords[2] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<i>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Delimiters[2] + Keywords[2] + Tokens[1] + Tokens[2] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "</i>");
                return SUCCESSFUL;
            }

            return FAILURE;
        }

        /// <summary>
        /// Convert underline command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToUnderlineTag(string _styleCommand, ref string _line)
        {
            if (_styleCommand == Delimiters[2] + Keywords[3] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<u>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Delimiters[2] + Keywords[3] + Tokens[1] + Tokens[2] + Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "</u>");
                return SUCCESSFUL;
            }

            return FAILURE;
        }

        /// <summary>
        /// Captitalize the first letter of a word
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        static string Capitalize(string _word) => _word[0].ToString().ToUpper() + _word.Substring(1, _word.Length - 1).ToLower();

        /// <summary>
        /// Do PascalCase for any underscore values
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        static string PascalCase(string _word)
        {
            //Split with "_"
            string[] words = _word.Split('_');
            string pascalWord = null;

            //Capitalize each word, and add it to pascalWord
            foreach (string word in words)
            {
                pascalWord += Capitalize(word);
            }

            //And return
            return pascalWord;
        }

        /// <summary>
        /// Checks if string contains another string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Has(string line, string token) => line.Contains(token);

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
    }
}