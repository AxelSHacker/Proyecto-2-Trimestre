using UnityEngine;

public class EnemigoInglesFordwardState : StateMachineBehaviour
{

    private EnemigoIngles enemigoIngles;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Recuperamos la refrencia al enemy propiietario de animator
        enemigoIngles = animator.GetComponentInParent<EnemigoIngles>();


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        //Debug.Log(enemigoIngles.HasTarget);
        //Si el navmesh agent esta activo y existe en objeto
        if (enemigoIngles.AgentIsActive && enemigoIngles.HasTarget)
        {
            enemigoIngles.SetDestinationToTarget();

            Vector3 dirrecion = enemigoIngles.Target.position - enemigoIngles.transform.position;

            enemigoIngles.transform.rotation = Quaternion.Slerp(enemigoIngles.transform.rotation,
                                                               Quaternion.LookRotation(dirrecion),
                                                               Time.deltaTime);
        }
        if (!enemigoIngles.PathPending && enemigoIngles.RemainingDistanceToTarget < enemigoIngles.InRange)
        {
            animator.SetBool("EnRango", true);
        }


    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
