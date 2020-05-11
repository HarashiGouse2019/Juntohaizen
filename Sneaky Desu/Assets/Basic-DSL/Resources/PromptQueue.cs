using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSL.PromptOptionCase
{
    
    public class PromptQueue
    {
        public static Queue<Prompt> QueuedPrompts = new Queue<Prompt>(50);

        public static void QueuePrompt(Prompt _prompt)
        {
            QueuedPrompts.Enqueue(_prompt);
        }

        public static Prompt DequeuePrompt() => QueuedPrompts.Dequeue();

        /// <summary>
        /// Calls the prompt that is queued
        /// </summary>
        /// <param name="_prompt"></param>
        public static void CallPrompt(Prompt _prompt)
        {
            _prompt = QueuedPrompts.Dequeue();

            /*We want to create buttons based on the total amount of options from
             a referenced, and show the options*/
            _prompt.Show();
        }
    } 
}
