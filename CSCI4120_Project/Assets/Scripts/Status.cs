using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField]
    protected int _LEV;
    [SerializeField]
    protected int _HP;
    [SerializeField]
    protected int _maxHP;
    [SerializeField]
    protected int _ATT;
    [SerializeField]
    protected int _DEF;
    [SerializeField]
    protected float _SP;

    public int Level { get { return _LEV; } set { _LEV = value; } }
    public int HP { get { return _HP; } set { _HP = value; } }
    public int MaxHP { get { return _maxHP; } set { _maxHP = value; } }
    public int Attack { get { return _ATT; } set { _ATT = value; } }
    public int Defence { get { return _DEF; } set { _DEF = value; } }
    public float Speed { get { return _SP; } set { _SP = value; } }

    private void Start()
    {
        _LEV = 1;
        _HP = 1;
        _maxHP = 1;
        _ATT = 20;
        _DEF = 5;
        _SP = 1f;
    }
}
