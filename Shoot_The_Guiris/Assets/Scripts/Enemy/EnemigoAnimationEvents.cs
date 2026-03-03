using UnityEngine;

public class EnemigoAnimationEvents : MonoBehaviour
{
    
    [SerializeField] MeleeWeapons _armaScript;
    [SerializeField] EnemigoIngles _enemigoIngles;

    
    public void StartAttack()
    {
        if (_armaScript != null) _armaScript.StartAtaque();
    }

    public void StopAttack()
    {
        if (_armaScript != null) _armaScript.StopAtaque();
    }
    public void ReturnToPool()
    {
        if (_enemigoIngles != null) _enemigoIngles.ReturnToPool();
        _enemigoIngles.AnimatorDeactivate();
    }
}
