using System;
using UnityEngine;
using PARSER = DSL.Parser.DialogueSystemParser;

namespace DSL.Core
{
    public class Validater
    {
        const bool SUCCESSFUL = true;
        const bool FAILURE = false;

        const string STRINGNULL = "";
        const string WHITESPACE = " ";
        const string UNDERSCORE = "_";
        const string TAB = "\t";

        public static string ValidateCharacterUsage(string _targetLine, int _linePosition)
        {
            #region Character Validation
            //We should have a list of defined characters with the DefinedCharacters() function. This is used to validate that that character exists
            if (Compiler.DefinedCharacters.Count != 0)
            {
                //We iterate through our DefinedCharacters
                foreach (string character in (Compiler.DefinedCharacters))
                {
                    string name = STRINGNULL;

                    //We make an attempt to get the whole name after "@", then we check
                    try
                    {
                        name = _targetLine.Substring(1, character.Length) + ":";

                        //If this character exist in the list of characters defined, we do some string manipulation
                        if ((Compiler.Has(name, character)))
                        {
                            //For names with _ scores replacing as spaces
                            name = name.Replace(UNDERSCORE, WHITESPACE);

                            //Insert the name that's been defined using the Insert command
                            _targetLine = _targetLine.Replace(Compiler.Tokens[0], STRINGNULL).Replace(Compiler.Tokens[3] + character, "[INSERT::\"" + name + "\"]");
                            break;
                        }

                        //If it has ???, a predefined token that a character's name is not known, we insert it.
                        else if (Compiler.Has(name.Substring(0, Compiler.Tokens[4].Length), Compiler.Tokens[4]))
                        {
                            _targetLine = _targetLine.Replace(Compiler.Tokens[0], STRINGNULL).Replace(Compiler.Tokens[3] + Compiler.Tokens[4], "[INSERT::\"" + Compiler.Tokens[4] + ":" + "\"]");
                            break;
                        }

                        //If there's no character or no ??? token, this means the narrator is speaking.
                        else if (Compiler.Has(name.Substring(0, WHITESPACE.Length), WHITESPACE))
                        {
                            _targetLine = _targetLine.Replace(Compiler.Tokens[0], STRINGNULL).Replace(Compiler.Tokens[3] + WHITESPACE, STRINGNULL);
                            break;
                        }

                        //Then we really check if our defined character exist. If we down, we throw an exception, and end the dialogue reading process.
                        else if (!Compiler.DefinedCharacters.Exists(x => Compiler.Has(x, _targetLine.Substring(1, _targetLine.IndexOf(WHITESPACE) - 1))))
                        {
                            string unidentifiedName = _targetLine.Substring(1, _targetLine.IndexOf(WHITESPACE) - 1);
                            throw new UnknownCharacterDefinedException("Unknown character definition at line " + (_linePosition + 1) + ". Did you define \"" + unidentifiedName + "\" under <CHARACTERS>?");
                        }
                    }
                    catch (UnknownCharacterDefinedException unknownCharacterException) { Debug.LogException(unknownCharacterException); break; }

                }
            }
            else
                _targetLine = _targetLine.Replace(Compiler.Tokens[0], STRINGNULL).Replace(Compiler.Tokens[3] + WHITESPACE, STRINGNULL);
            #endregion

            return _targetLine;
        }

        /// <summary>
        /// Validate if the Expression is defined in the .dsl file
        /// </summary>
        /// <param name="value"></param>
        /// <param name="_notFlag"></param>
        /// <returns></returns>
        public static bool ValidateExpressionsAndChange(string value, ref bool _notFlag)
        {
            //Check if a key matches
            string data = STRINGNULL;

            if (Compiler.DefinedExpressions.ContainsKey(value))
            {
                if (value.GetType() == typeof(string))
                {
                    data = DialogueSystem.FindKeyAndConvertToString(value, Compiler.DefinedExpressions);
                    _notFlag = false;
                }
            }

            else if (Compiler.DefinedExpressions.ContainsValue(Convert.ToInt32(value)))
            {
                if (Convert.ToInt32(value).GetType() == typeof(int))
                {

                    data = DialogueSystem.FindValueAndConvertToString(Convert.ToInt32(value), Compiler.DefinedExpressions);

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
            DialogueSystemSpriteChanger changer = DialogueSystem.Find_Sprite_Changer(characterName + "_" + changeType);

            changer.CHANGE_IMAGE(characterState);

            DialogueSystem.ShiftCursorPosition(-1);

            return SUCCESSFUL;
        }

        /// <summary>
        /// Validate if the Pose is defined in the .dsl file
        /// </summary>
        /// <param name="value"></param>
        /// <param name="_notFlag"></param>
        /// <returns></returns>
        public static bool ValidatePosesAndChange(string value, ref bool _notFlag)
        {
            //Check if a key matches
            string data = STRINGNULL;

            if (Compiler.DefinedExpressions.ContainsKey(value))
            {
                if (value.GetType() == typeof(string))
                {
                    data = DialogueSystem.FindKeyAndConvertToString(value, Compiler.DefinedPoses);
                    _notFlag = false;
                }
            }

            else if (Compiler.DefinedExpressions.ContainsValue(Convert.ToInt32(value)))
            {
                if (Convert.ToInt32(value).GetType() == typeof(int))
                {

                    data = DialogueSystem.FindValueAndConvertToString(Convert.ToInt32(value), Compiler.DefinedPoses);

                    _notFlag = false;
                }
            }

            if (_notFlag)
            {
                //Otherwise, we'll through an error saying this hasn't been defined.
                Debug.LogError(value + " has not been defined. Perhaps declaring it in the associated " + DialogueSystem.DSLFileExtention + " File.");
                return FAILURE;
            }

            //We get the name, keep if it's EXPRESSION or POSE, and the emotion value
            string characterName = data.Split('_')[0];
            string changeType = data.Split('_')[1];
            string characterState = data.Split('_')[2];

            Debug.Log(characterState);

            //Now we see if we can grab the image, and have it change...
            DialogueSystemSpriteChanger changer = DialogueSystem.Find_Sprite_Changer(characterName + "_" + changeType);

            changer.CHANGE_IMAGE(characterState);

            DialogueSystem.ShiftCursorPosition(-1);

            return SUCCESSFUL;
        }

        public static bool ValidateLineEndOperartor(string _targetLine)
        {
            //The operator "<<"
            string stopOperator = Compiler.Tokens[0];

            string stringScanningRange = null;
            
            for(int pos = 0; pos < _targetLine.Length; pos++)
            {
                try
                {
                    stringScanningRange = _targetLine.Substring(pos, stopOperator.Length);
                    if (stringScanningRange == stopOperator) return true;
                }catch { }
            }

            return false;
        }

        public static bool ValidateCallFunction(string _targetLine, out int _continuation)
        {
            //The operator "<<"
            string callMethod = Compiler.GetKeyWord("CALL");

            string stringScanningRange = null;

            for (int pos = 0; pos < _targetLine.Length; pos++)
            {
                try
                {
                    stringScanningRange = _targetLine.Substring(pos, callMethod.Length);

                    if (stringScanningRange == callMethod)
                    {
                        _continuation = pos;
                        return true;
                    }
                }
                catch { }
            }

            _continuation = -1;
            return false;
        }
    }
}
