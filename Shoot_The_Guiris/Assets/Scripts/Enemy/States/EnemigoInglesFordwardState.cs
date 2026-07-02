using UnityEngine;


public class EnemigoInglesFordwardState : StateMachineBehaviour
{

    private EnemigoIngles enemigoIngles;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Recuperamos la refrencia al enemy propiietario de animator
        enemigoIngles = animator.GetComponentInParent<EnemigoIngles>();
        //enemigoIngles.Agent.speed = enemigoIngles.VelocidadBusqueda;
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Si el navmesh agent esta activo y existe en objeto
        if (enemigoIngles.AgentIsActive && enemigoIngles.HasTarget)
        {
            enemigoIngles.SetDestinationToTarget();
        }
        if (!enemigoIngles.PathPending && enemigoIngles.RemainingDistanceToTarget < enemigoIngles.InRange)
        {
            animator.SetBool("EnRango", true);
            //enemigoIngles.Agent.speed = enemigoIngles.VelocidadEnRango;
        }
        else
        {
            animator.SetBool("EnRango", false);
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {

    // }

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
