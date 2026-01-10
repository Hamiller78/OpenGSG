using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.Localization
{
    /// <summary>
    /// Manages localized strings from CSV files.
    /// Loads all .csv files from the localization directory and provides lookups by key and language.
    /// </summary>
    public class LocalizationManager
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizations = new();
        private string _currentLanguage = "english";

        /// <summary>
        /// Gets or sets the current active language.
        /// </summary>
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value ?? "english";
        }

        /// <summary>
        /// Loads all localization files from the specified directory.
        /// CSV format: key,english,french,german,...
        /// </summary>
        /// <param name="localizationPath">Path to the localisation directory.</param>
        public void LoadFromDirectory(string localizationPath)
        {
            if (!Directory.Exists(localizationPath))
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Warning,
                        $"Localization directory not found: {localizationPath}"
                    );
                return;
            }

            var csvFiles = Directory.GetFiles(
                localizationPath,
                "*.csv",
                SearchOption.AllDirectories
            );

            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loading localizations from {csvFiles.Length} file(s)");

            foreach (var file in csvFiles)
            {
                try
                {
                    LoadLocalizationFile(file);
                }
                catch (Exception ex)
                {
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(
                            LogLevel.Err,
                            $"Error loading localization file {file}: {ex.Message}"
                        );
                }
            }

            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loaded {_localizations.Count} localization keys");
        }

        /// <summary>
        /// Loads a single CSV localization file.
        /// Expected format: key,english,french,german,...
        /// First row is header with language names.
        /// </summary>
        private void LoadLocalizationFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return;

            // First line is header with language names
            var header = ParseCsvLine(lines[0]);
            if (header.Length < 2)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Warning, $"Invalid localization file header: {filePath}");
                return;
            }

            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var columns = ParseCsvLine(line);
                if (columns.Length < 2)
                    continue;

                var key = columns[0].Trim();
                if (string.IsNullOrEmpty(key))
                    continue;

                // Get or create dictionary for this key
                if (!_localizations.ContainsKey(key))
                {
                    _localizations[key] = new Dictionary<string, string>();
                }

                var locDict = _localizations[key];

                // Map columns to languages from header
                for (int col = 1; col < Math.Min(columns.Length, header.Length); col++)
                {
                    var language = header[col].Trim().ToLowerInvariant();
                    var text = columns[col].Trim();

                    if (!string.IsNullOrEmpty(text))
                    {
                        locDict[language] = text;
                    }
                }
            }
        }

        /// <summary>
        /// Parses a CSV line, handling quoted values with commas.
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var currentField = string.Empty;
            var insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ';' && !insideQuotes)
                {
                    result.Add(currentField);
                    currentField = string.Empty;
                }
                else
                {
                    currentField += c;
                }
            }

            // Add last field
            result.Add(currentField);

            return result.ToArray();
        }

        /// <summary>
        /// Gets a localized string for the given key in the current language.
        /// Returns the key itself if not found.
        /// </summary>
        /// <param name="key">Localization key.</param>
        /// <returns>Localized string or the key if not found.</returns>
        public string GetString(string key)
        {
            return GetString(key, _currentLanguage);
        }

        /// <summary>
        /// Gets a localized string for the given key in the specified language.
        /// Falls back to English if the language is not found.
        /// Returns the key itself if not found in any language.
        /// </summary>
        /// <param name="key">Localization key.</param>
        /// <param name="language">Language code (e.g., "english", "french").</param>
        /// <returns>Localized string or the key if not found.</returns>
        public string GetString(string key, string language)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (!_localizations.TryGetValue(key, out var locDict))
                return key; // Return key if not found

            language = language.ToLowerInvariant();

            // Try requested language
            if (locDict.TryGetValue(language, out var text))
                return text;

            // Fallback to English
            if (locDict.TryGetValue("english", out text))
                return text;

            // Fallback to first available language
            if (locDict.Count > 0)
                return locDict.Values.First();

            // Return key if no translation found
            return key;
        }

        /// <summary>
        /// Checks if a localization key exists.
        /// </summary>
        /// <param name="key">Localization key.</param>
        /// <returns>True if the key exists.</returns>
        public bool HasKey(string key)
        {
            return _localizations.ContainsKey(key);
        }

        /// <summary>
        /// Gets all available languages.
        /// </summary>
        /// <returns>List of language codes.</returns>
        public List<string> GetAvailableLanguages()
        {
            var languages = new HashSet<string>();
            foreach (var locDict in _localizations.Values)
            {
                foreach (var lang in locDict.Keys)
                {
                    languages.Add(lang);
                }
            }
            return languages.ToList();
        }

        /// <summary>
        /// Clears all loaded localizations.
        /// </summary>
        public void Clear()
        {
            _localizations.Clear();
        }
    }
}
