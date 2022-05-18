using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public enum ClickType
    {
        Default,
        Close,
        Accept,
        Reject,
    }
    ClickType _type;
    public ClickType Type
    {
        get { return _type; }
    }

    bool _clicked = false;
    public bool Clicked
    {
        get { return _clicked; }
        set { _clicked = value; }
    }

    public bool OnClick()
    {
        return _clicked;
    }

    public void OnClickNext()
    {
        _clicked = true;
        _type = ClickType.Default;
    }

    public void OnClickAccept()
    {
        _clicked = true;
        _type = ClickType.Accept;
    }

    public void OnClickReject()
    {
        _clicked = true;
        _type = ClickType.Reject;
    }

    public void OnClickClose()
    {
        _clicked = true;
        _type = ClickType.Close;
    }


    bool _clickedForInventory = false;
    public bool ClickedForInventory
    {
        get { return ClickedForInventory; }
    }
    public void OnClickSlot()
    {
        _clickedForInventory = true;
    }
}
