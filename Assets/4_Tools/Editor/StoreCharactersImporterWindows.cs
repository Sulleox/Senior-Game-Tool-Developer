using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class StoreCharactersImporterWindows : EditorWindow
{
    [MenuItem("Tools/Character Assembler")]
    [MenuItem("Assets/Open Character Assembler")]
    public static void  ShowWindow () 
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(StoreCharactersImporterWindows));
        window.minSize = new Vector2(300f, 450f);
        window.title = "Character Importer";
        ToolsUtils.CharacterDB.Init();
    }

    private int OPTIMIZE_TEXTURE_SIZE = 512;

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

    void OnGUI()
    {
        //Name && Price && Shop Order
        m_characterName = EditorGUILayout.TextField("Name :", m_characterName);
        int.TryParse(EditorGUILayout.TextField("Price :", m_characterPrice.ToString()), out m_characterPrice);
        int.TryParse(EditorGUILayout.TextField("Shop Order :", m_characterShopOrder.ToString()), out m_characterShopOrder);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //FBX
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        m_characterMesh = ToolsUtils.GameObjectField("FBX", m_characterMesh);
        if (m_characterMesh != null)
        {
            Editor.CreateCachedEditor(m_characterMesh, null, ref m_meshPreview);
            m_meshPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(ToolsUtils.FIELD_SIDE_LENGTH, ToolsUtils.FIELD_SIDE_LENGTH), GUIStyle.none);
            if (m_characterMaterial == null)
            {
                m_characterMaterial = m_characterMesh.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial;
            }
        }
        GUILayout.EndVertical();

        //Material
        GUILayout.BeginVertical();
        m_characterMaterial = ToolsUtils.MaterialField("Material", m_characterMaterial);
        if (m_characterMaterial != null)
        {
            Editor.CreateCachedEditor(m_characterMaterial, null, ref m_materialPreview);
            m_materialPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(ToolsUtils.FIELD_SIDE_LENGTH, ToolsUtils.FIELD_SIDE_LENGTH), GUIStyle.none);
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Icon
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        m_characterIcon = ToolsUtils.SpriteField("Icon", m_characterIcon);
        if (m_characterPrefab != null || m_characterMesh != null)
        {
            if (GUILayout.Button("Generate Icon"))
            {
                CharacterIconGenerator iconGeneratorWindow = EditorWindow.GetWindow<CharacterIconGenerator>();
                iconGeneratorWindow.Load(m_characterPrefab ? m_characterPrefab : GenerateCharacterPrefab(true), m_characterName, GetIconFromGenerator);
            }
        }
        GUILayout.EndVertical();

        //Textures
        GUILayout.BeginVertical();
        m_characterTexture = ToolsUtils.TextureField("Texture", m_characterTexture);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


        //Animator && Avatar
        GUILayout.BeginHorizontal();
        m_characterAnimator = ToolsUtils.AnimatorControllerField("Controller", m_characterAnimator);
        m_characterAvatar = ToolsUtils.AvatarField("Avatar", m_characterAvatar);
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.BeginHorizontal();
        if (m_selectedCharacter == null)
        {
            if (GUILayout.Button("Create Character"))
            {
                if (m_characterTexture != null)
                    ChangeTextureType();

                m_characterPrefab = GenerateCharacterPrefab(true);
                ToolsUtils.CharacterDB.AddCharacter(m_characterName, m_characterPrice, m_characterPrefab, m_characterShopOrder, m_characterIcon, m_characterMaterial, m_characterAvatar, m_characterAnimator);
            }

            if (GUILayout.Button("Load Character"))
                EditorGUIUtility.ShowObjectPicker<CharacterEntry>(null, false, "", 1);

            if (Event.current.commandName == "ObjectSelectorUpdated")
                if (EditorGUIUtility.GetObjectPickerControlID() == 1)
                {
                    m_selectedCharacter = (CharacterEntry)EditorGUIUtility.GetObjectPickerObject();
                    LoadCharacterValues();
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

    private void LoadCharacterValues()
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

    private GameObject GenerateCharacterPrefab(bool withoutIcon = false)
    {
        string prefabPath = $"{ToolsPaths.CHARACTER_PREFAB_PATH}/{m_characterName}.prefab";

        GameObject characterGameObject = Instantiate(m_characterMesh);
        Animator characterAnimator = ToolsUtils.GetOrAddAnimator(characterGameObject);
        characterAnimator.runtimeAnimatorController = m_characterAnimator;
        characterAnimator.avatar = m_characterAvatar;
        AddAndFitColliderToMesh(characterGameObject);

        SetMaterialAndTexture(characterGameObject);
        GameObject characterPrefab = PrefabUtility.SaveAsPrefabAsset(characterGameObject, prefabPath);
        DestroyImmediate(characterGameObject);

        if (m_characterIcon == null && !withoutIcon)
        {
            if (EditorUtility.DisplayDialog("Sprite Generation", "Do you want to auto generate an Icon for the shop ?", "Yes", "No"))
            {
                CharacterIconGenerator iconGeneratorWindow = EditorWindow.GetWindow<CharacterIconGenerator>();
                iconGeneratorWindow.Load(m_characterPrefab ? m_characterPrefab : GenerateCharacterPrefab(true), m_characterName, GetIconFromGenerator);
            }
        }
        return characterPrefab;
    }

    private void GetIconFromGenerator(string iconPath)
    {
        m_characterIcon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
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
        Material[] characterSharedMaterials = meshRenderer.sharedMaterials;
        for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
        {
            characterSharedMaterials[i] = m_characterMaterial;
            if (m_characterTexture != null)
            {
                characterSharedMaterials[i].SetTexture("_MainTex", m_characterTexture);
            }
        }
        meshRenderer.sharedMaterials = characterSharedMaterials;
    }

    private void AddAndFitColliderToMesh(GameObject character)
    {
        CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
        SkinnedMeshRenderer meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();

        collider.center = meshRenderer.bounds.center;
        collider.radius = meshRenderer.bounds.size.x / 2;
        collider.height = meshRenderer.bounds.size.y;
    }

    public void LoadPossibleCharacter(GameObject characterPrefab)
    {
        m_characterName = characterPrefab.name;
        m_characterPrefab = characterPrefab;
        m_characterIcon = null;

        SkinnedMeshRenderer skinnedMeshRenderer = characterPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        m_characterMaterial = skinnedMeshRenderer.sharedMaterial;
        m_characterTexture = (Texture2D) skinnedMeshRenderer.sharedMaterial.mainTexture;
        m_characterMesh = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh));

        Animator animator = characterPrefab.GetComponent<Animator>();
        m_characterAvatar = animator.avatar;
        m_characterAnimator = (AnimatorController) animator.runtimeAnimatorController;
    }
}
