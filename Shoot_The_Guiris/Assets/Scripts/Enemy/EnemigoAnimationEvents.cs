using UnityEngine;

public class EnemigoAnimationEvents : MonoBehaviour
{
    
    [SerializeField] private MeleeWeapons _armaScript; 

    
    public void StartAttack()
    {
        if (_armaScript != null) _armaScript.StartAtaque();
    }

    public void StopAttack()
    {
        if (_armaScript != null) _armaScript.StopAtaque();
    }
}
