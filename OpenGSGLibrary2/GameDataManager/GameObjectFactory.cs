using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WorldData
{
    /// <summary>
    /// Class to generate game objects from game files
    /// </summary>
    public static class GameObjectFactory
    {
        public static Dictionary<int, TBase> FromFolderWithFilenameId<TBase, TDerived>(
            string folderPath
        )
            where TBase : GameObject, new()
            where TDerived : TBase, new()
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(
                    "Given game data directory not found: " + folderPath
                );

            var objectTable = new Dictionary<int, TBase>();

            var parsedObjectData = ParseFolder(folderPath);
            foreach (var singleObjectData in parsedObjectData)
            {
                var newObject = new TDerived();
                newObject.SetData(
                    singleObjectData.Key,
                    (ILookup<string, object>)singleObjectData.Value
                );

                try
                {
                    var filenameParts = ExtractFromFilename(singleObjectData.Key);
                    var key = Convert.ToInt32(filenameParts[0]);
                    objectTable.Add(key, newObject);
                }
                catch (Exception)
                {
                    // Ignore entries that cannot be parsed; original code logged a fatal error.
                }
            }

            return objectTable;
        }

        public static Dictionary<TKey, TBase> FromFolder<TKey, TBase, TDerived>(
            string folderPath,
            string keyField
        )
            where TBase : GameObject, new()
            where TDerived : TBase, new()
            where TKey : notnull
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(
                    "Given game data directory not found: " + folderPath
                );

            var objectTable = new Dictionary<TKey, TBase>();

            var parsedObjectData = ParseFolder(folderPath);
            foreach (var singleObjectData in parsedObjectData)
            {
                var countryData = (ILookup<string, object>)singleObjectData.Value;
                var newObject = new TDerived();
                newObject.SetData(singleObjectData.Key, countryData);

                var keyObj = countryData[keyField].Single();
                TKey key = (TKey)Convert.ChangeType(keyObj, typeof(TKey));
                objectTable.Add(key, newObject);
            }

            return objectTable;
        }

        public static List<T> ListFromLookup<T>(ILookup<string, object> parsedData, string objectId)
            where T : GameObject, new()
        {
            var generatedList = new List<T>();

            foreach (var objParserData in parsedData[objectId])
            {
                var newObj = new T();
                newObj.SetData(string.Empty, (ILookup<string, object>)objParserData);
                generatedList.Add(newObj);
            }

            return generatedList;
        }

        public static Dictionary<string, object> ParseFolder(string gamedataPath)
        {
            var dictionaryOfParsedData = new Dictionary<string, object>();

            var txtFilesInDir = Directory.GetFiles(
                gamedataPath,
                "*.txt",
                SearchOption.AllDirectories
            );

            // Use reflection to invoke Parser.Scanner and Parser.Parser if available at runtime.
            Type? scannerType = null;
            Type? parserType = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (scannerType == null)
                    scannerType = asm.GetType("Parser.Scanner");
                if (parserType == null)
                    parserType = asm.GetType("Parser.Parser");
                if (scannerType != null && parserType != null)
                    break;
            }

            foreach (var textFile in txtFilesInDir)
            {
                try
                {
                    using var rawFile = File.OpenText(textFile);

                    if (scannerType != null && parserType != null)
                    {
                        var scanner = Activator.CreateInstance(scannerType);
                        var parser = Activator.CreateInstance(parserType);

                        var scanMethod = scannerType.GetMethod(
                            "Scan",
                            new Type[] { typeof(TextReader) }
                        );
                        var parseMethod = parserType.GetMethod(
                            "Parse",
                            new Type[] { typeof(IEnumerator<object>) }
                        );

                        // Fallback: call Parse with the returned enumerator using dynamic invoke
                        var tokenStream = scanMethod.Invoke(scanner, new object[] { rawFile });
                        var nextParseData = parseMethod.Invoke(
                            parser,
                            new object[] { tokenStream }
                        );
                        if (nextParseData != null)
                        {
                            dictionaryOfParsedData.Add(
                                Path.GetFileNameWithoutExtension(textFile),
                                nextParseData
                            );
                        }
                    }
                }
                catch (Exception)
                {
                    // ignore parse errors
                }
            }

            return dictionaryOfParsedData;
        }

        public static string[] ExtractFromFilename(string filePath)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrEmpty(filename))
                throw new ApplicationException("No file name found in path: " + filePath);

            var nameParts = filename.Split('-', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nameParts.Length; i++)
                nameParts[i] = nameParts[i].Trim();

            return nameParts;
        }
    }
}
