using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    
    public PreDefine.DialogueStage Stage;

    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject _npc;

    [SerializeField]
    GameObject[] _quests;
    [SerializeField]
    GameObject _pDialogue;
    GameObject _dialogue;
    [SerializeField]
    GameObject _questManager;

    ButtonScript _buttonScript;
    int _qIdx = 0;
    int _dIdx = 0;

    bool _availQuest = false;
    public bool AvailQuest
    {
        get { return _availQuest; }
        set { _availQuest = value; }
    }

    bool _proceding = false;

    void Start()
    {
        Stage = PreDefine.DialogueStage.FirstBefore;
    }

    void Update()
    {
        int qN = _quests[_qIdx].transform.childCount;
        if (_npc.GetComponent<NPCManager>().ReadyToTalk && _player.GetComponent<PlayerController>().ReadyToTalk)
        {
            if (_questManager.GetComponent<QuestManager>().Complete == 1 && _qIdx == 0)
            {
                AvailQuest = false;
                Stage = PreDefine.DialogueStage.FirstAfter;
                _qIdx = 1;
            }
            else if (_questManager.GetComponent<QuestManager>().Complete == 2 && _qIdx == 2)
            {
                AvailQuest = false;
                Stage = PreDefine.DialogueStage.SecondAfter;
                _qIdx = 3;
            }

            if (AvailQuest)
            {
                _pDialogue.SetActive(true);
                return;
            }

            _dialogue = _quests[_qIdx].transform.GetChild(_dIdx).gameObject;
            if (!_dialogue.activeSelf)
                _dialogue.SetActive(true);
            _buttonScript = _dialogue.GetComponentInChildren<Button>().GetComponent<ButtonScript>();
            if (_buttonScript.OnClick())
            {
                _dialogue.SetActive(false);
                if (_buttonScript.Type == ButtonScript.ClickType.Close)
                {
                    _dIdx = 0;
                    _buttonScript.Clicked = false;
                    _player.GetComponent<PlayerController>().ReadyToTalk = false;
                }
                else
                {
                    _buttonScript.Clicked = false;
                    _dIdx++;
                    if (_dIdx == qN)
                    {
                        if (_buttonScript.Type == ButtonScript.ClickType.Accept)
                        {
                            AvailQuest = true;
                            if (_qIdx == 2)
                                Stage = PreDefine.DialogueStage.SecondBefore;
                        }
                        _dIdx = 0;
                        _player.GetComponent<PlayerController>().ReadyToTalk = false;
                        if (_qIdx == 1)
                            _qIdx = 2;
                        else if (_qIdx == 3)
                            Stage = PreDefine.DialogueStage.Finish;
                    }
                }
            }
        }

        else
        {
            ResetManager();
        }
    }

    void ResetManager()
    {
        if (_pDialogue.activeSelf)
            _pDialogue.SetActive(false);
        if (_quests[_qIdx].transform.GetChild(_dIdx).gameObject.activeSelf)
            _quests[_qIdx].transform.GetChild(_dIdx).gameObject.SetActive(false);
        _dIdx = 0;
    }
}
