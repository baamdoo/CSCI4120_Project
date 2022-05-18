using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField]
    Transform _box;
    Slot[] _slots;

    public List<Item> itemList;
    public Item[] items;

    private void OnValidate()
    {
        _slots = _box.GetComponentsInChildren<Slot>();
        items = new Item[itemList.Count];
    }

    public void Reload()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (i < items.Length)
                _slots[i].Item = items[i];
            else
                _slots[i].Item = null;
        }
    }

    public void Add(int idx)
    {
        if (idx > 0)
        {
            items[idx - 1] = itemList[idx - 1];
            Reload();
        }
    }

    public void Remove(int idx)
    {
        if (idx > 0)
        {
            items[idx - 1] = null;
            Reload();
        }
    }
}
