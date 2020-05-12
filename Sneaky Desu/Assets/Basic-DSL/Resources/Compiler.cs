using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace DSL.Core
{
    public class Compiler : MonoBehaviour
    {
        public static List<string> CompiledData { get; private set; } = new List<string>();

        public Compiler(string _source)
        {
            _source = Application.streamingAssetsPath + @"/" + _source;
            string line = null;
            using (StreamReader fileReader = new StreamReader(_source))
            {
                while (!fileReader.EndOfStream)
                {
                    line = fileReader.ReadLine();
                    CompiledData.Add(line);
                }
            }
        }
    }

}