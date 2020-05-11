using System.Collections.Generic;
using UnityEngine;

namespace DSL
{
    public class Prompt
    {
        public List<Option> Options { get; private set; } = new List<Option>();

        public int Number { get; private set; }

        public int Capacity { get; private set; }

        public int DialogueIndexReference { get; private set; }

        public const int DEFAULT_CAPACITY = 4;

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
    }
}
