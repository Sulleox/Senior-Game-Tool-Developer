using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public static class ToolsUtils
{
    public const int FIELD_SIDE_LENGTH = 100;

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
        GUILayout.Label(name, TextFieldStyle(TextAnchor.UpperLeft));
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

    public static CharacterDatabase m_characterDB = null;
    public static CharacterDatabase CharacterDB
    {
        get
        {
            m_characterDB = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(ToolsPaths.CHARACTERS_DATABASE_ASSET_PATH);
            if (m_characterDB == null)
            {
                m_characterDB = ScriptableObject.CreateInstance<CharacterDatabase>();
                AssetDatabase.CreateAsset(m_characterDB, ToolsPaths.CHARACTERS_DATABASE_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            //Create character entries folder
            if (!Directory.Exists(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH))
                Directory.CreateDirectory(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH);

            return m_characterDB;
        }
    }
}
