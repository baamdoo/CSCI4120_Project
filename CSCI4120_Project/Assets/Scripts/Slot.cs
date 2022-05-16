using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    Image _image;

    Item _item;
    public Item item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
                _image.color = new Color(1, 1, 1, 0);
            else
            {
                _image.sprite = item.sprite;
                _image.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
