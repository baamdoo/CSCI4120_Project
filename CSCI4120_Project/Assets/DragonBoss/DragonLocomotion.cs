using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class DragonLocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform playerTransform;
    
    //field of view
    public float scanRange;
    public float peripheralVision;

    public float health;
    public float maxHealth;
    
    public LayerMask tragetMask;
    public LayerMask obstructoinMask;

    public bool canSeePlayer = false;

    public float maxDistanceFromPlayer;
    public float attackDistance;

    public ParticleSystem fire;

    GameObject _target;
    Status _stat;

    float speed = 0.0f;
    int speedHash;
    
   Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());
        StartCoroutine(locomotion());
        animator = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");

        _stat = GetComponent<Status>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HitEvent()
    {
    
    }

    private IEnumerator locomotion()
    {
        while (true)
        {
            if (_stat.HP == 0)
            {
                animator.SetBool("Dead", true);
                break;
            }
            if (canSeePlayer)
            {
                //rotate towards player
                Vector3 PlayerDirection = playerTransform.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(PlayerDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                //move towards player
                
                if (Vector3.Distance(transform.position, playerTransform.position) > maxDistanceFromPlayer)
                {
                    agent.speed = Mathf.Lerp(6, speed, Time.deltaTime * 5f);

                }
                else
                {

                    agent.speed = Mathf.Lerp(agent.speed, 0, Time.deltaTime * 5f);
                    //attack
                    if(Vector3.Distance(transform.position, playerTransform.position) < attackDistance && health>(maxHealth/2))
                    {
                        animator.SetTrigger("Attack1");
                    }
                    else if(Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
                    {
                        animator.SetTrigger("FireBreath");

                       
                        
                        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("breath fire"))
                        {
                            fire.Play();
                            yield return new WaitForSeconds(0.08f);
                        }
                        else
                        {
                            fire.Stop();
                        }
                    }
                }
                agent.SetDestination(playerTransform.position);

                speed = agent.speed;
                animator.SetFloat(speedHash, speed);
            }
            yield return null;
        }
    }
    
    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, scanRange, tragetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < peripheralVision / 2)
            {

                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstructoinMask))
                {
                    canSeePlayer = true;
                    break;
                }
            }
        }
    }
}
