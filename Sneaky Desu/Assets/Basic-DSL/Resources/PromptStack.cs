using System.Collections.Generic;

namespace DSL.PromptOptionCase
{
    public class PromptStack
    {
        public static Stack<Prompt> StackedPrompts = new Stack<Prompt>(20);

        public static void Push(Prompt _prompt)
        {
            StackedPrompts.Push(_prompt);
        }

        public static Prompt Pop() => StackedPrompts.Pop();

        /// <summary>
        /// Calls the prompt that is queued
        /// </summary>
        /// <param name="_prompt"></param>
        public static void CallPrompt(Prompt _prompt)
        {
            _prompt = StackedPrompts.Pop();

            /*We want to create buttons based on the total amount of options from
             a referenced, and show the options*/
            _prompt.Show();
        }
    } 
}
