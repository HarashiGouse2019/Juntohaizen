using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace DSL
{
    namespace DSLParser
    {
        public class UnknownCharacterDefinedException : IOException
        {
            public UnknownCharacterDefinedException() { }
            public UnknownCharacterDefinedException(string message) : base(message) { }
            public UnknownCharacterDefinedException(string message, Exception inner) : base(message, inner) { }
        }

        public class DialogueSystemParser
        {
            public class CommandCallLocation
            {
                public string Command { get; private set; } = "";
                public int CallPosition { get; private set; } = -1;

                public int CallLine { get; private set; } = -1;

                public int DialogueSetLocation { get; private set; } = -1;

                private CommandCallLocation(string command, int dialogueSetLocation, int callLine, int callPosition)
                {
                    Command = command;

                    DialogueSetLocation = dialogueSetLocation;

                    CallLine = callLine;

                    CallPosition = callPosition;
                }

                public static CommandCallLocation New(string command, int dialogueSetLocation, int callLine, int callPosition)
                    => new CommandCallLocation(command, dialogueSetLocation, callLine, callPosition);

                public object[] GetCallLocation()
                {
                    object[] data =
                    {
                    DialogueSetLocation,
                    CallLine,
                    CallPosition
                };


                    return data;
                }
            }

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
                "GRID"
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
            const string END = "<END>";

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

                        if (startingBracketPos == 0)
                        {
                            DialogueSystem.ShiftCursorPosition(value);
                        }

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
            /// Define expressions listed underneath <EXPRESSIONS> in the .dsl file
            /// </summary>
            public static void DefineExpressions()
            {
                try
                {
                    string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                    string line = null;

                    int position = 0;

                    bool foundExpression = false;

                    if (File.Exists(dsPath))
                    {
                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == END && foundExpression)
                                    return;

                                if (Has(line, "EXPRESSIONS"))
                                {
                                    foundExpression = true;
                                    try { GetExpressions(position); } catch { }
                                }

                                position++;
                            }
                        }
                    }
                    Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
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

                    string line = null;

                    int position = 0;

                    bool foundPose = false;

                    if (File.Exists(dsPath))
                    {
                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == END && foundPose)
                                    return;

                                if (Has(line, "POSES"))
                                {
                                    foundPose = true;
                                    try { GetPoses(position); } catch { }
                                }

                                position++;
                            }
                        }
                    }
                    Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
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

                    string line = null;

                    int position = 0;

                    bool foundCharacter = false;

                    if (File.Exists(dsPath))
                    {
                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == END && foundCharacter)
                                    return;

                                if (Has(line, "CHARACTERS"))
                                {
                                    foundCharacter = true;
                                    try { GetCharacterNames(position); } catch { }
                                }

                                position++;
                            }
                        }
                    }
                    Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
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

                    string line = null;

                    int position = 0;

                    bool foundKeyCode = false;

                    if (File.Exists(dsPath))
                    {
                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == END && foundKeyCode)
                                    return;

                                if (Has(line, "KEYCODES"))
                                {
                                    string[] expressions = null;
                                    try { expressions = line.Replace(WHITESPACE, STRINGNULL).Split(Delimiters); } catch { }

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

                    string line = null;

                    int position = 0;

                    bool foundPrompt = false;

                    if (File.Exists(dsPath))
                    {
                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == STRINGNULL && foundPrompt)
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
                                        try { dataSet = line.Trim(Delimiters[0], Delimiters[1]).Split(Delimiters[5]); }
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
                    }
                    Debug.LogError("File specified doesn't exist. Try creating one in StreamingAssets folder.");
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

                            if (line == STRINGNULL && atTargetLine)
                                return;


                            if (position > _position)
                            {
                                atTargetLine = true;

                                if (line != STRINGNULL)
                                {
                                    string[] data = line.Split('=');
                                    DefinedExpressions.Add(data[0].Replace(WHITESPACE, STRINGNULL), Convert.ToInt32(data[1].Replace(WHITESPACE, STRINGNULL)));
                                }
                            }

                            position++;
                        }
                    }
                }
            }

            /// <summary>
            /// Get poses defined underneath <POSES> inside the .dsl file.
            /// </summary>
            /// <param name="_position">Where to start collecting data.</param>
            static void GetPoses(int _position)
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

                            if (atTargetLine)
                            {
                                if (line == STRINGNULL)
                                    return;
                            }


                            if (position > _position)
                            {
                                atTargetLine = true;

                                if (line != STRINGNULL)
                                {
                                    string[] data = line.Split('=');
                                    DefinedPoses.Add(data[0].Replace(WHITESPACE, STRINGNULL), Convert.ToInt32(data[1].Replace(WHITESPACE, STRINGNULL)));
                                }

                            }

                            position++;
                        }


                    }
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

                            if (atTargetLine)
                            {
                                if (line == STRINGNULL)
                                    return;
                            }

                            if (position > _position)
                            {
                                atTargetLine = true;

                                if (line != STRINGNULL)
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
                                    RetrieveKeyCodeAndFunctionality(line, ref declaration, ref keyCodeValue, ref functionalityValue, ref multiWord);

                                    // Parse string keyCodeValue to actual KeyCode enumerator value
                                    newKeyCode = ParseStringToKeyCode(keyCodeValue, newKeyCode, multiWord);

                                    // Retrieve name of input
                                    inputName = GetInputNameFromDSL(line, inputName);

                                    // Retrieve descriptive name of input
                                    inputDescriptiveName = GetDescriptiveNameFromDSL(line, inputDescriptiveName);

                                    // Register new input
                                    InvokeRegistrationToInputManager(keyCodeValue, functionalityValue, inputName, inputDescriptiveName, newKeyCode);
                                }
                            }
                            position++;
                        }
                    }
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
                    if (Has(keyCodeValue, "_"))
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
                string line = null;

                //Toggle if we are at a desired position in the file.
                bool atTargetLine = false;

                //If the defined path exist, interate through the file
                if (File.Exists(dsPath))
                {
                    //Stream reader allows use to read our file without compromising too much performance.
                    using (StreamReader fileReader = new StreamReader(dsPath))
                    {
                        //We use this to keep track of our position in the file
                        int position = 0;

                        //Loop until end of file
                        while (true)
                        {
                            //Read the current line
                            line = fileReader.ReadLine();

                            line = line.Trim('\t', ' '); //Remove all tabs and spaces, so that we can always get to @

                            //If we reach the end of the dialogue set, we are done reading it
                            if (line == "<END>" && atTargetLine)
                                return;

                            //However, if we are at the DialogueSet tag with a specified number, we collect all the dialogue that starts with "@"
                            if (position > _position)
                            {
                                atTargetLine = true; //Toggle on

                                //Make sure that we are specifically encountering "<...>"
                                if (line != STRINGNULL && (line[0].ToString() == Tokens[3] && line[line.Length - 1] == '<'))
                                {
                                    //We should have a list of defined characters with the DefinedCharacters() function. This is used to validate that that character exists
                                    if (DefinedCharacters.Count != 0)
                                    {
                                        //We iterate through our DefinedCharacters
                                        foreach (string character in DefinedCharacters)
                                        {
                                            string name = STRINGNULL;

                                            //We make an attempt to get the whole name after "@", then we check
                                            try
                                            {
                                                name = line.Substring(1, character.Length) + ":";
                                            }
                                            catch { }

                                            //If this character exist in the list of characters defined, we do some string manipulation
                                            if (Has(name, character))
                                            {
                                                //For names with _ scores replacing as spaces
                                                name = name.Replace("_", WHITESPACE);

                                                //Insert the name that's been defined using the Insert command
                                                line = line.Replace(Tokens[0], STRINGNULL).Replace(Tokens[3] + character, "[INSERT::\"" + name + "\"]");


                                                break;
                                            }

                                            //If it has ???, a predefined token that a character's name is not known, we insert it.
                                            else if (Has(name.Substring(0, Tokens[4].Length), Tokens[4]))
                                            {
                                                line = line.Replace(Tokens[0], STRINGNULL).Replace(Tokens[3] + Tokens[4], "[INSERT::\"" + Tokens[4] + ":" + "\"]");
                                                break;
                                            }

                                            //If there's no character or no ??? token, this means the narrator is speaking.
                                            else if (Has(name.Substring(0, WHITESPACE.Length), WHITESPACE))
                                            {
                                                line = line.Replace(Tokens[0], STRINGNULL).Replace(Tokens[3] + WHITESPACE, STRINGNULL);
                                                break;
                                            }

                                            //Then we really check if our defined character exist. If we down, we throw an exception, and end the dialogue reading process.
                                            else if (!DefinedCharacters.Exists(x => Has(x, line.Substring(1, line.IndexOf(WHITESPACE) - 1))))
                                            {
                                                string unidentifiedName = line.Substring(1, line.IndexOf(WHITESPACE) - 1);
                                                throw new UnknownCharacterDefinedException("Unknown character definition at line " + (position + 1) + ". Did you define \"" + unidentifiedName + "\" under <CHARACTERS>?");
                                            }
                                        }
                                    }
                                    else
                                        line = line.Replace(Tokens[0], STRINGNULL).Replace(Tokens[3] + WHITESPACE, STRINGNULL);

                                    DialogueSystem.Dialogue.Add(line);

                                }
                            }
                            position++;
                        }
                    }
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

            public static void GetOptions(int _position, int _number, int _capacity, ref List<Option> _options)
            {
                _options = new List<Option>();

                string dsPath = Application.streamingAssetsPath + @"/" + DialogueSystem.GET_DIALOGUE_SCRIPTING_FILE();

                string line = null;

                bool atTargetLine = false;

                int position = 0;

                try
                {
                    //We'll be needing a list of Options for this one

                    if (File.Exists(dsPath))
                    {
                        //We now create a new prompt, but check if the number of OPTIONS doesn't exceed CAPACITY
                        Option newOption = null;

                        using (StreamReader fileReader = new StreamReader(dsPath))
                        {
                            while (true)
                            {
                                line = fileReader.ReadLine();

                                line = line.Trim('\t', ' ');

                                if (line == "<END>" && atTargetLine)
                                {
                                    if (Prompt.ValidateCapacity(_capacity, _options.Count))
                                        return;
                                    else throw new ExceedsCapacityException("The number of options in Prompt " + _number + " exceeds its capacity of " + _capacity + ": Line " + position);
                                }

                                if (position > _position)
                                {
                                    atTargetLine = true;

                                    //If OPTION is defined before the numerical representation
                                    if (Has(line, GetKeyWord("OPTION")))
                                    {
                                        //We have reached the point where we can add our new options for creating a prompt
                                        //Options are shown as OPTION 1 >> Do a thing.
                                        string[] optionData = line.Replace(Keywords[18], STRINGNULL).Split('>');

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
                    }
                }
                catch (ExceedsCapacityException e){ Debug.LogError(e.Message); }
                _options = null;
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
                    _line = _line.Replace(_styleCommand + WHITESPACE, "<i>");
                    return SUCCESSFUL;
                }

                else if (_styleCommand == Delimiters[2] + Keywords[2] + Tokens[1] + Tokens[2] + Delimiters[3])
                {
                    _line = _line.Replace(_styleCommand + WHITESPACE, "</i>");
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
                    _line = _line.Replace(_styleCommand + WHITESPACE, "<u>");
                    return SUCCESSFUL;
                }

                else if (_styleCommand == Delimiters[2] + Keywords[3] + Tokens[1] + Tokens[2] + Delimiters[3])
                {
                    _line = _line.Replace(_styleCommand + WHITESPACE, "</u>");
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
        }
    }
}