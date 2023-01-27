using HomaGames.Internal.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StoreItem : ScriptableObject
{
    public int Id;
    public string Name;
    public int Price;
    public Sprite Icon;
    public GameObject Prefab;
}

public class Store : Singleton<Store>
{
    public CharacterDatabase CharacterDatabase;
    public List<CharacterEntry> StoreCharacters
    {
        get
        {
            return CharacterDatabase.m_characters.OrderBy(x => x.ShopOrder).ToList();
        }
    }
    public Action<StoreItem> OnItemSelected;

    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }
}
