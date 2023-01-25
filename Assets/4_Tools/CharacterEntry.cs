using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CharacterEntry : ScriptableObject
{
    public CharacterEntry(string name, int price, GameObject prefab, int shopPriority = int.MaxValue)
    {
        Name = name;
        Price = price;
        Prefab = prefab;
        ShopPriority= shopPriority;
    }

    public string Name { get; private set; }
    public int Price { get; private set; }
    public GameObject Prefab { get; private set; }
    public int ShopPriority { get; private set; }
}
