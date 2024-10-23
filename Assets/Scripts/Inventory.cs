using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<KeyType> keys = new List<KeyType>();

    public List<KeyType> Keys { get { return keys; } }

    public void AddKey(KeyType key)
    {
        keys.Add(key);
    }

    public bool HasKey(KeyType type)
    {
        return keys.Contains(type);
    }
}
