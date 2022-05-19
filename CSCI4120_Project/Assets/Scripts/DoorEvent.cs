﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event System", menuName = "Event System/Door Event")]
public class DoorEvent : ScriptableObject
{
    [NonSerialized]
    public Action<int> OnOpenDoor;
    [NonSerialized]
    public Action<int> OnCloseDoor;

    public void OpenDoor(int id)
    {
        OnOpenDoor?.Invoke(id);
    }
    public void CloseDoor(int id)
    {
        OnCloseDoor?.Invoke(id);
    }
}
