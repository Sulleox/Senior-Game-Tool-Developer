using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class CharacterIconGenerator : EditorWindow
{

    private const string ICON_GENERATION_SCENE_PATH = @"Assets\0_Scenes\IconGenerationScene.unity";
    private int m_iconSizeX = 256;
    private int m_iconSizeY = 256;

    private GameObject m_prefab;
    private Editor m_meshPreview = null;

    private string m_iconName;
    private Action<string> m_onIconGenerated;

    [MenuItem("Tools/Icon Generator")]
    [MenuItem("Assets/Open Icon Generator")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(CharacterIconGenerator));
        window.minSize = new Vector2(250f, 450f);
    }

    public void Load(GameObject prefab, string name, Action<string> result)
    {
        m_prefab = prefab;
        m_iconName = name;
        m_onIconGenerated = result;
    }

    private void OnGUI()
    {
        int.TryParse(EditorGUILayout.TextField("Icon Size X :", m_iconSizeX.ToString()), out m_iconSizeX);
        int.TryParse(EditorGUILayout.TextField("Icon Size Y :", m_iconSizeY.ToString()), out m_iconSizeY);
        m_prefab = ToolsUtils.GameObjectField("FBX", m_prefab);
        if (m_prefab != null)
        {
            Editor.CreateCachedEditor(m_prefab, null, ref m_meshPreview);
            m_meshPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(ToolsUtils.FIELD_SIDE_LENGTH, ToolsUtils.FIELD_SIDE_LENGTH), GUIStyle.none);
            if (string.IsNullOrEmpty(m_iconName))
                m_iconName = m_prefab.name;
        }
        if (GUILayout.Button("Generate Icon"))
        {
            if(m_onIconGenerated != null)
            {
                m_onIconGenerated?.Invoke(CreateCharacterIcon());
                m_onIconGenerated = null;
            }
            else
            {
                CreateCharacterIcon();
            }
            EditorUtility.DisplayDialog("Confirmation dialog", $"Icon well generated", "Ok");
            this.Close();
        }
    }

    public string CreateCharacterIcon()
    {
        string previousScenePath = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.OpenScene(ICON_GENERATION_SCENE_PATH);
        Camera renderCamera = Camera.main;

        GameObject character = Instantiate(m_prefab, renderCamera.transform);
        character.transform.position += Vector3.forward;
        character.transform.LookAt(renderCamera.transform);
        character.transform.position -= new Vector3(0, character.GetComponent<CapsuleCollider>().height / 2);

        RenderTexture screenshotTexture = new RenderTexture(m_iconSizeX, m_iconSizeY, (int)RenderTextureFormat.ARGB32);
        renderCamera.targetTexture = screenshotTexture;
        renderCamera.Render();

        Texture2D texture = new Texture2D(screenshotTexture.width, screenshotTexture.height, screenshotTexture.graphicsFormat, 0, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        RenderTexture.active = screenshotTexture;
        Rect rect = new Rect(0, 0, screenshotTexture.width, screenshotTexture.height);
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        renderCamera.targetTexture = null;

        //Saving PNG
        byte[] spriteBytes = texture.EncodeToPNG();
        string savePath = Path.Combine(ToolsPaths.CHARACTERS_ICONS_FOLDER_PATH, $"Icon_{m_iconName}.png");
        File.WriteAllBytes(savePath, spriteBytes);
        AssetDatabase.Refresh();

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(savePath);

        if (EditorUtility.DisplayDialog("Optimize choice",
            "Do you want to optimize your texture ? (Set the texture size to 512px, remove MipMaps, and set Filtermode to Bilinear",
            "Optimize", "Go without change"))
        {
            importer.maxTextureSize = Mathf.Max(m_iconSizeX, m_iconSizeY);
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.SaveAndReimport();

        DestroyImmediate(screenshotTexture);
        DestroyImmediate(character);

        EditorSceneManager.OpenScene(previousScenePath);
        return savePath;
    }
}
