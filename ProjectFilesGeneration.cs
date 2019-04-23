// <copyright file="ProjectFilesGeneration.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Analyzers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using UnityEditor;

    /// <summary>
    /// Customize the project file generation with Roslyn Analyzers and custom c# version.
    /// </summary>
    [InitializeOnLoad]
    public class ProjectFilesGeneration : AssetPostprocessor
    {
#if ENABLE_VSTU
        private const string CSharpVersion = "7.3";
#endif

        static ProjectFilesGeneration()
        {
#if ENABLE_VSTU
            SyntaxTree.VisualStudio.Unity.Bridge.ProjectFilesGenerator.ProjectFileGeneration += (name, contents) =>
            {
                XDocument xml = XDocument.Parse(contents);

                UpgradeProjectFile(xml);

                // Write to the csproj file:
                using (Utf8StringWriter str = new Utf8StringWriter())
                {
                    xml.Save(str);
                    return str.ToString();
                }
            };
#else
        }

        private static string OnGeneratedCSProject(string path, string contents)
        {
            XDocument xml = XDocument.Parse(contents);

            UpgradeProjectFile(xml);

            // Write to the csproj file:
            using (Utf8StringWriter str = new Utf8StringWriter())
            {
                xml.Save(str);
                return str.ToString();
            }
#endif
        }

        private static void UpgradeProjectFile(XDocument doc)
        {
            var projectContentElement = doc.Root;
            if (projectContentElement != null)
            {
                XNamespace xmlns = projectContentElement.Name.NamespaceName; // do not use var
                SetRoslynAnalyzers(projectContentElement, xmlns);
#if UNITY_VTSU
                SetCSharpVersion(projectContentElement, xmlns);
#endif
            }
        }

        /// <summary>
        ///  Add everything from RoslynAnalyzers folder to csproj.
        /// </summary>
        private static void SetRoslynAnalyzers(XElement projectContentElement, XNamespace xmlns)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // TODO do not require strict com.bovinelabs.analyzers folder
            var roslynAnalyzerBaseDir = new DirectoryInfo(Path.Combine(currentDirectory, Util.GetDirectory()));

            if (!roslynAnalyzerBaseDir.Exists)
            {
                //Debug.LogWarning($"Directory {roslynAnalyzerBaseDir} does not exist, please place analyzers in correct location.");
                return;
            }

            var relPaths = roslynAnalyzerBaseDir.GetFiles("*", SearchOption.AllDirectories)
                    .Select(x => x.FullName.Substring(currentDirectory.Length + 1));

            var itemGroup = new XElement(xmlns + "ItemGroup");

            foreach (var file in relPaths)
            {
                var extension = new FileInfo(file).Extension;

                switch (extension)
                {
                    case ".dll":
                    {
                        var reference = new XElement(xmlns + "Analyzer");
                        reference.Add(new XAttribute("Include", file));
                        itemGroup.Add(reference);
                        break;
                    }

                    case ".json":
                    {
                        var reference = new XElement(xmlns + "AdditionalFiles");
                        reference.Add(new XAttribute("Include", file));
                        itemGroup.Add(reference);
                        break;
                    }

                    case ".ruleset":
                    {
                        SetOrUpdateProperty(projectContentElement, xmlns, "CodeAnalysisRuleSet", existing => file);
                        break;
                    }
                }
            }

            projectContentElement.Add(itemGroup);
        }

        // Don't need to do this for Rider as it has built in support for setting c# version.
#if UNITY_VTSU
        private static void SetCSharpVersion(XContainer projectContentElement, XNamespace ns)
        {
            // Find all PropertyGroups with Condition defining a Configuration and a Platform:
            XElement[] nodes = projectContentElement.Descendants()
                .Where(child =>
                    child.Name.LocalName == "PropertyGroup"
                    && (child.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "Condition")?.Value
                            .Contains("'$(Configuration)|$(Platform)'") ?? false))
                .ToArray();

            // Add <LangVersion>7.3</LangVersion> to these PropertyGroups:
            foreach (XElement node in nodes)
            {
                node.Add(new XElement(ns + "LangVersion", CSharpVersion));
            }
        }
#endif

        private static void SetOrUpdateProperty(
            XContainer root,
            XNamespace xmlns,
            string name,
            Func<string, string> updater)
        {
            var element = root.Elements(xmlns + "PropertyGroup").Elements(xmlns + name).FirstOrDefault();
            if (element != null)
            {
                var result = updater(element.Value);
                if (result != element.Value)
                {
                    element.SetValue(result);
                }
            }
            else
            {
                AddProperty(root, xmlns, name, updater(string.Empty));
            }
        }

        // Adds a property to the first property group without a condition
        private static void AddProperty(XContainer root, XNamespace xmlns, string name, string content)
        {
            var propertyGroup = root.Elements(xmlns + "PropertyGroup")
              .FirstOrDefault(e => !e.Attributes(xmlns + "Condition").Any());
            if (propertyGroup == null)
            {
                propertyGroup = new XElement(xmlns + "PropertyGroup");
                root.AddFirst(propertyGroup);
            }

            propertyGroup.Add(new XElement(xmlns + name, content));
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
