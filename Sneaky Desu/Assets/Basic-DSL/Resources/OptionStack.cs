using System.Collections.Generic;

namespace DSL.PromptOptionCase
{
    public class OptionStack
    {
        public static Queue<Option> StackedOptions = new Queue<Option>(500);

        public static void Enqueue(Option _option)
        {
            StackedOptions.Enqueue(_option);
        }

        public static Option Dequeue() => StackedOptions.Dequeue();
    }
}
