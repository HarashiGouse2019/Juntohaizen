using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;

namespace DSL.Core
{
    public class TokenType
    {
        //Token string
        public string pattern;

        //Regex expression
        Regex regex;

        //The type of the token
        public enum Type
        {
            NULL, //Nothing
            COM, //Commment
            OP, //Operator
            NUM, //Number
            ID, //Identifier
            RESERVED, //Reserved keyword
            STRING, //String value
            DIALOGUE //Dialogue
        }

        public Type type;

        //Constructor
        public TokenType(string pattern, Type type)
        {
            this.pattern = pattern;
            this.type = type;

            //New RegularExpression
            regex = new Regex(this.pattern);

        }

        public TokenMatch Match(string inputString)
        {

            var match = regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                    remainingText = inputString.Substring(match.Length);

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = type,
                    Value = match.Value
                };
            }
            else
            {
                return new TokenMatch() { IsMatch = false };
            }
        }
    }

    public class TokenMatch
    {
        public bool IsMatch { get; set; }
        public TokenType.Type TokenType { get; set; }
        public string Value { get; set; }
        public string RemainingText { get; set; }
    }

    public class DslToken
    {
        public TokenType.Type TokenType { get; set; }
        public string Value { get; set; }

        public DslToken(TokenType.Type tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public DslToken(TokenType.Type tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public DslToken Clone()
        {
            return new DslToken(TokenType, Value);
        }
    }


    public class Lexer : IDSLCoord
    {
        public int TotalLines { get; set; }
        public int Line { get; set; }
        public int Col { get; set; }
        public int Index { get; set; }

        //The data of tokens
        List<TokenType> tokens = new List<TokenType>();

        //String Builder to combine CollectedData
        public StringBuilder builder = new StringBuilder();

        //A set of regular expressions that we will use to validate tokens
        TokenType[] tokenExpressions =
        {
            new TokenType("^[ \n\t\r]", TokenType.Type.NULL),
            new TokenType("^is", TokenType.Type.RESERVED),
            new TokenType("^\\(+", TokenType.Type.OP),
            new TokenType("^\\)", TokenType.Type.OP),
            new TokenType("^\\+", TokenType.Type.OP),
            new TokenType("^\\-", TokenType.Type.OP),
            new TokenType("^\\*", TokenType.Type.OP),
            new TokenType("^\\'", TokenType.Type.OP),
            new TokenType("^=", TokenType.Type.OP),
            new TokenType("^<=", TokenType.Type.OP),
            new TokenType("^<", TokenType.Type.OP),
            new TokenType("^>=", TokenType.Type.OP),
            new TokenType("^>", TokenType.Type.OP),
            new TokenType("^!=", TokenType.Type.OP),
            new TokenType("^::", TokenType.Type.OP),
            new TokenType("^and", TokenType.Type.RESERVED),
            new TokenType("^or", TokenType.Type.RESERVED),
            new TokenType("^not", TokenType.Type.RESERVED),
            new TokenType("^if", TokenType.Type.RESERVED),
            new TokenType("^else", TokenType.Type.RESERVED),
            new TokenType("^while", TokenType.Type.RESERVED),
            new TokenType("^do", TokenType.Type.RESERVED),
            new TokenType("^end", TokenType.Type.RESERVED),
            new TokenType("^to", TokenType.Type.RESERVED),
            new TokenType("^from", TokenType.Type.RESERVED),
            new TokenType("^for", TokenType.Type.RESERVED),
            new TokenType("^foreach", TokenType.Type.RESERVED),
            new TokenType("^switch", TokenType.Type.RESERVED),
            new TokenType("^case", TokenType.Type.RESERVED),
            new TokenType("^var", TokenType.Type.RESERVED),
            new TokenType("^let", TokenType.Type.RESERVED),
            new TokenType("^run", TokenType.Type.RESERVED),
            new TokenType("^at", TokenType.Type.RESERVED),
            new TokenType("^as", TokenType.Type.RESERVED),
            new TokenType("^prepare", TokenType.Type.RESERVED),
            new TokenType("^call", TokenType.Type.RESERVED),
            new TokenType("^omit", TokenType.Type.RESERVED),
            new TokenType("^set", TokenType.Type.RESERVED),
            new TokenType("^get", TokenType.Type.RESERVED),
            new TokenType("^apply", TokenType.Type.RESERVED),
            new TokenType("^mod", TokenType.Type.RESERVED),
            new TokenType("^break", TokenType.Type.RESERVED),
            new TokenType("^out", TokenType.Type.RESERVED),
            new TokenType("^incrementive", TokenType.Type.RESERVED),
            new TokenType("^decrementive", TokenType.Type.RESERVED),
            new TokenType("^\\+\\+", TokenType.Type.OP),
            new TokenType("^--", TokenType.Type.OP),
            new TokenType("^using", TokenType.Type.RESERVED),
            new TokenType("^dontDetain", TokenType.Type.RESERVED),
            new TokenType("^allow", TokenType.Type.RESERVED),
            new TokenType("^true", TokenType.Type.RESERVED),
            new TokenType("^false", TokenType.Type.RESERVED),
            new TokenType("^src", TokenType.Type.RESERVED),
            new TokenType("^auto", TokenType.Type.RESERVED),
            new TokenType("^const", TokenType.Type.RESERVED),
            new TokenType("^try", TokenType.Type.RESERVED),
            new TokenType("^catch", TokenType.Type.RESERVED),
            new TokenType("^const_str", TokenType.Type.RESERVED),
            new TokenType("^const_num", TokenType.Type.RESERVED),
            new TokenType("^other", TokenType.Type.RESERVED),
            new TokenType("^parent", TokenType.Type.RESERVED),
            new TokenType("^request", TokenType.Type.RESERVED),
            new TokenType("^type", TokenType.Type.RESERVED),
            new TokenType("^release", TokenType.Type.RESERVED),
            new TokenType("^detain", TokenType.Type.RESERVED),
            new TokenType("^define", TokenType.Type.RESERVED),
            new TokenType("^start", TokenType.Type.RESERVED),
            new TokenType("^\\<", TokenType.Type.OP),
            new TokenType("^\\>", TokenType.Type.OP),
            new TokenType("^begin", TokenType.Type.RESERVED),
            new TokenType("^SMProxy", TokenType.Type.RESERVED),
            new TokenType("^title", TokenType.Type.RESERVED),
            new TokenType("^author", TokenType.Type.RESERVED),
            new TokenType("^date", TokenType.Type.RESERVED),
            new TokenType("^poses", TokenType.Type.RESERVED),
            new TokenType("^expressions", TokenType.Type.RESERVED),
            new TokenType("^voices", TokenType.Type.RESERVED),
            new TokenType("^music", TokenType.Type.RESERVED),
            new TokenType("^description", TokenType.Type.RESERVED),
            new TokenType("^dialogueSet", TokenType.Type.RESERVED),
            new TokenType("^main", TokenType.Type.RESERVED),
            new TokenType("^end", TokenType.Type.RESERVED),
            new TokenType("^keycodes", TokenType.Type.RESERVED),
            new TokenType("^objBinds", TokenType.Type.RESERVED),
            new TokenType("^/title", TokenType.Type.RESERVED),
            new TokenType("^/author", TokenType.Type.RESERVED),
            new TokenType("^/date", TokenType.Type.RESERVED),
            new TokenType("^/poses", TokenType.Type.RESERVED),
            new TokenType("^/expressions", TokenType.Type.RESERVED),
            new TokenType("^/voices", TokenType.Type.RESERVED),
            new TokenType("^/music", TokenType.Type.RESERVED),
            new TokenType("^/description", TokenType.Type.RESERVED),
            new TokenType("^/dialogueSet", TokenType.Type.RESERVED),
            new TokenType("^/main", TokenType.Type.RESERVED),
            new TokenType("^end", TokenType.Type.RESERVED),
            new TokenType("^/keycodes", TokenType.Type.RESERVED),
            new TokenType("^/objBinds", TokenType.Type.RESERVED),
            new TokenType("^/", TokenType.Type.OP),
            new TokenType("^\\|", TokenType.Type.OP),
            new TokenType("^<<", TokenType.Type.OP),
            new TokenType("^>>", TokenType.Type.OP),
            new TokenType("^\\?^\\?^\\?", TokenType.Type.OP),
            new TokenType("^(^\\@)+[a-zA-Z0-9 _!@#$%^&*()^\\-^\\+^\\=^\\{^\\}^\\[^\\]^\\^\\:^\\^\\;^\\'^\\<^\\,^\\>^\\/^\\?^\\|^\\^\\.^\\r^\\n]+(<<)^\"", TokenType.Type.DIALOGUE),
            new TokenType("^!", TokenType.Type.NULL),
            new TokenType("^\\$", TokenType.Type.NULL),
            new TokenType("^#", TokenType.Type.NULL),
            new TokenType("^(^\\/^\\/)[a-zA-Z0-9 _!@#$%^&*()^\\-^\\+^\\=^\\{^\\}^\\[^\\]^\\:^\\;^\\'^\\<^\\,^\\>^\\/^\\?^\\|^\\\\^\\.^\"]", TokenType.Type.COM),
            new TokenType("^\\/*[^\\/*]*\\*/", TokenType.Type.COM),
            new TokenType("^\\d+", TokenType.Type.NUM),
            new TokenType("^\\[A-Za-z][A-za-z0-9_]", TokenType.Type.ID),
            new TokenType("^\"[^\"]*\"", TokenType.Type.STRING)
        };

        //Constructor
        public Lexer()
        {
            foreach (string line in Compiler.CompiledData)
            {
                builder.Append(line);
            }
            Tokenize(builder.ToString());
        }

       

        List<DslToken> Tokenize(string srcText)
        {
            var tokens = new List<DslToken>();
            string remainingText = srcText;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);

                if (match.IsMatch)
                {
                    tokens.Add(new DslToken(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                } else
                    remainingText = remainingText.Substring(1);
            }

            tokens.Add(new DslToken(TokenType.Type.NULL, string.Empty));

            return tokens;
        }

        private TokenMatch FindMatch(string srcText)
        {
            foreach(var expr in tokenExpressions)
            {
                var match = expr.Match(srcText);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };

        }
    }
}