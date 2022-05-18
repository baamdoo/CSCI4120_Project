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

    public void Equip(int idx)
    {
        if (idx > 0)
        {
            _inventory.Remove(idx);
            _equipment.Add(idx);
            _player.GetComponent<PlayerController>().EquipPlayer(idx);
        }
    }
    public void Unequip(int idx)
    {
        if (idx > 0)
        {
            _equipment.Remove(idx);
            _inventory.Add(idx);
            _player.GetComponent<PlayerController>().UnequipPlayer(idx);
        }
    }
}
