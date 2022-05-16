using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    Transform _box;
    [SerializeField]
    Slot[] _slots;

    public List<Item> itemList;
    private List<Item> _items;

    private void OnValidate()
    {
        _slots = _box.GetComponentsInChildren<Slot>();
        _items = new List<Item>();
    }

    private void Awake()
    {
        Add(itemList[8]);
        Reload();
    }

    public void Reload()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (i < _items.Count)
                _slots[i].item = _items[i];
            else
                _slots[i].item = null;
        }
    }

    public void Add(Item item)
    {
        if (_items.Count >= _slots.Length)
        {
            Debug.Log("Slot is full!");
            return;
        }
        else
        {
            _items.Add(item);
            Reload();
        }
    }
}
