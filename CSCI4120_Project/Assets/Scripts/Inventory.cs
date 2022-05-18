using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    Transform _box;
    Slot[] _slots;

    public List<Item> itemList;
    private Item[] _items;

    private void OnValidate()
    {
        _slots = _box.GetComponentsInChildren<Slot>();
        _items = new Item[itemList.Count];
    }

    private void Awake()
    {
        Add((int)PreDefine.ItemType.Potion);
        Reload();
    }

    public void Reload()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (i < _items.Length)
                _slots[i].Item = _items[i];
            else
                _slots[i].Item = null;
        }
    }

    public void Add(int idx)
    {
        if (_items.Length >= _slots.Length)
        {
            Debug.Log("Slot is full!");
            return;
        }
        else
        {
            _items[idx] = itemList[idx];
            Reload();
        }
    }

    public void Remove(int idx)
    {
        _items[idx] = null;
        Reload();
    }
}
