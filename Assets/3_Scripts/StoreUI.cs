using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField]
    UIStoreItem itemUIPrefab;
    void Start()
    {
        foreach (CharacterEntry character in Store.Instance.StoreCharacters)
        {
            UIStoreItem listItem = Instantiate(itemUIPrefab, transform);
            listItem.Initialize(character);
        }
    }
}
