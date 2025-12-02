using System;
using System.Collections.Generic;

namespace OpenGSGLibrary.GameFilesParser
{
    public class Parser
    {
        /// <summary>
        /// Parse a token stream.
        /// Will return a nested structure with the following properties:
        /// A Lookup is a list of assignments with a keyword string and a VariantType object with a contained type
        /// depending on the represented content.
        /// Strings and numbers are String and Integer. Nested lists of assignments are Lookup objects.
        /// A list of numbers is a list of integers.
        /// </summary>
        /// <param name="tokenStream">Token stream.</param>
        /// <returns>Returns a Lookup with a structure repressenting the stream content.</returns>
		public ILookup<string, object> Parse(IEnumerator<Token> tokenStream)
        {
            tokenStream.MoveNext();
            return ParseAssignmentList(tokenStream);
        }
		
        /// <summary>
        /// Parses any type of collection (stuff enclosed in curly brackets).
        /// Token stream should be set to the left bracket and will be moved to the first token after the right bracket.
        /// </summary>
        /// <param name="tokenStream">Token stream. Must be set to an element behind a left bracket.</param>
        /// <returns>Returns an object containing either a Lookup, a list of integers or another object.</returns>
        private object ParseCollection(IEnumerator<Token> tokenStream)
        {
            object collectionObj = new object();

            tokenStream.MoveNext();
            var currentToken = tokenStream.Current;
            if (currentToken.kind == Kind.NUMBER)
            {
                collectionObj = ParseValueList(tokenStream);
            }
            else if (currentToken.kind == Kind.KEYWORD)
            {
                collectionObj = ParseAssignmentList(tokenStream);
            }
            else if (currentToken.kind == Kind.RIGHTBRACKET)
            {
                collectionObj = null;
            }
            else
            {
                throw new ApplicationException("Error while parsing: {, } or keyword expected, got " + currentToken.ToString() + " instead.");
            }
            tokenStream.MoveNext();

            return collectionObj;
        }

        /// <summary>
        /// Parses a list of assignment. Also works on the top level, where the list is terminated by EOF.
        /// Returns with the token stream on the RIGHTBRACKET or EOF.
        /// </summary>
        /// <param name="tokenStream">Token stream swet on the first keyword.</param>
        /// <returns>Returns a Lookup with keywords as keys and objects as value.</returns>
        private ILookup<string, object> ParseAssignmentList(IEnumerator<Token> tokenStream)
        {
            var parsedList = new List<KeyValuePair<string, object>>();
            var currentToken = tokenStream.Current;

            while ((currentToken.kind != Kind.RIGHTBRACKET) && (currentToken.kind != Kind.EOF))
            {
                var newEntry = ParseAssignment(tokenStream);
                if (newEntry.Value != null)
                {
                    parsedList.Add(newEntry);
                }
                currentToken = tokenStream.Current;
            }

            var returnLookup = parsedList.ToLookup(kvPair => kvPair.Key, kvPair => kvPair.Value);
            return returnLookup;
        }

        /// <summary>
        /// Parses an assignment. Token stream will be moved to the token after the rhs of the assignment.
        /// </summary>
        /// <param name="tokenStream">Token stream must be set on a KEYWORD token.</param>
        /// <returns>Returns a KeyValuePair with the keyword and a general object.</returns>
        private KeyValuePair<string, object> ParseAssignment(IEnumerator<Token> tokenStream)
        {
            var currentToken = tokenStream.Current;
            if (currentToken.kind != Kind.KEYWORD)
                throw new ApplicationException("Error while parsing: Keyword expected, got " + currentToken.ToString() + " instead.");

            var keyword = currentToken.text;

            tokenStream.MoveNext();
            currentToken = tokenStream.Current;
            if (currentToken.kind != Kind.EQUAL)
                throw new ApplicationException("Error while parsing: = expected, got " + currentToken.ToString() + " instead.");

            var assignedObject = ParseRhs(tokenStream);

            return new KeyValuePair<string, object>(keyword, assignedObject);
        }

        /// <summary>
        /// Parses right hand side of an assigment.
        /// Token stream must be set to the EQUAL token and will be moved behind the assignment.
        /// </summary>
        /// <param name="tokenStream">Token stream set to an EQUAL token.</param>
        /// <returns>Returns an object containing either a Lookup, a list of integers or a nested object.</returns>
        private object ParseRhs(IEnumerator<Token> tokenStream)
        {
            object returnObj = new object();

            tokenStream.MoveNext();
            var currentToken = tokenStream.Current;
            switch (currentToken.kind)
            {
                case Kind.LEFTBRACKET:
                    returnObj = ParseCollection(tokenStream);
                    break;
                case Kind.NUMBER:
                    returnObj = ParseValue(tokenStream);
                    break;
                case Kind.KEYWORD:
                    returnObj = ParseText(tokenStream);
                    break;
                default:
                    throw new ApplicationException("Error while parsing: {, } or assignable value expected, got " + currentToken.ToString() + " instead.");
            }

            return returnObj;
        }

        /// <summary>
        /// Parses a list of numeric values.
        /// Token stream should be set on first element and will be moved to the right bracket which ends the list.
        /// </summary>
        /// <param name="tokenStream">Token stream. Should be set to the first number after a left bracket.</param>
        /// <returns>List of the values as integers.</returns>
        private List<int> ParseValueList(IEnumerator<Token> tokenStream)
        {
            var returnList = new List<int>();
            var currentToken = tokenStream.Current;
            while (currentToken.kind != Kind.RIGHTBRACKET)
            {
                if (currentToken.kind != Kind.NUMBER)
                    throw new ApplicationException("Error while parsing value list: Number expected, got " + currentToken.ToString() + " instead.");

                returnList.Add(currentToken.value);
                tokenStream.MoveNext();
                currentToken = tokenStream.Current;
            }

            return returnList;
        }

        /// <summary>
        /// Parses the numeric value of the current token and moves the iterator one element forward.
        /// </summary>
        /// <param name="tokenStream">Token stream. Must be set to a value element.</param>
        /// <returns>Returns the value as an integer.</returns>
        private int ParseValue(IEnumerator<Token> tokenStream)
        {
            var currentToken = tokenStream.Current;
            tokenStream.MoveNext();
            return currentToken.value;
        }

        /// <summary>
        /// Parses the text of the current token and moves the iterator one element forward.
        /// </summary>
        /// <param name="tokenStream">Token stream. Must be set to a keyword element.</param>
        /// <returns>Returns the text of the current token as a string</returns>
        private string ParseText(IEnumerator<Token> tokenStream)
        {
            var currentToken = tokenStream.Current;
            tokenStream.MoveNext();
            return currentToken.text;
        }
    }
}
