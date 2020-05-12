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
    public class DialogueSystemParser
    {
        public static int skipValue = 0;

        public static object returnedValue = null;

        const bool SUCCESSFUL = true;
        const bool FAILURE = false;

        const string STRINGNULL = "";
        const string WHITESPACE = " ";
        const string UNDERSCORE = "_";
        const string TAB = "\t";

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
                if (line[value] == Compiler.Delimiters[2])
                {
                    startingBracketPos = value;
                }

                if (line[value] == Compiler.Delimiters[3])
                {


                    /*At this point, we want to see if a command is actually
                     in between the brackets. If there is, then we can remove
                     from the starting point to the end point, and add our new
                     string to our found commands list.*/

                    string command = line.Substring(startingBracketPos, (value - startingBracketPos) + 1);

                    DialogueSystem.ShiftCursorPosition(value);

                    /*Now we have to see if it contains one of the commands.*/
                    foreach (string keyword in Compiler.Keywords)
                    {


                        //If the parser command is a value one, we can remove it.
                        //This will allow the person
                        if (Compiler.Has(command, keyword))
                        {
                            foundCommands.Add(command);

                            CommandCallLocation newCall =
                                CommandCallLocation.New(command, DialogueSystem.DialogueSet, (int)DialogueSystem.LineIndex, startingBracketPos);

                            Compiler.commandCallLocations.Add(newCall);
                        }
                    }
                }
            }

            foreach (CommandCallLocation call in Compiler.commandCallLocations)
            {
                /*For stuff like [BOLD] and [ITALIZE], and [UNDERLINE], we want to replace those with
                 <b>, <i>, and <u>*/

                bool tagsParser =
                    ParseToBoldTag(call.Command, ref line) ||
                    ParseToItalizeTag(call.Command, ref line) ||
                    ParseToUnderlineTag(call.Command, ref line) ||
                    ParseToSpeedTag(call.Command, ref line) ||
                    ParseToActionTag(call.Command, ref line) ||
                    ParserToInsertTag(call.Command, ref line) ||
                    ParseToExpressionTag(call.Command, ref line) ||
                    ParseToPoseTag(call.Command, ref line) ||
                    ParseToHaltag(call.Command, ref line);

                if (tagsParser != SUCCESSFUL)
                    line = line.Replace(call.Command + " ", "");
            }


            /*We finally got it to work!!!*/

            return line;
        }

        /// <summary>
        /// Convert speed command into a tag.
        /// </summary>
        /// <param name="_styleCommand">The valid commands.</param>
        /// <param name="_line">The line to parse.</param>
        /// <returns></returns>
        static bool ParseToSpeedTag(string _styleCommand, ref string _line)
        {
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[0]))
            {
                var speedValue = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Split(':')[2];

                /*The Dialogue System's ChangeSpeed function used enumerators,
                 so we need to use the array that we have in the parser, and get there indexes*/
                foreach (string speed in Compiler.ValidTextSpeeds)
                {
                    if (speedValue == speed)
                    {
                        _line = _line.Replace(_styleCommand, "<" + "sp=" + Array.IndexOf(Compiler.ValidTextSpeeds, speed) + ">");
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
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[6]))
            {
                var actionString = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Split(':')[2].Trim('"');

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
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[5]))
            {
                var value = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Split(':')[2];

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
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[8]))
            {
                var value = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Split(':')[2];

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
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[9]))
            {
                var word = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Replace(Compiler.Tokens[1], STRINGNULL).Split('"')[1];
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
            if (Compiler.Has(_styleCommand, Compiler.Delimiters[2] + Compiler.Keywords[7]))
            {

                var value = _styleCommand.Trim(Compiler.Delimiters[2], Compiler.Delimiters[3]).Split(':')[2];

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
            if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[1] + Compiler.Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<b>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[1] + Compiler.Tokens[1] + Compiler.Tokens[2] + Compiler.Delimiters[3])
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
            if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[2] + Compiler.Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<i>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[2] + Compiler.Tokens[1] + Compiler.Tokens[2] + Compiler.Delimiters[3])
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
            if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[3] + Compiler.Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "<u>");
                return SUCCESSFUL;
            }

            else if (_styleCommand == Compiler.Delimiters[2] + Compiler.Keywords[3] + Compiler.Tokens[1] + Compiler.Tokens[2] + Compiler.Delimiters[3])
            {
                _line = _line.Replace(_styleCommand, "</u>");
                return SUCCESSFUL;
            }

            return FAILURE;
        }
    }
}