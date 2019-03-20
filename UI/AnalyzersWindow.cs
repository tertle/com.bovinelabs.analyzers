// <copyright file="AnalyzersWindow.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Analyzers.UI
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
#if UNITY_2019_1_OR_NEWER
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
#else
    using UnityEditor.Experimental.UIElements;
    using UnityEngine.Experimental.UIElements;
#endif


    public class AnalyzersWindow : EditorWindow
    {
        private const string Packages = "Packages/com.bovinelabs.analyzers/";
        private const string StyleCopDirectory = Packages + "RoslynAnalyzers/StyleCopAnalyzers/";
        private const string ReflectionDirectory = Packages + "RoslynAnalyzers/ReflectionAnalyzers/";
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
#if UNITY_2019_1_OR_NEWER
            var ui = asset.CloneTree((string)null);
#else
            var ui = asset.CloneTree(null);
            //ui.AddStyleSheetPath(UIDirectory + "AnalyzersStyle.uss");
#endif

#if UNITY_2019_1_OR_NEWER
            var root = this.rootVisualElement;
#else
            var root = this.GetRootVisualContainer();
#endif
            root.Add(ui);

            root.Query<Button>("stylecop").First().clickable.clicked += StyleCopOnClicked;
            root.Query<Button>("reflection").First().clickable.clicked += ReflectionOnClicked;

            var targetDirectoryField = root.Query<TextField>("targetdirectory").First();
            targetDirectoryField.value = Util.GetDirectory();
#if UNITY_2019_1_OR_NEWER
            targetDirectoryField.RegisterValueChangedCallback(evt => Util.SetDirectory(evt.newValue));
#else
            targetDirectoryField.OnValueChanged(evt => Util.SetDirectory(evt.newValue));
#endif
        }
    }
}