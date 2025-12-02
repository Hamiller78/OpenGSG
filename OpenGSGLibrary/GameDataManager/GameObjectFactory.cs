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
        /// <summary>
        /// Creates a table of generated objects from the parsed files in one directory.
        /// </summary>
        /// <typeparam name="baseType">Base variable type of the generated objects (e.g. Province) used for the returned dictionary</typeparam>
        /// <typeparam name="derivedType">Specialized variable type of the generated objects (e.g. CwpProvince) used for the generated objects.</typeparam>
        /// <param name="folderPath">String with the directory path.</param>
        /// <returns>Dictionary with generated objects using file names without extensions as key.</returns>
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

        /// <summary>
        /// Creates a table of generated objects from the parsed files in one directory.
        /// </summary>
        /// <typeparam name="idtype">Variable type of the id (e.g. Integer, String)</typeparam>
        /// <typeparam name="baseType">Base variable type of the generated objects (e.g. Province) used for the returned dictionary</typeparam>
        /// <typeparam name="derivedType">Specialized variable type of the generated objects (e.g. CwpProvince) used for the generated objects.</typeparam>
        /// <param name="folderPath">String with the directory path.</param>
        /// <param name="keyField">Name of the field in the parsed file which value is used as key in the return value.</param>
        /// <returns>Dictionary with generated objects.</returns>
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

        /// <summary>
        /// Creates a list of GameObject objects from a Lookup structure.
        /// </summary>
        /// <typeparam name="type">Class inheriting from gamneObject class</typeparam>
        /// <param name="parsedData">Parser data as a Lookup(Of String, object)</param>
        /// <param name="objectId">Field id in the top level of the parser data for the 'type' objects</param>
        /// <returns></returns>
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

        /// <summary>
        /// Parses all files in a folder with txt extension.
        /// </summary>
        /// <param name="gamedataPath">String with the path of the folder.</param>
        /// <returns>Dictionary with a file name as key (without extension) and that file's parsed data as object.</returns>
        public static Dictionary<string, object> ParseFolder(string gamedataPath)
        {
            var dictionaryOfParsedData = new Dictionary<string, object>();

            var txtFilesInDir = Directory.GetFiles(
                gamedataPath,
                "*.txt",
                SearchOption.AllDirectories
            );

            foreach (var textFile in txtFilesInDir)
            {
                try
                {
                    using var rawFile = File.OpenText(textFile);

                    // Use the Parser.Scanner and Parser.Parser types which are included in the project
                    var scanner = new Parser.Scanner();
                    var parser = new Parser.Parser();

                    var tokenStream = scanner.Scan(rawFile);
                    var nextParseData = parser.Parse(tokenStream);
                    if (nextParseData != null)
                    {
                        dictionaryOfParsedData.Add(
                            Path.GetFileNameWithoutExtension(textFile),
                            nextParseData
                        );
                    }
                }
                catch (Exception)
                {
                    // ignore parse errors
                }
            }

            return dictionaryOfParsedData;
        }

        /// <summary>
        /// Returns parts of a file name separated by - without path and extension
        /// </summary>
        /// <param name="filePath">File name with extension and possibly path.</param>
        /// <returns>Array of strings with the parts of the file names.</returns>
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
