using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public bool ReadyToTalk = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            ReadyToTalk = true;
    }
    private void OnTriggerExit(Collider other)
    {
        ReadyToTalk = false;
    }
}
