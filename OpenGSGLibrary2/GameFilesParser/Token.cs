namespace Parser
{
    public enum Kind
    {
        EOF,
        EQUAL,
        LEFTBRACKET,
        RIGHTBRACKET,
        KEYWORD,
        NUMBER,
        UNKNOWN
    }

    public class Token
    {
        public Kind kind { get; }
        public int value { get; }
        public string text { get; }

        private Token(Kind k)
        {
            kind = k;
            value = 0;
            text = string.Empty;
        }

        private Token(int i)
        {
            kind = Kind.NUMBER;
            value = i;
            text = string.Empty;
        }

        private Token(string s)
        {
            kind = Kind.KEYWORD;
            text = s;
            value = 0;
        }

        public override string ToString()
        {
            if (kind == Kind.NUMBER)
                return $"NUMBER({value})";
            else if (kind == Kind.KEYWORD)
                return text;
            else
                return kind.ToString();
        }

        public static Token FromKind(Kind k) => new Token(k);
        public static Token FromValue(int i) => new Token(i);
        public static Token FromString(string s) => new Token(s);
    }
}
