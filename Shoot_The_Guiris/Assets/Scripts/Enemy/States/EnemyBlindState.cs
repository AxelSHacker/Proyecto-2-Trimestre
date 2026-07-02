using UnityEngine;
using UnityEngine.AI;

public class EnemyBlindState : StateMachineBehaviour
{
    NavMeshAgent _agenteEnemigo;
    EnemigoIngles _enemigo;
    bool _check = false;
    float _originalSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_agenteEnemigo == null) _agenteEnemigo = animator.GetComponentInParent<NavMeshAgent>();

        if (_enemigo == null) _enemigo = animator.GetComponentInParent<EnemigoIngles>();

        _check = false;
        _originalSpeed = _agenteEnemigo.speed;
        _agenteEnemigo.speed = _originalSpeed * 0.5f;

        _enemigo.CalculoZonaBusqueda1();
        _enemigo.CalculoZonaBusqueda2();

        if (_enemigo.ComprobarZonas(_enemigo._destino1))
        {
            _agenteEnemigo.SetDestination(_enemigo._destinoFinal);
        }
        animator.SetLayerWeight(1, 0f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_agenteEnemigo.pathPending ) return;
        //comprobamos si llegamos al primer destinoFinal
        {
            if (_agenteEnemigo.remainingDistance <= _agenteEnemigo.stoppingDistance + 0.1f)
            {
                //Si llegamos a un punto pero todavia no es el punto 1
                if (!_check)
                {
                    _check = true;
                    //Cambiamos el destinoFineal
                    if (_enemigo.ComprobarZonas(_enemigo._destino2))
                    {
                        _agenteEnemigo.SetDestination(_enemigo._destinoFinal);
                     
                    }
                }
            }
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agenteEnemigo.speed = _originalSpeed;
        animator.SetLayerWeight(1, 1f);
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
