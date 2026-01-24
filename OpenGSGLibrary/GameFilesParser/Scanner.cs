using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenGSGLibrary.GameFilesParser
{
    /// <summary>
    /// Prepares a text for parsing by converting it into a stream of tokens representing control characters,
    /// keywords and values. Unnecessary characters are removed in the process.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Main Scan method of the Scanner class.
        /// </summary>
        /// <param name="reader">Text in the form of an implementation of TextReader i.e. StringBuilder</param>
        /// <returns></returns>
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
                    if (IsSimpleNumber(sval))
                    {
                        // If it contains a decimal point, keep it as a string
                        if (sval.Contains('.'))
                        {
                            yield return Token.FromString(sval);
                        }
                        else
                        {
                            // Integer - parse and return as int
                            yield return Token.FromValue(
                                (int)
                                    double.Parse(
                                        sval,
                                        System.Globalization.CultureInfo.InvariantCulture
                                    )
                            );
                        }
                    }
                    else
                    {
                        yield return Token.FromString(sval);
                    }
                }
                else if (nextChar == '-') // <- Add this check for negative numbers
                {
                    // Check if it's a negative number
                    reader.Read(); // Consume the minus
                    if (reader.Peek() != -1 && char.IsDigit((char)reader.Peek()))
                    {
                        // It's a negative number
                        var sval = "-" + ScanName(reader);
                        if (IsSimpleNumber(sval))
                        {
                            // If it contains a decimal point, keep it as a string
                            if (sval.Contains('.'))
                            {
                                yield return Token.FromString(sval);
                            }
                            else
                            {
                                // Integer - parse and return as int
                                yield return Token.FromValue(
                                    (int)
                                        double.Parse(
                                            sval,
                                            System.Globalization.CultureInfo.InvariantCulture
                                        )
                                );
                            }
                        }
                        else
                        {
                            yield return Token.FromString(sval);
                        }
                    }
                    else
                    {
                        // It's just a minus sign
                        yield return Token.FromKind(Kind.UNKNOWN);
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

        /// <summary>
        /// Checks if a string represents a simple numeric value.
        /// Supports integers and simple floating-point numbers (e.g., 123, -5, 0.5, -0.5).
        /// Does NOT support dates (e.g., 1950.01.09) or scientific notation.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>True if the string is a valid simple number.</returns>
        private bool IsSimpleNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            int startIndex = 0;

            // Handle optional leading minus sign
            if (value[0] == '-')
            {
                if (value.Length == 1)
                    return false; // Just a minus is not a number
                startIndex = 1;
            }

            bool hasDecimalPoint = false;

            for (int i = startIndex; i < value.Length; i++)
            {
                char c = value[i];

                if (c == '.')
                {
                    // Already have a decimal point, or decimal point is at start/end
                    if (hasDecimalPoint || i == startIndex || i == value.Length - 1)
                        return false;
                    hasDecimalPoint = true;
                }
                else if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        private string ScanName(TextReader reader)
        {
            var sb = new StringBuilder();
            var nextCharCode = reader.Peek();
            while (
                char.IsLetterOrDigit((char)nextCharCode)
                || nextCharCode == '_'
                || nextCharCode == '.'
            )
            {
                sb.Append((char)reader.Read());
                nextCharCode = reader.Peek();
                if (nextCharCode == -1)
                    return sb.ToString();
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
                case '>':
                    if (reader.Peek() == '=')
                    {
                        _ = (char)reader.Read();
                        return Token.FromKind(Kind.GREATER_EQUAL);
                    }
                    else
                    {
                        return Token.FromKind(Kind.GREATER);
                    }
                case '<':
                    if (reader.Peek() == '=')
                    {
                        _ = (char)reader.Read();
                        return Token.FromKind(Kind.LESS_EQUAL);
                    }
                    else
                    {
                        return Token.FromKind(Kind.LESS);
                    }
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
