﻿using System;
using System.Collections.Generic;
using UnityEngine;
using DSL.Styling;
using DSL.Core;

namespace DSL.PromptOptionCase
{
    public class Prompt : IDialogueReference
    {
        public List<Option> Options { get; private set; } = new List<Option>();

        public int Number { get; private set; }

        public int Capacity { get; private set; }

        public string DialogueReference { get; set; }

        public int gotoLine { get; set; }

        public const int DEFAULT_CAPACITY = 4;

        public int CallingLine { get; private set; }

        public bool IsChoiceMade { get; private set; } = false;

        /// <summary>
        /// Create a new prompt
        /// </summary>
        /// <param name="_capacity"></param>
        /// <param name="_options"></param>
        public Prompt(int _number, List<Option> _options, int _capacity = DEFAULT_CAPACITY)
        {
            Capacity = _capacity;
            Number = _number;
            AddOptions(_options);
        }

        /// <summary>
        /// Add options to prompt
        /// </summary>
        /// <param name="_options"></param>
        void AddOptions(List<Option> _options)
        {
            //Create a new array of options
            Options = _options;
        }

        /// <summary>
        /// Validate the capacity of options for the given prompt
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="_numberOfOptions"></param>
        /// <returns></returns>
        public static bool ValidateCapacity(int _promptCapacity, int _numberOfOptions) => _numberOfOptions <= _promptCapacity;

        /// <summary>
        /// Show the options for this Prompt
        /// </summary>
        public void Show(ButtonStylerFormat _format = ButtonStylerFormat.LIST)
        {
            //Generate this prompts objects.
            Styler.GenerateOptions(_format, Options.ToArray());
        }

        public Option GetOption(int _optionID)
        {
            foreach (Option option in Options)
            {
                if (_optionID == option.ID) return option;
            }

            return null;
        }
        /// <summary>
        /// Set the calling level of the prompt being called in dialogue
        /// </summary>
        /// <param name="_value"></param>
        public void SetCallingLine(int _value) => CallingLine = _value;

        public void SetDialogueReference(string _dialogue) => DialogueReference = _dialogue;

        public int FindDialoguePosition()
        {
            int position = 0;

            foreach (string dialogue in DialogueSystem.DialogueData)
            {
                if (DialogueReference.Contains(dialogue))
                {
                    gotoLine = position;
                    return position;
                }
                position++;
            }

            return -1;
        }

        public int SelectOption(int _optionID)
        {
            foreach (Option option in Options)
            {
                if (_optionID == option.ID) { IsChoiceMade = true; Debug.Log(option.Content); return option.ID; }

            }

            return -1;
        }

        //It'll find the right Option ID, and count only @.
        //The count will be assigned to DialogueSystem steps variable
        public static void GetOptionResultContent(int _optionID, int _startPosition, Prompt _prompt)
        {
            string caseOptionKeywords = Compiler.GetKeyWord("CASE") + " " + Compiler.GetKeyWord("OPTION");

            int position = 0;

            int stepAmount = 0;

            Option latestOption = null;

            Option callOption = null;

            int? jumpTo = null;

            bool caseFound = false;

            foreach (string lineData in Compiler.CompiledData)
            {
                string line = lineData.Trim('\t', ' ');

                if (position >= _startPosition && line != "")
                {
                    if (caseFound && jumpTo == null && Validater.ValidateLineEndOperartor(line, out string _moddedLine))
                    {
                        string dialogueReference = Validater.ValidateCharacterUsage(_moddedLine, position);

                        callOption.SetDialogueReference(dialogueReference);

                        callOption.FindDialoguePosition();

                        jumpTo = callOption.gotoLine - 1;

                        DialogueSystem.JumpToLineIndex(jumpTo.GetValueOrDefault());
                    }

                    if (Validater.ValidateLineEndOperartor(line, out string _stepLine) && caseFound)
                        stepAmount++;

                    if (OptionStack.StackedOptions.Count == 0 && latestOption != null && latestOption == callOption)
                    {
                        DialogueSystem.steps = stepAmount;
                        DialogueSystem.SetPreviousAnsweredPrompt(_prompt);
                        return;
                    }

                    for (int pos = 0; pos < lineData.Length; pos++)
                    {
                        try
                        {
                            //Search for "CASE OPTION"
                            string caseOption = line.Substring(pos, caseOptionKeywords.Length);

                            if (caseOption == caseOptionKeywords)
                            {
                                string[] data = Compiler.ExtractCaseOptionFrom(pos, line);

                                int optionId = Convert.ToInt32(data[2]);

                                Option targetOption = Compiler.GetDefinedOption(optionId, _prompt);

                                if (targetOption.ID == _optionID && caseFound == false)
                                {
                                    callOption = targetOption;

                                    caseFound = true;
                                }

                                OptionStack.Push(targetOption);

                                break;
                            } 
                        }
                        catch { }
                    }
                }

                if (line == Compiler.GetKeyWord("BREAK") && OptionStack.StackedOptions.Count != 0)
                    latestOption = OptionStack.Pop();

                position++;
            }
        }
    }
}
