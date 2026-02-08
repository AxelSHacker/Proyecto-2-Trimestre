using System;
using UnityEngine;

public interface IDamageabe<T>
{
    public float Maxhealt { get; }
    public float Currentealt{ get; }
    public bool IsDead { get; }
    public void TakeDamag(T damage, Vector3 impactPoint = default(Vector3));
}
