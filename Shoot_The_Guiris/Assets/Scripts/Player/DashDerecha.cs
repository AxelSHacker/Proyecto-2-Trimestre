using UnityEngine;

public class DashDerecha : StateMachineBehaviour
{
    public Vector3 direccionDasch;
    public float _daschForce = 1f;
    private CharacterController _cC;
    PlayerControler playerControler;
    Vector3 inicioDasch;
    Vector3 destinoDasch;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _cC = animator.GetComponentInParent<CharacterController>();
        playerControler = animator.GetComponentInParent<PlayerControler>();

        inicioDasch = animator.transform.position;
        Vector3 dirMundoDasch = animator.transform.TransformDirection(direccionDasch);
        destinoDasch = animator.transform.position + (dirMundoDasch * playerControler.DashForce);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float progress = stateInfo.normalizedTime;

        Vector3 posicionFinalDasch = Vector3.Lerp(inicioDasch, destinoDasch, progress);
        
        if (progress <= 1f)
        {
            _cC.Move(posicionFinalDasch - animator.transform.position);
        }

    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
