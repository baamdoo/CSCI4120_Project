using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    [SerializeField]
    protected int _EXP;
    [SerializeField]
    protected float _range;
    [SerializeField]
    protected int _MP;

    public int Exp { get { return _EXP; } set { _EXP = value; } }
    public float Range { get { return _range; } set { _range = value; } }
    public int Stamina { get { return _MP; } set { _MP = value; } }

    private void Start()
    {
        _LEV = 1;
        _HP = 100;
        _MP = 100;
        _maxHP = 100;
        _ATT = 20;
        _DEF = 10;
        _SP = 2.5f;
    }
}
