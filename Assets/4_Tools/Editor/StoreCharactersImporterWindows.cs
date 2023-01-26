using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class StoreCharactersImporterWindows : EditorWindow
{
    [MenuItem("Tools/Character Assembler")]
    [MenuItem("Assets/Open Character Assembler")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(StoreCharactersImporterWindows));
        ToolsUtils.CharacterDB.Init();
    }

    private const int OPTIMIZE_TEXTURE_SIZE = 512;

    private string m_characterName = "Name";
    private int m_characterPrice = 100;
    private int m_characterShopOrder = 10;
    private GameObject m_characterMesh;
    private Sprite m_characterIcon;
    private Texture2D m_characterTexture;
    private Material m_characterMaterial;
    private AnimatorController m_characterAnimator;
    private Avatar m_characterAvatar;

    private GameObject m_characterPrefab = null;
    private CharacterEntry m_selectedCharacter = null;

    private Editor m_meshPreview = null;
    private Editor m_materialPreview = null;

    void OnGUI () 
    {
        //Name && Price && Shop Order
        m_characterName = ToolsUtils.TextField("Name :",m_characterName);
        int.TryParse(ToolsUtils.TextField("Price :", m_characterPrice.ToString()), out m_characterPrice);
        int.TryParse(ToolsUtils.TextField("Shop Order :", m_characterShopOrder.ToString()), out m_characterShopOrder);

        //Icon
        GUILayout.BeginHorizontal();
        m_characterIcon = ToolsUtils.SpriteField("Icon", m_characterIcon);

        //Textures
        GUILayout.BeginVertical();
        m_characterTexture = ToolsUtils.TextureField("Texture", m_characterTexture);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        //FBX
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        m_characterMesh = ToolsUtils.GameObjectField("FBX", m_characterMesh);
        if (m_meshPreview == null)
            m_meshPreview = Editor.CreateEditor(m_characterMesh);
        m_meshPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(ToolsUtils.FIELD_SIDE_LENGTH, ToolsUtils.FIELD_SIDE_LENGTH), GUIStyle.none);
        GUILayout.EndVertical();

        //Material
        GUILayout.BeginVertical();
        m_characterMaterial = ToolsUtils.MaterialField("Material", m_characterMaterial);
        if (m_materialPreview == null)
            m_materialPreview = Editor.CreateEditor(m_characterMaterial);
        m_materialPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(ToolsUtils.FIELD_SIDE_LENGTH, ToolsUtils.FIELD_SIDE_LENGTH), GUIStyle.none);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        //Animator && Avatar
        GUILayout.BeginHorizontal();
        m_characterAnimator = ToolsUtils.AnimatorControllerField("Controller", m_characterAnimator);
        m_characterAvatar = ToolsUtils.AvatarField("Avatar", m_characterAvatar);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (m_selectedCharacter == null)
        {
            if (GUILayout.Button("Create New"))
            {
                if (m_characterTexture != null)
                    ChangeTextureType();

                m_characterPrefab = GenerateCharacterPrefab();
                ToolsUtils.CharacterDB.AddCharacter(m_characterName, m_characterPrice, m_characterPrefab, m_characterShopOrder, m_characterIcon, m_characterMaterial, m_characterAvatar, m_characterAnimator);
            }

            if (GUILayout.Button("Load Character"))
                EditorGUIUtility.ShowObjectPicker<CharacterEntry>(null, false, "", 1);

            if (Event.current.commandName == "ObjectSelectorUpdated")
                if (EditorGUIUtility.GetObjectPickerControlID() == 1)
                {
                    m_selectedCharacter = (CharacterEntry) EditorGUIUtility.GetObjectPickerObject();
                    UpdateCharacterValues();
                }
        }
        else
        {
            if (GUILayout.Button("Update Existing"))
            {
                ToolsUtils.CharacterDB.UpdateCharacter(m_characterName, m_characterPrice, m_characterPrefab, m_characterShopOrder, m_characterIcon, m_characterMaterial, m_characterAvatar, m_characterAnimator);
                SetMaterialAndTexture(m_characterPrefab);
            }

            if (GUILayout.Button("Unload Character"))
            {
                m_selectedCharacter = null;
                m_characterName = string.Empty;
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear Character DB"))
        {
            if (EditorUtility.DisplayDialog("Confirmation Dialog", "Are you sure that you want to delete the whole character database ?", "Yes", "No"))
            {
                ToolsUtils.CharacterDB.CheckCharacters();
            }
        }
    }

    private void UpdateCharacterValues()
    {
        m_characterName = m_selectedCharacter.Name;
        m_characterPrice = m_selectedCharacter.Price;
        m_characterShopOrder = m_selectedCharacter.ShopOrder;
        m_characterPrefab = m_selectedCharacter.Prefab;
        m_characterIcon = m_selectedCharacter.Icon;
        m_characterMaterial =  m_selectedCharacter.Material;
        m_characterAvatar = m_selectedCharacter.Avatar;
        m_characterAnimator = m_selectedCharacter.Animator;
    }

    private GameObject GenerateCharacterPrefab()
    {
        if (m_characterIcon == null)
            m_characterIcon = CreateCharacterIcon();

        string prefabPath = $"{ToolsPaths.CHARACTER_PREFAB_PATH}/{m_characterName}.prefab";

        GameObject characterGameObject = Instantiate(m_characterMesh);
        Animator characterAnimator = ToolsUtils.GetOrAddAnimator(characterGameObject);
        characterAnimator.runtimeAnimatorController = m_characterAnimator;
        characterAnimator.avatar = m_characterAvatar;
        AddAndFitColliderToMesh(characterGameObject);

        SetMaterialAndTexture(characterGameObject);
        GameObject characterPrefab = PrefabUtility.SaveAsPrefabAsset(characterGameObject, prefabPath);
        DestroyImmediate(characterGameObject);
        return characterPrefab;
    }

    private Sprite CreateCharacterIcon()
    {
        //Load a scene
        //Import Prefab in the scene
        //Take camera screenshot
        return null;
    }

    private void ChangeTextureType()
    {
        string path = AssetDatabase.GetAssetPath(m_characterTexture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

        if (EditorUtility.DisplayDialog("Optimize choice", 
            "Do you want to optimize your texture ? (Set the texture size to 512px, remove MipMaps, and set Filtermode to Bilinear", 
            "Optimize", "Go without change"))
        {
            importer.maxTextureSize = OPTIMIZE_TEXTURE_SIZE;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
        }
            
        importer.textureType = TextureImporterType.Sprite;
        importer.SaveAndReimport();
    }

    private void SetMaterialAndTexture(GameObject character)
    {
        SkinnedMeshRenderer meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
        {
            meshRenderer.sharedMaterials[i] = m_characterMaterial;
            if (m_characterTexture != null)
            {
                meshRenderer.sharedMaterials[i].SetTexture("_MainTex", m_characterTexture);
            }
        }

    }

    private void AddAndFitColliderToMesh(GameObject character)
    {
        CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
        SkinnedMeshRenderer meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();

        collider.center = meshRenderer.bounds.center;
        collider.radius = meshRenderer.bounds.size.x / 2;
        collider.height = meshRenderer.bounds.size.y;
    }
}
