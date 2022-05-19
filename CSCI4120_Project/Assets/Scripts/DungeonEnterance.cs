using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonEnterance : MonoBehaviour
{
    public string DungeonName;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("test");
            SceneManager.LoadScene(DungeonName);
        }
    }
}
