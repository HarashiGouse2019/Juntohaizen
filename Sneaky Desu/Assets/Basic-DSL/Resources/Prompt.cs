using System.Collections.Generic;
using UnityEngine;
using DSL.Styling;

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
            foreach(Option option in Options)
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

        public int FindDialoguePosition() {
            int position = 0;

            foreach(Dialogue dialogue in DialogueSystem.DialogueList)
            {
                if (DialogueReference.Contains(dialogue.Content))
                {
                    gotoLine = position;
                    return position;
                }
                    position++;
            }

            return -1;
        }

    }
}
