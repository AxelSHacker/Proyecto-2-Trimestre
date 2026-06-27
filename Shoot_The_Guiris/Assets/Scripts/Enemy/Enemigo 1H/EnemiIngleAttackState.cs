
using UnityEngine;

public class EnemiIngleAttackState : StateMachineBehaviour
{
   private EnemigoIngles enemigoIngles;
   int _espadaEscudo = -1;
   float _velocidadAtaque = 1;

   // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
   override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      enemigoIngles = animator.GetComponentInParent<EnemigoIngles>();
      _espadaEscudo = Random.Range(0, 2);
      _velocidadAtaque = Random.Range(0.7f, 1.3f);
   }
   // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
   override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      if (enemigoIngles == null) return;
      float distancia = enemigoIngles.RemainingDistanceToTarget;

      Vector3 dirrecion = enemigoIngles.Target.position - enemigoIngles.transform.position;
      enemigoIngles.transform.rotation = Quaternion.LookRotation(dirrecion);

      if (distancia < enemigoIngles.AttacDistance)
      {
         animator.SetInteger("EspadaEscudo", _espadaEscudo);
         animator.SetFloat("ReproduccionVelocidad", _velocidadAtaque);
      }
   }

   // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
   override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      _espadaEscudo = -1;
      _velocidadAtaque = 1;
   }

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



























