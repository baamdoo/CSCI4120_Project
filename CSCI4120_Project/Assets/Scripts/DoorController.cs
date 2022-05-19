using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorEvent doorEventObject;

    public int id = 0;
    public float openOffset = 10f;
    public float closeOffset = 1f;

    private void OnEnable()
    {
        doorEventObject.OnOpenDoor += OnOpenDoor;
        doorEventObject.OnCloseDoor += OnCloseDoor;
    }

    private void OnDisable()
    {
        doorEventObject.OnOpenDoor -= OnOpenDoor;
        doorEventObject.OnCloseDoor -= OnCloseDoor;
    }

    public void OnOpenDoor(int id)
    {
        if (id != this.id)
            return;

        StopAllCoroutines();
        StartCoroutine(OpenDoor());
    }
    public void OnCloseDoor(int id)
    {
        if (id != this.id)
            return;

        StopAllCoroutines();
        StartCoroutine(CloseDoor());
    }

    IEnumerator OpenDoor()
    {
        while (transform.position.y < openOffset)
        {
            Vector3 pos = transform.position;
            pos.y += 0.05f;
            transform.position = pos;

            yield return null;
        }
    }
    IEnumerator CloseDoor()
    {
        while (transform.position.y > closeOffset)
        {
            Vector3 pos = transform.position;
            pos.y -= 0.05f;
            transform.position = pos;

            yield return null;
        }
    }
}
