// <copyright file="Util.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Analyzers
{
    using System.IO;
    using UnityEditor;

    /// <summary>
    /// The Util.
    /// </summary>
    public static class Util
    {
        private const string DirectoryKey = "BovineLabs.Analyzers.Directory";
        private const string DefaultDirectory = "RoslynAnalyzers";

        public static string GetDirectory()
        {
            return EditorPrefs.GetString(DirectoryKey, DefaultDirectory);
        }

        public static void SetDirectory(string directory)
        {
            EditorPrefs.SetString(DirectoryKey, directory);
        }

        public static string GetCreateDirectory()
        {
            var directory = GetDirectory();

            var fullDir = Path.Combine(Directory.GetCurrentDirectory(), directory);
            Directory.CreateDirectory(fullDir);

            return directory;
        }
    }
}