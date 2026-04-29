using System;
using UnityEngine;

public class EnemigoAnimationEvents : MonoBehaviour
{
    
    [SerializeField] MeleeWeapons _armaScript;
    [SerializeField] MeleeWeapons _armaScriptLeft;
    [SerializeField] EnemigoIngles _enemigoIngles;
    [SerializeField] PlayerControler _playerControler;

    
    public void StartAttack()
    {
        if (_armaScript != null) _armaScript.StartAtaque();
    }

    public void StopAttack()
    {
        if (_armaScript != null) _armaScript.StopAtaque();
    }
    public void StartAttackLeft()
    {
        if (_armaScriptLeft != null) _armaScriptLeft.StartAtaque();
    }

    public void StopAttackLeft()
    {
        if (_armaScriptLeft != null) _armaScriptLeft.StopAtaque();
    }
    public void PlayerRealoading()
    {
        if (_playerControler != null) _playerControler.EvetoRecarga();
    }
    public void ReturnToPool()
    {
        if (_enemigoIngles != null) _enemigoIngles.ReturnToPool();
        _enemigoIngles.AnimatorDeactivate();
    }

    public void AtaqueSaltoCoroutina()
    {
        if (_enemigoIngles != null) _enemigoIngles.AtaqueSaltoCoroutina();
    }

}
