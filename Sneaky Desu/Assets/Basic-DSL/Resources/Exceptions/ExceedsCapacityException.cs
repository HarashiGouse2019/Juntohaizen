using System;

namespace DSL
{
    //Exceeds Capacity Exception
    public class ExceedsCapacityException : Exception
    {
        public ExceedsCapacityException() { }
        public ExceedsCapacityException(string message) : base(message) { }
        public ExceedsCapacityException(string message, Exception inner) : base(message, inner) { }
    }
}
