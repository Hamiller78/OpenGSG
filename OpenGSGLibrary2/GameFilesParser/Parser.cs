using System;
using System.Collections.Generic;

namespace Parser
{
    public class Parser
    {
        public ILookup<string, object> Parse(IEnumerator<Token> tokenStream)
        {
            tokenStream.MoveNext();
            return ParseAssignmentList(tokenStream);
        }

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

        private int ParseValue(IEnumerator<Token> tokenStream)
        {
            var currentToken = tokenStream.Current;
            tokenStream.MoveNext();
            return currentToken.value;
        }

        private string ParseText(IEnumerator<Token> tokenStream)
        {
            var currentToken = tokenStream.Current;
            tokenStream.MoveNext();
            return currentToken.text;
        }
    }
}
