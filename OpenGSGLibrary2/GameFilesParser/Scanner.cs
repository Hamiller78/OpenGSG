using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Prepares a text for parsing by converting it into a stream of tokens representing control characters,
    /// keywords and values. Unnecessary characters are removed in the rpocess.
    /// </summary>
    public class Scanner
    {
        public IEnumerator<Token> Scan(TextReader reader)
        {
            while (reader.Peek() != -1)
            {
                var nextChar = (char)reader.Peek();
                if (char.IsWhiteSpace(nextChar) || char.IsControl(nextChar))
                {
                    reader.Read();
                }
                else if (nextChar == '#')
                {
                    reader.ReadLine();
                }
                else if (char.IsLetterOrDigit(nextChar))
                {
                    var sval = ScanName(reader);
                    if (double.TryParse(sval, out _))
                    {
                        yield return Token.FromValue((int)double.Parse(sval));
                    }
                    else
                    {
                        yield return Token.FromString(sval);
                    }
                }
                else if (nextChar == '"')
                {
                    yield return Token.FromString(ScanEmbeddedName(reader));
                }
                else if (char.IsSymbol(nextChar))
                {
                    yield return ScanSymbol(reader);
                }
                else
                {
                    yield return ScanOther(reader);
                }
            }
            yield return Token.FromKind(Kind.EOF);
        }

        private string ScanName(TextReader reader)
        {
            var sb = new StringBuilder();
            var nextCharCode = reader.Peek();
            while (char.IsLetterOrDigit((char)nextCharCode) || (char)nextCharCode == '_')
            {
                sb.Append((char)reader.Read());
                nextCharCode = reader.Peek();
                if (nextCharCode == -1) return sb.ToString();
            }
            return sb.ToString();
        }

        private string ScanEmbeddedName(TextReader reader)
        {
            var sb = new StringBuilder();
            var nextCharCode = reader.Read();
            nextCharCode = reader.Read();
            while ((char)nextCharCode != '"')
            {
                sb.Append((char)nextCharCode);
                nextCharCode = reader.Read();
            }
            return sb.ToString();
        }

        private Token ScanSymbol(TextReader reader)
        {
            var currentChar = (char)reader.Read();
            switch (currentChar)
            {
                case '=':
                    return Token.FromKind(Kind.EQUAL);
                default:
                    return Token.FromKind(Kind.UNKNOWN);
            }
        }

        private Token ScanOther(TextReader reader)
        {
            var currentChar = (char)reader.Read();
            switch (currentChar)
            {
                case '{':
                    return Token.FromKind(Kind.LEFTBRACKET);
                case '}':
                    return Token.FromKind(Kind.RIGHTBRACKET);
                default:
                    return Token.FromKind(Kind.UNKNOWN);
            }
        }
    }
}
