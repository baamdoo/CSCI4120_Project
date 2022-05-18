using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Image _image;
    [SerializeField]
    ItemManager _itemManager;

    [SerializeField]
    public bool isInventorySlot;
    [SerializeField]
    public int slotIndex;

    Item _item;
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                if (isInventorySlot)
                    _image.color = new Color(0, 0, 0, 0);
                else
                    _image.color = new Color(0, 0, 0, 0.4f);
            }
            else
            {
                _image.sprite = Item.sprite;
                _image.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isInventorySlot)
                _itemManager.Equip(slotIndex);
            else
                _itemManager.Unequip(slotIndex);
        }
    }
}
