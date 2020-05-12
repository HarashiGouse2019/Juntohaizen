using System;

namespace DSL
{
    public class UnknownSpriteChangerException : Exception
    {
        public UnknownSpriteChangerException() { }
        public UnknownSpriteChangerException(string message) : base(message) { }
        public UnknownSpriteChangerException(string message, Exception inner) : base(message, inner) { }
    } 
}