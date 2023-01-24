using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoreCharactersImporterWindows : EditorWindow
{
    [MenuItem("Tools/Store Characters Importer")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(StoreCharactersImporterWindows));
    }

    private string m_characterName = "Name";
    private int m_characterPrice = 100;
    private Texture2D m_characterIcon;
    private Texture2D m_characterTexture;
    private Mesh m_characterMesh;

    void OnGUI () 
    {
        m_characterName = EditorGUILayout.TextField(m_characterName);
        int.TryParse(EditorGUILayout.TextField(m_characterPrice.ToString()), out m_characterPrice);

        GUILayout.BeginHorizontal();

        m_characterIcon = ToolsUtils.TextureField("Icon", m_characterIcon);
        m_characterTexture = ToolsUtils.TextureField("Texture", m_characterTexture);

        GUILayout.EndHorizontal();

        m_characterMesh = ToolsUtils.MeshField("FBX", m_characterMesh);

        if (GUILayout.Button("Generate"))
        {
            ChangeTextureType(m_characterTexture, TextureImporterType.Sprite);
            GenerateStoreCharacterPrefab();

        }
    }

    private void GenerateStoreCharacterPrefab()
    {
        
    }

    private void AddCharacterToStore()
    {

    }

    private void ChangeTextureType(Texture2D texture, TextureImporterType type)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
        importer.textureType = type;
        importer.SaveAndReimport();
    }
}
