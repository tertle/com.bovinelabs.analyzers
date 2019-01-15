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
        private const string StyleCopDirectory = "Packages/com.bovinelabs.analyzers/RoslynAnalyzers/StyleCop/";
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
            var directory = Util.GetDirectory();

            var fullDir = Path.Combine(Directory.GetCurrentDirectory(), directory);
            Directory.CreateDirectory(fullDir);

            Copy(StyleCopDirectory + "StyleCop.Analyzers.dll", directory);
            Copy(StyleCopDirectory + "StyleCop.Analyzers.CodeFixes.dll", directory);
            Copy(StyleCopDirectory + "Ruleset.ruleset", directory);
            Copy(StyleCopDirectory + "stylecop.json", directory);
        }

        private static void Copy(string asset, string targetDirectory)
        {
            var filename = Path.GetFileName(asset);
            var target = Path.Combine(targetDirectory, filename);
            AssetDatabase.CopyAsset(asset, target);
        }

        private void OnEnable()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIDirectory + "AnalyzersTemplate.uxml");
            var ui = asset.CloneTree(null);
            ui.AddStyleSheetPath(UIDirectory + "AnalyzersStyle.uss");

            var root = this.GetRootVisualContainer();
            root.Add(ui);

            root.Query<Button>("stylecop").First().clickable.clicked += StyleCopOnClicked;

            var targetDirectoryField = root.Query<TextField>("targetdirectory").First();
            targetDirectoryField.value = Util.GetDirectory();
            targetDirectoryField.OnValueChanged(evt => Util.SetDirectory(evt.newValue));
        }
    }
}