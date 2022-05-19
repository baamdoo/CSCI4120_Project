using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonController : MonoBehaviour
{
    public Rigidbody prefabBullet;
    public Transform shootPosition;
    

    [SerializeField]
    Animator _animator;

    [SerializeField]
    float _scanRange = 10.0F;
    [SerializeField]
    float _attackRange = 2.0f;
    [SerializeField]
    float _scanDegree = 60.0f;
    public float rotationDamping = 6.0f;
    public float shootForce;

    private NavMeshAgent _agent;
    

    readonly int _stateHash = Animator.StringToHash("State");

    public ParticleSystem fire;

    bool _reset;


    bool _isAttacked = false;
    public bool Attacked
    {
        get { return _isAttacked; }
        set { _isAttacked = value; }
    }

    Status _stat;
    GameObject _target;
    PreDefine.State _state;
    public PreDefine.State State
    {
        get { return _state; }
        set
        {
            _state = value;
            _animator.SetInteger(_stateHash, (int)_state);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        //turn off fire particle system
        fire.Stop();

        _agent = GetComponent<NavMeshAgent>();
        
        _stat = gameObject.GetComponent<Status>();
        // StartCoroutine("attackOrMove");
        // _agent.isStopped = true;
        State = PreDefine.State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Player");
        if (enemy == null || !enemy.activeSelf)
            return;
        Vector3 heading = enemy.transform.position - transform.position;

        bool attackFlag = isInAttackRange(transform.position, enemy.transform.position);

        if (State == PreDefine.State.Move || State == PreDefine.State.Idle || Attacked)
        {
            if (isInRange(transform.position, enemy.transform.position))
            {
                _agent.isStopped = true;
                Debug.LogFormat("{0}: Player detected!", gameObject.name);
                _target = enemy;

                if (attackFlag)
                    State = PreDefine.State.Attack;
                else
                    State = PreDefine.State.Chase;
            }
            else
            {
                State = PreDefine.State.Idle;
            }
            
            
        }

        else if (State == PreDefine.State.Chase)
        {
            if (attackFlag)
            {
                _agent.isStopped = true;
                if (_stat.HP < _stat.MaxHP / 2)
                {
                    State = PreDefine.State.Special;
                }
                else
                {
                    
                    State = PreDefine.State.Attack;
                }
            }
            else
            {
                _agent.isStopped = false;
                _agent.speed = _stat.Speed * 2f;
                _agent.SetDestination(enemy.transform.position);
                _agent.Move(_agent.velocity * Time.deltaTime);
            }
        }
        else if (State == PreDefine.State.Attack)
        {
            _agent.ResetPath();

            if (attackFlag)
            {
                Quaternion rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);

     
            }
            else
                State = PreDefine.State.Chase;
        }

        if (_stat.HP <= 0)
        {
            fire.Stop();
            State = PreDefine.State.Die;
        }
    }

    IEnumerator attackOrMove()
    {
        while (true)
        {
            if (State == PreDefine.State.Move)
            {
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                GameObject enemy = GameObject.FindGameObjectWithTag("Player");
                Vector3 heading = enemy.transform.position - transform.position;

                if (isInRange(transform.position, enemy.transform.position) && checkVisibility(heading))
                {
                    transform.LookAt(enemy.transform.position);
                    Rigidbody instanceBullet = Instantiate(prefabBullet, shootPosition.position + shootPosition.forward * 2.5f, shootPosition.rotation);
                    instanceBullet.GetComponent<Rigidbody>().AddForce(shootPosition.forward * shootForce);
                }
                else
                {
                    _state = PreDefine.State.Idle;
                    _agent.isStopped = false;
                }

                yield return new WaitForSeconds(1.0f);
            }
        }
    }

    bool isInRange(Vector3 from, Vector3 to)
    {
        Vector3 heading = to - from;
        if (heading.sqrMagnitude > _scanRange * _scanRange)
            return false;

        Vector3 normal = Vector3.Normalize(heading);
        float cos = Vector3.Dot(normal, transform.forward);
        return cos >= Mathf.Cos(_scanDegree * Mathf.Deg2Rad);
    }
    bool checkVisibility(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit))
        {
            Debug.LogFormat("{0} {1}", hit.collider.tag, hit.collider.gameObject.name);
            if (hit.collider.tag == "Player")
                return true;
        }
        return false;
    }

    bool isInAttackRange(Vector3 from, Vector3 to)
    {
        Vector3 heading = to - from;
        if (heading.sqrMagnitude > _attackRange * _attackRange)
            return false;

        return true;
    }

    void HitEvent()
    {
        if (_target != null && _target.GetComponent<PlayerController>().State != PreDefine.State.Dodge)
        {
            Status myStat = gameObject.GetComponent<Status>();
            Status targetStat = _target.GetComponent<Status>();

            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defence);
            targetStat.HP -= damage;

            if (targetStat.HP <= 0)
            {
                StartCoroutine(timeDuration());
                return;
            }
        }

        if (_target.GetComponent<PlayerController>().State != PreDefine.State.Dodge)
            State = PreDefine.State.Attack;
        else
            State = PreDefine.State.Chase;
    }

    //turens on and off the dragons fire breath
    void Fire()
    {
        Debug.Log("Fire");
        if (fire.isPlaying)
        {
            fire.Stop();
        }
        else
        {
            fire.Play();
        }
    }

    IEnumerator timeDuration()
    {
        yield return new WaitForSeconds(6f);
        State = PreDefine.State.Move;
        Attacked = false;
        
    }
}
