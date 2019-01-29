// <copyright file="AnalyzersWindow.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Analyzers.UI
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.Experimental.UIElements;
    using UnityEngine;
    using UnityEngine.Experimental.UIElements;

    public class AnalyzersWindow : EditorWindow
    {
        private const string Packages = "Packages/com.bovinelabs.analyzers/";
        private const string StyleCopDirectory = Packages + "RoslynAnalyzers/StyleCopAnalyzers/";
        private const string ReflectionDirectory = Packages + "RoslynAnalyzers/ReflectionAnalyzers/";
        private const string DocumentationDirectory = Packages + "RoslynAnalyzers/DocumentationAnalyzers/";
        private const string UIDirectory = "Packages/com.bovinelabs.analyzers/UI/";

        [MenuItem("Window/BovineLabs/Analyzers")]
        private static void ShowWindow()
        {
            var window = GetWindow<AnalyzersWindow>();
            window.minSize = new Vector2(350, 200);
            window.titleContent = new GUIContent("Analyzers Window");
        }

        private static void StyleCopOnClicked()
        {
            var directory = Util.GetCreateDirectory();

            Copy(StyleCopDirectory + "StyleCop.Analyzers.dll", directory);
            Copy(StyleCopDirectory + "StyleCop.Analyzers.CodeFixes.dll", directory);
            Copy(StyleCopDirectory + "Ruleset.ruleset", directory);
            Copy(StyleCopDirectory + "stylecop.json", directory);
        }

        private static void ReflectionOnClicked()
        {
            var directory = Util.GetCreateDirectory();

            Copy(ReflectionDirectory + "ReflectionAnalyzers.dll", directory);
            Copy(ReflectionDirectory + "Gu.Roslyn.Extensions.dll", directory);
        }

        private static void DocumentationOnClicked()
        {
            var directory = Util.GetCreateDirectory();

            Copy(DocumentationDirectory + "CommonMark.dll", directory);
            Copy(DocumentationDirectory + "DocumentationAnalyzers.CodeFixes.dll", directory);
            Copy(DocumentationDirectory + "DocumentationAnalyzers.dll", directory);
        }

        private static void Copy(string asset, string targetDirectory)
        {
            var filename = Path.GetFileName(asset);
            if (filename == null)
            {
                Debug.LogError($"Invalid asset ({asset})");
                return;
            }

            var target = Path.Combine(targetDirectory, filename);
            if (!AssetDatabase.CopyAsset(asset, target))
            {
                Debug.LogError($"File ({asset}) not found.");
            }
        }

        private void OnEnable()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIDirectory + "AnalyzersTemplate.uxml");
            var ui = asset.CloneTree(null);
            ui.AddStyleSheetPath(UIDirectory + "AnalyzersStyle.uss");

            var root = this.GetRootVisualContainer();
            root.Add(ui);

            root.Query<Button>("stylecop").First().clickable.clicked += StyleCopOnClicked;
            root.Query<Button>("reflection").First().clickable.clicked += ReflectionOnClicked;
            root.Query<Button>("documentation").First().clickable.clicked += DocumentationOnClicked;

            var targetDirectoryField = root.Query<TextField>("targetdirectory").First();
            targetDirectoryField.value = Util.GetDirectory();
            targetDirectoryField.OnValueChanged(evt => Util.SetDirectory(evt.newValue));
        }
    }
}