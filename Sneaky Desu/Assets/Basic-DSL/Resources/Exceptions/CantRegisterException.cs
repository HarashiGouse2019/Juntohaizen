using System;

namespace DSL
{
    public class CantRegisterException : Exception
    {
        public CantRegisterException() { }
        public CantRegisterException(string message) : base(message) { }
        public CantRegisterException(string message, Exception inner) : base(message, inner) { }
    } 
}
