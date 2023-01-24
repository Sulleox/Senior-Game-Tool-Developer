using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ToolsUtils
{
    private const int ICONS_SIDE_LENGHT = 100;

    private static GUIStyle TextFieldStyle (TextAnchor anchor)
    {
        if (m_textFieldStyle == null)
        {
            m_textFieldStyle = new GUIStyle(GUI.skin.label);
            m_textFieldStyle.alignment = anchor;
            m_textFieldStyle.fixedWidth = ICONS_SIDE_LENGHT; 
        }
        return m_textFieldStyle;
    }
    private static GUIStyle m_textFieldStyle;

    public static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperCenter));
        var result = (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(ICONS_SIDE_LENGHT), GUILayout.Height(ICONS_SIDE_LENGHT));
        GUILayout.EndVertical();
        return result;
    }

    public static Mesh MeshField(string name, Mesh mesh)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperLeft));
        var result = (Mesh) EditorGUILayout.ObjectField(mesh, typeof(Mesh), false);
        GUILayout.EndVertical();
        return result;
    }
}
