using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
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

    CharacterController _controller;
    NavMeshAgent _agent;
    Camera _camera;
    [SerializeField]
    Animator _animator;

    [SerializeField]
    public float groundCheckDist = 0.3f;

    readonly int _stateHash = Animator.StringToHash("State");

    [SerializeField]
    GameObject _inventory;
    bool _open = false;

    PlayerStatus _stat;
    LayerMask _mask;

    [SerializeField]
    Texture2D[] _cursors;

    GameObject _target = null;

    [SerializeField]
    GameObject _gameover;
    FadeScript _fade;
    Vector3 _initPos = new Vector3(1.65f, 0, -5.0f);

    bool _isAttack = false;
    bool _isDead = false;
    bool _isDodge = false;

    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStatus>();

        _controller = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = true;

        _camera = Camera.main;

        _fade = _gameover.GetComponent<FadeScript>();
    }

    void Update()
    {
        if (!_isDead)
        {
            SetMouseCursor();

            if (Input.GetMouseButtonDown(0) && !_isDodge)
            {
                OnMouseLeftClicked();
            }

            if (Input.GetMouseButtonDown(1))
            {
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

            if (Input.GetKeyDown(KeyCode.I))
            {
                _open = !_open;
                _inventory.SetActive(_open);

                // string itemName = "Axe";
                // _inventory.GetComponent<Inventory>().GetItem(itemName);
                // _inventory.GetComponent<Inventory>().Add(item);
            }

            if (_stat.HP <= 0)
            {
                _isDead = true;
                State = PreDefine.State.Die;
                _fade.Fade();
                StartCoroutine(timeDuration());
            }
        }
    }

    IEnumerator timeDuration()
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
        _mask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Monster");

        if (Physics.Raycast(ray, out hit, 100, _mask))
        {
            if (hit.collider.gameObject.layer == (int)PreDefine.Layer.Ground)
            {
                Debug.Log("We hit" + hit.collider.name + " " + hit.point);
                _agent.SetDestination(hit.point);
                _target = null;

                _isAttack = false;
            }
            else if (hit.collider.gameObject.layer == (int)PreDefine.Layer.Monster && hit.collider.gameObject.GetComponent<MonsterController>().State != PreDefine.State.Die)
            {
                Debug.Log("We hit monster!");
                _target = hit.collider.gameObject;
                _agent.SetDestination(hit.point);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(hit.point - transform.position), 90 * Time.deltaTime);

                _isAttack = true;
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
        }
    }

    void SetMouseCursor()
    {
        if (_state == PreDefine.State.Die)
            return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _mask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Monster");

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
        }

        State = PreDefine.State.Idle;
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
}
