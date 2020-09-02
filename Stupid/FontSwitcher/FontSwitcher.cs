using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_EDITOR
public class FontSwitcher : EditorWindow
{
    Font font;

    [MenuItem("Window/Switch Fonts")]
    public static void Init()
    {
        FontSwitcher window = (FontSwitcher)EditorWindow.GetWindow(typeof(FontSwitcher), true, "Font Switcher");
        window.Show();
        window.maxSize = new Vector2(200, 200);
        window.minSize = window.maxSize;
    }

    public static void Exit()
    {
        FontSwitcher window = (FontSwitcher)EditorWindow.GetWindow(typeof(FontSwitcher), true, "Font Switcher");
        window.Close();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a new font to load:", EditorStyles.boldLabel);
        font = (Font)EditorGUILayout.ObjectField("", font, typeof(Font), false);

        if (GUILayout.Button("Apply font"))
        {
            ApplyFont(font);
            Exit();
        }

        GUILayout.Label("");
        GUILayout.Label("Example:", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUIStyle style = EditorStyles.boldLabel;
        Font previousFont = style.font;

        style.font = font;
        style.wordWrap = true;

        GUILayout.Label("The quick brown fox jumps over the lazy dog.", style);

        style.font = previousFont;

        style = EditorStyles.label;
        previousFont = style.font;

        style.font = font;
        style.wordWrap = true;

        GUILayout.Label("The quick brown fox jumps over the lazy dog.", style);

        style.font = previousFont;

        EditorGUILayout.EndVertical();
    }


    public static void ApplyFont(Font newFont)
    {
        PropertyInfo[] editorStyleProperties;
        PropertyInfo[] defaultGuiProperties;

        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty;
        editorStyleProperties = typeof(EditorStyles).GetProperties(flags);
        defaultGuiProperties = GUI.skin.GetType().GetProperties();

        foreach (PropertyInfo i in editorStyleProperties)
        {
            if (PropertyInfoIsValid(i, null))
            {
                GUIStyle style = (GUIStyle)i.GetValue(null, null);
                style.font = newFont;
            }
        }

        foreach (PropertyInfo i in defaultGuiProperties)
        {
            if (PropertyInfoIsValid(i, GUI.skin))
            {
                GUIStyle style = (GUIStyle)i.GetValue(GUI.skin, null);
                style.font = newFont;
            }
        }

        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            style.font = newFont;
        }

        RepaintAll();
    }

    static bool PropertyInfoIsValid(PropertyInfo x, object item)
    {
        if (string.IsNullOrEmpty(x.Name))
        {
            return false;
        }
        else if (x.PropertyType != typeof(GUIStyle))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static void RepaintAll() { foreach (var w in Resources.FindObjectsOfTypeAll<EditorWindow>()) w.Repaint(); }
}

#endif