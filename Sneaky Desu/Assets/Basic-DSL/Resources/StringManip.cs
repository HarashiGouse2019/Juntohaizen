using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DSL.Core;

namespace DSL.Text
{
    public class StringManip : MonoBehaviour
    {
        const string STRINGNULL = "";

        /// <summary>
        /// Captitalize the first letter of a word
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        public static string Capitalize(string _word) => _word[0].ToString().ToUpper() + _word.Substring(1, _word.Length - 1).ToLower();

        /// <summary>
        /// Do PascalCase for any underscore values
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        public static string PascalCase(string _word)
        {
            //Split with "_"
            string[] words = _word.Split('_');
            string pascalWord = null;

            //Capitalize each word, and add it to pascalWord
            foreach (string word in words)
            {
                pascalWord += Capitalize(word);
            }

            //And return
            return pascalWord;
        }
    }
}
