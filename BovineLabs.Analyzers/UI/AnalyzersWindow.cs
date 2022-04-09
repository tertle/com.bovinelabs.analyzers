// <copyright file="AnalyzersWindow.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Analyzers.UI
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class AnalyzersWindow : EditorWindow
    {
        private const string Packages = "Packages/com.bovinelabs.analyzers/";
        private const string StyleCopDirectory = Packages + "RoslynAnalyzers/StyleCopAnalyzers/";
        private const string ReflectionDirectory = Packages + "RoslynAnalyzers/ReflectionAnalyzers/";
        private const string DisposableDirectory = Packages + "RoslynAnalyzers/DisposableAnalyzers/";
        private const string UIDirectory = Packages + "BovineLabs.Analyzers/UI/";

        [MenuItem("Window/BovineLabs/Tools/Analyzers", priority = 1005)]
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

        private static void DisposableOnClicked()
        {
            var directory = Util.GetCreateDirectory();

            Copy(DisposableDirectory + "IDisposableAnalyzers.dll", directory);
            Copy(DisposableDirectory + "Gu.Roslyn.Extensions.dll", directory);
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

            File.Copy(asset, target, true);
        }

        private void OnEnable()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIDirectory + "AnalyzersTemplate.uxml");
            var ui = asset.CloneTree((string)null);
            var root = this.rootVisualElement;

            root.Add(ui);
            root.Query<Button>("stylecop").First().clickable.clicked += StyleCopOnClicked;
            root.Query<Button>("reflection").First().clickable.clicked += ReflectionOnClicked;
            root.Query<Button>("disposable").First().clickable.clicked += DisposableOnClicked;

            var targetDirectoryField = root.Query<TextField>("targetDirectory").First();
            targetDirectoryField.value = Util.GetDirectory();
            targetDirectoryField.RegisterValueChangedCallback(evt => Util.SetDirectory(evt.newValue));
        }
    }
}
