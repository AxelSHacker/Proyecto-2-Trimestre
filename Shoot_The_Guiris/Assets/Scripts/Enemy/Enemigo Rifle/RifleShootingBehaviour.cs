using UnityEngine;

public class RifleShootingBehaviour : StateMachineBehaviour
{
    private EnemigoIngles enemigoIngles;
     public float _velocidadAtaque;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemigoIngles = animator.GetComponentInParent<EnemigoIngles>();

        animator.SetFloat("ReproduccionVelocidad", 1f / _velocidadAtaque);

        Quaternion rotation = Quaternion.LookRotation(enemigoIngles.transform.forward);
        PoolManager.Instance.Pull("ProyectilEnemigo", enemigoIngles.Posicion.position, rotation);
       enemigoIngles.DisparoRealizado();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       _velocidadAtaque = 1;
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
