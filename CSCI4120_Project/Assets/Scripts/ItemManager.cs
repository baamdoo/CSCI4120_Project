using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    Inventory _inventory;
    [SerializeField]
    Equipment _equipment;
    [SerializeField]
    GameObject _player;

    bool[] _isEquipped = new bool[15];

    public void Equip(int idx)
    {
        if (idx > 0 && !_isEquipped[idx])
        {
            _inventory.Remove(idx);
            _equipment.Add(idx);
            _player.GetComponent<PlayerController>().EquipPlayer(idx);
            _isEquipped[idx] = true;
        }
    }
    public void Unequip(int idx)
    {
        if (idx > 0 && _isEquipped[idx])
        {
            _equipment.Remove(idx);
            _inventory.Add(idx);
            _player.GetComponent<PlayerController>().UnequipPlayer(idx);
            _isEquipped[idx] = false;
        }
    }
}
