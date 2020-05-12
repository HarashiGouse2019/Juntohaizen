using System.Collections.Generic;

namespace DSL.PromptOptionCase
{
    public class OptionStack
    {
        public static Stack<Option> StackedOptions = new Stack<Option>(500);

        public static void Push(Option _option)
        {
            StackedOptions.Push(_option);
        }

        public static Option Pop() => StackedOptions.Pop();
    }
}
