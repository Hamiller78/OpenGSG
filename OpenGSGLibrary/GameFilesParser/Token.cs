namespace OpenGSGLibrary.GameFilesParser
{
    /// <summary>
    /// Enumeration for the different Kinds of Tokens
    /// </summary>
    public enum Kind
    {
        EOF,
        EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESS,
        LESS_EQUAL,
        LEFTBRACKET,
        RIGHTBRACKET,
        KEYWORD,
        NUMBER,
        UNKNOWN,
    }

    /// <summary>
    /// Token class to represent the elements in game files.
    /// A whole file will be represented in a compact stream of Token objects by the Scanner
    /// </summary>
    public class Token
    {
        public Kind kind { get; }
        public int value { get; }
        public string text { get; }

        /// <summary>
        /// Creates a Token object of a given Kind without a value
        /// </summary>
        /// <param name="k">The Kind of the new Token</param>
        private Token(Kind k)
        {
            kind = k;
            value = 0;
            text = string.Empty;
        }

        /// <summary>
        /// Creates a Token object of the NUMBER Kind with the given value
        /// </summary>
        /// <param name="i">Number to be assigned to the Token</param>
        private Token(int i)
        {
            kind = Kind.NUMBER;
            value = i;
            text = string.Empty;
        }

        /// <summary>
        /// Creates a Token object of the KEYWORD Kind with the given text
        /// </summary>
        /// <param name="s">Text to be assigned to the Token</param>
        private Token(string s)
        {
            kind = Kind.KEYWORD;
            text = s;
            value = 0;
        }

        /// <summary>
        /// Returns a string representation of the Token depending on its Kind
        /// </summary>
        /// <returns>Text for KEYWORD Tokens, value for NUMBER Tokens, Kind as text for other</returns>
        public override string ToString()
        {
            if (kind == Kind.NUMBER)
                return $"NUMBER({value})";
            else if (kind == Kind.KEYWORD)
                return text;
            else
                return kind.ToString();
        }

        /// <summary>
        /// Static factory method to generate a Token of the given Kind
        /// </summary>
        /// <param name="k">Kind of the generated Token</param>
        /// <returns>Generated Token</returns>
        public static Token FromKind(Kind k) => new Token(k);

        /// <summary>
        /// Static factory method to generate a NUMBER Token
        /// </summary>
        /// <param name="i">Value of the generated Token</param>
        /// <returns>Generated Token</returns>
        public static Token FromValue(int i) => new Token(i);

        /// <summary>
        /// Static factory method to generate a KEYWORD Token
        /// </summary>
        /// <param name="s">Text of the generated Token</param>
        /// <returns>Generated Token</returns>
        public static Token FromString(string s) => new Token(s);
    }
}
