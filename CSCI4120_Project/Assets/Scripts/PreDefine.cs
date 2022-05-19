﻿using System.Collections;
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
        Boss = 13,
    }

    public enum State
    {
        Idle,
        Move,
        Attack,
        Chase,
        Die,
        Dodge,
        Special,
    }

    public enum DialogueStage
    {
        FirstBefore,    // 00
        FirstAfter,     // 01
        SecondBefore,   // 10
        SecondAfter,    // 11
        Finish
    }

    public enum ItemType
    {
        Potion,
        Helmet,
        Chest,
        Pants,
        Gloves,
        Boots,
        Cape,
        Sword,
    }
}
