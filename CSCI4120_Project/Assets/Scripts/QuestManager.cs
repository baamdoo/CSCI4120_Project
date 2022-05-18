using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    GameObject _questpanel;
    [SerializeField]
    Text _script;
    [SerializeField]
    GameObject _dialogueManager;

    GameObject[] _monsters;
    GameObject _boss;

    int _complete = 0;
    public int Complete
    {
        get { return _complete; }
        set { _complete = value; }
    }

    uint hunted = 0;

    void Start()
    {
        _monsters = GameObject.FindGameObjectsWithTag("Monster");
        _boss = GameObject.FindGameObjectWithTag("Boss");
        _script.fontSize = 14;
    }

    void Update()
    {
        if (_dialogueManager.GetComponent<DialogueManager>().Stage == PreDefine.DialogueStage.FirstBefore)
        {
            if (_dialogueManager.GetComponent<DialogueManager>().AvailQuest)
                _questpanel.SetActive(true);

            if (_questpanel.activeSelf)
            {
                uint cnt = 0;
                foreach (GameObject go in _monsters)
                {
                    if (go.GetComponent<MonsterController>().State == PreDefine.State.Die)
                        cnt++;
                }
                _script.text = "Hunted Hydra (" + cnt.ToString() + " / " + _monsters.Length.ToString() + ")";
                hunted = cnt;
            }

            if (hunted == _monsters.Length)
            {
                Complete = 1;
                hunted = 0;
            }
        }
        else if (_dialogueManager.GetComponent<DialogueManager>().Stage == PreDefine.DialogueStage.SecondBefore)
        {
            if (_dialogueManager.GetComponent<DialogueManager>().AvailQuest)
                _questpanel.SetActive(true);

            if (_questpanel.activeSelf)
            {
                uint cnt = 0;
                if (_boss.GetComponent<MonsterController>().State == PreDefine.State.Die)
                    cnt++;
                _script.text = "Kill the boss! (" + cnt.ToString() + " / 1)";
                hunted = cnt;
            }

            if (hunted == 1)
                Complete = 2;
        }
        else
        {
            _questpanel.SetActive(false);
        }
    }
}
