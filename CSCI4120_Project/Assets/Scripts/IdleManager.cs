using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : StateMachineBehaviour
{
    public int state = 2;
    public float minExeTime = 0f;
    public float maxExeTime = 0.5f;

    public float randomTime;

    readonly int hashRandomIdle = Animator.StringToHash("RandomIdle");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        randomTime = Random.Range(minExeTime, maxExeTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            animator.SetInteger(hashRandomIdle, -1);

        if (stateInfo.normalizedTime > randomTime && !animator.IsInTransition(0))
            animator.SetInteger(hashRandomIdle, Random.Range(0, state+1));
    }
}
