using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreDefine
{
    public enum Layer
    {
        UI = 5,
        Ground = 8,
        Wall = 9,
        Player = 10,
        Monster = 11,
        NPC = 12,
    }

    public enum State
    {
        Idle,
        Move,
        Attack,
        Chase,
        Die,
        Dodge
    }
}
