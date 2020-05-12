using System;

namespace DSL
{
    public class UnknownCharacterDefinedException : Exception
    {
        public UnknownCharacterDefinedException() { }
        public UnknownCharacterDefinedException(string message) : base(message) { }
        public UnknownCharacterDefinedException(string message, Exception inner) : base(message, inner) { }
    } 
}