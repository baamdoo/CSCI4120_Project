using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStatus : Status
{
    int _SpecialATT;
    public int SpecialAttack
    {
        get { return _SpecialATT; }
    }
    private void Start()
    {
        _LEV = 1;
        _HP = 1000;
        _maxHP = 1000;
        _ATT = 50;
        _SpecialATT = 100;
        _DEF = 20;
        _SP = 1.5f;
    }
}
