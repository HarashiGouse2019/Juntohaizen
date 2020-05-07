using System;
using System.IO;

namespace DSL
{
    //NotInCorrectLayer Exception
    public class InvalidLayerException : Exception
    {
        public InvalidLayerException() { }
        public InvalidLayerException(string message) : base(message) { }
        public InvalidLayerException(string message, Exception inner) : base(message, inner) { }
    }
}