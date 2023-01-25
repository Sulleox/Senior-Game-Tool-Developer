using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class ToolsUtils
{
    private const int FIELD_SIDE_LENGTH = 100;

    private static GUIStyle TextFieldStyle (TextAnchor anchor)
    {
        if (m_textFieldStyle == null)
        {
            m_textFieldStyle = new GUIStyle(GUI.skin.label);
            m_textFieldStyle.fixedWidth = FIELD_SIDE_LENGTH; 
        }
        m_textFieldStyle.alignment = anchor;
        return m_textFieldStyle;
    }
    private static GUIStyle m_textFieldStyle;
    
    /*
    public static UnityEngine.Object CustomField(string name, UnityEngine.Object unityObject)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperCenter));
        var result = EditorGUILayout.ObjectField(unityObject, typeof(Texture2D), false, GUILayout.Width(FIELD_SIDE_LENGTH), GUILayout.Height(FIELD_SIDE_LENGTH));
        GUILayout.EndVertical();
        return result;
    }
    */

    public static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperCenter));
        var result = (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(FIELD_SIDE_LENGTH), GUILayout.Height(FIELD_SIDE_LENGTH));
        GUILayout.EndVertical();
        return result;
    }

    public static Sprite SpriteField(string name, Sprite texture)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperCenter));
        var result = (Sprite)EditorGUILayout.ObjectField(texture, typeof(Sprite), false, GUILayout.Width(FIELD_SIDE_LENGTH), GUILayout.Height(FIELD_SIDE_LENGTH));
        GUILayout.EndVertical();
        return result;
    }


    public static GameObject GameObjectField(string name, GameObject gameObject)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperCenter));
        var result = (GameObject) EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);
        GUILayout.EndVertical();
        return result;
    }

    public static Material MaterialField(string name, Material material)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperLeft));
        var result = (Material) EditorGUILayout.ObjectField(material, typeof(Material), false);
        GUILayout.EndVertical();
        return result;
    }

    public static AnimatorController AnimatorControllerField(string name, AnimatorController animatorController)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperLeft));
        var result = (AnimatorController)EditorGUILayout.ObjectField(animatorController, typeof(AnimatorController), false);
        GUILayout.EndVertical();
        return result;
    }

    public static Avatar AvatarField(string name, Avatar avatar)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperLeft));
        var result = (Avatar)EditorGUILayout.ObjectField(avatar, typeof(Avatar), false);
        GUILayout.EndVertical();
        return result;
    }

    public static Animator GetOrAddAnimator(GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        return animator ? animator : gameObject.AddComponent<Animator>();
    }
}
