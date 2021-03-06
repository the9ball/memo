﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using System.Text;
using System.IO;
using System.Linq;

public static class GenerateEnum
{
    private const string Indent = "    ";
    private const string EndLine = "\r\n";

    private static readonly string[] InvalidCharacters =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };
    private static readonly string ReplaceCharacter = "_";

    private const string MenuItemName = "Tools/Create/Generate Project Enums";
    private const string ExportFilePath = "Assets/Products/Scripts/Generated/ProjectEnums.cs"; // 出力ファイル.

    [DidReloadScripts(), MenuItem(MenuItemName)]
    public static void Generate()
    {
        var text = string.Empty;

        text += "/// Auto Generated by class GenerateEnum" + EndLine;

        text += "namespace Projectf" + EndLine;
        text += "{" + EndLine;

        text += ExportLayer(Indent);
        text += ExportTag(Indent);
        text += ExportSortingLayer(Indent);
        text += ExportSceneName(Indent);

        text += "}" + EndLine;
        text += EndLine;

        var directoryName = Path.GetDirectoryName(ExportFilePath);
        if (!Directory.Exists(directoryName))
        {
            //Directory.CreateDirectory(directoryName);
            UnityEngine.Debug.LogError("Not Found:" + directoryName);
            return;
        }

        File.WriteAllText(ExportFilePath, text, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    private static string ExportTag(string indent)
    {
        var text = string.Empty;

        text += indent + "public enum Tag" + EndLine;
        text += indent + "{" + EndLine;

        var tags = InternalEditorUtility.tags
            .Select((x, i) => new { i = i, x = x });
        foreach (var x in tags)
        {
            text += indent + Indent + ReplaceInvalidCharacter(x.x) + " = " + x.i + "," + EndLine;
        }

        text += indent + "}" + EndLine;
        text += "" + EndLine;

        return text;
    }

    private static string ExportLayer(string indent)
    {
        var text = string.Empty;

        var layers = InternalEditorUtility.layers;

        text += indent + "public enum Layer" + EndLine;
        text += indent + "{" + EndLine;

        foreach (var x in layers)
        {
            text += indent + Indent + ReplaceInvalidCharacter(x) + " = " + LayerMask.NameToLayer(x) + "," + EndLine;
        }

        text += indent + "}" + EndLine;
        text += "" + EndLine;

        text += indent + "[System.Flags]" + EndLine;
        text += indent + "public enum LayerMask" + EndLine;
        text += indent + "{" + EndLine;

        foreach (var x in layers)
        {
            text += indent + Indent + ReplaceInvalidCharacter(x) + " = 1 << " + LayerMask.NameToLayer(x) + "," + EndLine;
        }

        text += indent + "}" + EndLine;
        text += "" + EndLine;

        return text;
    }

    private static string ExportSortingLayer(string indent)
    {
        var text = string.Empty;

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var sortingLayer = tagManager.FindProperty("m_SortingLayers");

        text += indent + "public enum SortingLayer" + EndLine;
        text += indent + "{" + EndLine;

        for (int i = 0; i < sortingLayer.arraySize; ++i)
        {
            string tag = sortingLayer.GetArrayElementAtIndex(i).displayName;
            text += indent + Indent + ReplaceInvalidCharacter(tag) + " = " + i + "," + EndLine;
        }

        text += indent + "}" + EndLine;
        text += "" + EndLine;

        return text;
    }

    private static string ExportSceneName(string indent)
    {
        var text = string.Empty;

        text += indent + "public enum SceneName" + EndLine;
        text += indent + "{" + EndLine;

        var scenes = EditorBuildSettings.scenes
            .Select(x => Path.GetFileNameWithoutExtension(x.path))
            .Select(x => ReplaceInvalidCharacter(x))
            .Distinct();
        foreach (var x in scenes)
        {
            text += indent + Indent + x + "," + EndLine;
        }

        text += indent + "}" + EndLine;
        text += "" + EndLine;

        return text;
    }

    private static string ReplaceInvalidCharacter(string s)
    {
        foreach (var x in InvalidCharacters)
        {
            s = s.Replace(x, ReplaceCharacter);
        }
        return s;
    }

    [MenuItem(MenuItemName, true)]
    public static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }
}
