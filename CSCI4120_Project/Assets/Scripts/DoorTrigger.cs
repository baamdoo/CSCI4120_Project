using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorEvent doorEvent;
    public DoorController doorController;

    public bool autoClose = true;

    private void OnTriggerEnter(Collider other)
    {
        doorEvent.OpenDoor(doorController.id);
    }
    private void OnTriggerExit(Collider other)
    {
        doorEvent.CloseDoor(doorController.id);
    }
}
