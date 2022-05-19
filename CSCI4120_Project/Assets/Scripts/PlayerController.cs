using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    struct Potion
    {
        public int cooldown;
        public int capacity;
    }
    Potion _potion;

    PreDefine.State _state = PreDefine.State.Idle;
    public PreDefine.State State
    {
        get { return _state; }
        set
        {
            _state = value;
            _animator.SetInteger(_stateHash, (int)_state);
        }
    }
    PlayerStatus _stat;
    public PlayerStatus PlayerStatus
    {
        get { return _stat; }
    }
    [SerializeField]
    Text _playerStatus;

    NavMeshAgent _agent;
    Camera _camera;
    [SerializeField]
    Animator _animator;

    [SerializeField]
    public float groundCheckDist = 0.3f;

    readonly int _stateHash = Animator.StringToHash("State");
    readonly int _fasterHash = Animator.StringToHash("Faster");

    [SerializeField]
    GameObject _inventory;
    [SerializeField]
    GameObject _equipment;
    bool _added = false;
    [SerializeField]
    GameObject _sword;

    LayerMask _mask;

    [SerializeField]
    Texture2D[] _cursors;

    GameObject _target = null;

    [SerializeField]
    GameObject _gameover;
    FadeScript _fade;
    Vector3 _initPos = new Vector3(1.65f, 0, -5.0f);

    int _dodgeCooldown = 1;
    bool _fastAttack = false;
    bool _dodgeEnabled = false;
    bool _isAttack = false;
    bool _isDead = false;
    bool _isDodge = false;
    bool _readyToTalk = false;
    public bool ReadyToTalk
    {
        get { return _readyToTalk; }
        set { _readyToTalk = value; }
    }
    bool _isTalking = false;
    public bool Talking
    {
        get { return _isTalking; }
        set { _isTalking = value; }
    }

    [SerializeField]
    GameObject _dialogueManager;
    #endregion

    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStatus>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = true;

        _camera = Camera.main;

        _fade = _gameover.GetComponent<FadeScript>();

        _potion.cooldown = 10;
        _potion.capacity = 20;

        InvokeRepeating("ManageCooldown", 0f, 1f);
    }

    void Update()
    {
        if (Talking || _isDead)
            return;

        SetMouseCursor();

        if (Input.GetMouseButtonDown(0) && !_isDodge)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            OnMouseLeftClicked();
        }

        if (Input.GetMouseButtonDown(1) && _dodgeEnabled && _dodgeCooldown == 0)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            OnMouseRightClicked();
        }

        if (!_isAttack)
        {
            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _agent.Move(_agent.velocity * Time.deltaTime);
                if (_isDodge)
                    State = PreDefine.State.Dodge;
                else
                    State = PreDefine.State.Move;
            }
            else
            {
                _agent.Move(Vector3.zero);
                _agent.velocity = Vector3.zero;
                State = PreDefine.State.Idle;
                if (_isDodge)
                {
                    _agent.speed = _stat.Speed;
                    _isDodge = false;
                }
            }

        }
        else
        {
            if (_agent.remainingDistance > _agent.stoppingDistance + _stat.Range)
            {
                _agent.Move(_agent.velocity * Time.deltaTime);
                State = PreDefine.State.Move;
            }
            else
            {
                _agent.Move(Vector3.zero);
                _agent.velocity = Vector3.zero;
                State = PreDefine.State.Attack;
            }
        }

        if (_stat.HP <= 0)
        {
            _isDead = true;
            State = PreDefine.State.Die;
            _fade.Fade();
            StartCoroutine(DelayTime());
        }

        if (_dialogueManager.GetComponent<DialogueManager>().Stage == PreDefine.DialogueStage.SecondBefore)
        {
            if (!_added)
            {
                for (int i = 0; i <= (int)PreDefine.ItemType.Sword; i++)
                    _inventory.GetComponent<Inventory>().Add(i);
                _added = true;
            }
        }
        else if (_dialogueManager.GetComponent<DialogueManager>().Stage == PreDefine.DialogueStage.Finish)
        {
            Debug.Log("Mission Complete!");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory.SetActive(!_inventory.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetStatusText();
            _equipment.SetActive(!_equipment.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && _potion.cooldown == 0)
        {
            _stat.HP = Mathf.Min(_stat.MaxHP, _stat.HP + _potion.capacity);
            _potion.cooldown = 10;
        }
    }

    void ManageCooldown()
    {
        _potion.cooldown = Mathf.Max(0, _potion.cooldown - 1);
        _dodgeCooldown = Mathf.Max(0, _dodgeCooldown - 1);
        SetStatusText();
    }

    IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
        ResetPlayer();
    }

    private void LateUpdate()
    {
        transform.position = _agent.nextPosition;
    }

    void OnMouseLeftClicked()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _mask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Monster") | LayerMask.GetMask("NPC");

        if (Physics.Raycast(ray, out hit, 100, _mask))
        {
            if (hit.collider.gameObject.layer == (int)PreDefine.Layer.Ground)
            {
                Debug.Log("We hit" + hit.collider.name + " " + hit.point);
                _agent.SetDestination(hit.point);
                _target = null;

                _isAttack = false;
                _readyToTalk = false;
            }
            else if (hit.collider.gameObject.layer == (int)PreDefine.Layer.Monster && hit.collider.gameObject.GetComponent<Animator>().GetInteger("State") != (int)PreDefine.State.Die)
            {
                Debug.Log("We hit monster!");
                _target = hit.collider.gameObject;
                _agent.SetDestination(hit.point);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(hit.point - transform.position), 90 * Time.deltaTime);

                _isAttack = true;
                _readyToTalk = false;
            }
            else if (hit.collider.gameObject.layer == (int)PreDefine.Layer.NPC)
            {
                Debug.Log("We hit monster!");
                _agent.SetDestination(hit.point);

                _readyToTalk = true;
            }
        }
    }

    void OnMouseRightClicked()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hit, 100, _mask))
        {
            Vector3 dir = (hit.point - transform.position).normalized;

            Debug.Log("Dodge to" + dir * 5f);
            _agent.acceleration = 200f;
            _agent.speed = 4.5f;
            _agent.SetDestination(transform.position + dir * 4.5f);
            _target = null;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 90 * Time.deltaTime);

            _isAttack = false;
            _isDodge = true;
            _dodgeCooldown = 1;
        }
    }

    void SetMouseCursor()
    {
        if (_state == PreDefine.State.Die)
            return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _mask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Monster") | LayerMask.GetMask("NPC") | LayerMask.GetMask("UI");

        if (Physics.Raycast(ray, out hit, 100, _mask))
        {
            if (hit.collider.gameObject.layer == (int)PreDefine.Layer.Monster)
            {
                Texture2D cursorTex = _cursors[1];
                Cursor.SetCursor(cursorTex, new Vector2(cursorTex.width / 5, 0), CursorMode.Auto);
            }
            else if (hit.collider.gameObject.layer == (int)PreDefine.Layer.NPC || hit.collider.gameObject.layer == (int)PreDefine.Layer.UI)
            {
                Texture2D cursorTex = _cursors[2];
                Cursor.SetCursor(cursorTex, new Vector2(cursorTex.width / 3, 0), CursorMode.Auto);
            }
            else
            {
                Texture2D cursorTex = _cursors[0];
                Cursor.SetCursor(cursorTex, new Vector2(cursorTex.width / 3, 0), CursorMode.Auto);
            }
        }
    }

    void HitEvent()
    {
        if (_target != null)
        {
            Status myStat = gameObject.GetComponent<Status>();
            Status targetStat = _target.GetComponent<Status>();

            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defence);
            targetStat.HP -= damage;
            if (_target.tag == "Monster")
                _target.GetComponent<MonsterController>().Attacked = true;
            else if (_target.tag == "Boss")
                _target.GetComponent<DragonController>().Attacked = true;
        }
        _isAttack = false;

        _agent.ResetPath();
    }

    private void ResetPlayer()
    {
        transform.position = _initPos;
        _stat.HP = 100;
        State = PreDefine.State.Idle;
        gameObject.SetActive(true);
        _isDead = false;
    }

    public void EquipPlayer(int idx)
    {
        switch (idx)
        {
            case (int)PreDefine.ItemType.Helmet:
                _stat.MaxHP += 100;
                _stat.HP += 100;
                break;
            case (int)PreDefine.ItemType.Chest:
                _stat.Defence += 10;
                break;
            case (int)PreDefine.ItemType.Pants:
                _stat.Defence += 10;
                break;
            case (int)PreDefine.ItemType.Gloves:
                _animator.SetBool(_fasterHash, true);
                _fastAttack = true;
                break;
            case (int)PreDefine.ItemType.Boots:
                _stat.Speed += 1.0f;
                break;
            case (int)PreDefine.ItemType.Cape:
                _dodgeEnabled = true;
                break;
            case (int)PreDefine.ItemType.Sword:
                _stat.Range += 1.0f;
                _stat.Attack += 30;
                _sword.SetActive(true);
                break;
        }
        SetStatusText();
    }
    public void UnequipPlayer(int idx)
    {
        switch (idx)
        {
            case (int)PreDefine.ItemType.Helmet:
                _stat.HP -= 100;
                _stat.MaxHP -= 100;
                break;
            case (int)PreDefine.ItemType.Chest:
                _stat.Defence -= 10;
                break;
            case (int)PreDefine.ItemType.Pants:
                _stat.Defence -= 10;
                break;
            case (int)PreDefine.ItemType.Gloves:
                _animator.SetBool(_fasterHash, false);
                _fastAttack = false;
                break;
            case (int)PreDefine.ItemType.Boots:
                _stat.Speed -= 1.0f;
                break;
            case (int)PreDefine.ItemType.Cape:
                _dodgeEnabled = false;
                break;
            case (int)PreDefine.ItemType.Sword:
                _stat.Range -= 1.0f;
                _stat.Attack -= 30;
                _sword.SetActive(false);
                break;
        }
        SetStatusText();
    }

    private void SetStatusText()
    {
        string text = "  Player Status \n\n\t";
        text += "HP: " + _stat.MaxHP.ToString() + "\n\t";
        text += "ATT: " + _stat.Attack.ToString() + "\n\t";
        text += "DEF: " + _stat.Defence.ToString() + "\n\t";
        text += "Speed: " + _stat.Speed.ToString() + "\n\t";
        if (_fastAttack)
            text += "ATT Speed: Fast\n\t";
        else
            text += "ATT Speed: Slow\n\t";
        if (_dodgeEnabled)
            text += "Skill: Dodge\n\t";
        else
            text += "Skill: None\n\t";
        text += "ATT Range: " + _stat.Range.ToString() + "\n\t";
        text += "Potion cooldown: " + _potion.cooldown.ToString();

        _playerStatus.text = text;
    }
}
