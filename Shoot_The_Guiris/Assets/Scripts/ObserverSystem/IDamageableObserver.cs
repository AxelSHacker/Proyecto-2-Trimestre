using UnityEngine;

public interface IDamageableObserver
{
    public void OnHealtUpdate(float currentealt, float maxealt);
    public void OnHit();
    public void OnDead();
}
