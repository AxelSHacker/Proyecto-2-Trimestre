using UnityEngine;

public interface PlayerObserver
{
    public void OnHealtUpdate(float currentealt, float maxealt);
    public void OnHit();
    public void OnDead();
    public void OnAtaqueEspecial(float timer, float time);
    public void OnDasch(float timer, float time);
}
