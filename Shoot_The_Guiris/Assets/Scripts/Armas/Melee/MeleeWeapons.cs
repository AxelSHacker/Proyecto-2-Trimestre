using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapons : CustomMonoBehaviour
{
   public Action OnInitialize;
   public Action<Vector3> OnImpact;
   [SerializeField] float _damage;
   [SerializeField] LayerMask _shootableLayers;
   [SerializeField] Vector3 _tamanioCaja = new Vector3(0.5f, 0.5f, 0.5f);
   [SerializeField] Vector3 _offset = new Vector3(0f, 0f, 1f);
   private bool _atacando;
   List<GameObject> _impactado = new List<GameObject>();

   public override void EditorInit()
   {

   }
   void Update()
   {
      if (_atacando)
      {
         CheckContacto();
      }
   }
   void OnDrawGizmos()
    {
        if (_atacando) Gizmos.color = Color.red;
        else Gizmos.color = Color.yellow;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(_offset), transform.rotation, _tamanioCaja);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
   //Llamada en el animation event para comenzar
   public void StartAtaque()
   {
      _atacando = true;
      _impactado.Clear();
   }
   //Llamada en el animation event para Trminar
   public void StopAtaque()
   {
      _atacando = false;
   }
   //Funcion que activa el overlapboxx y comprueba si colisiona
   private void CheckContacto()
   {
      Vector3 centro = transform.TransformPoint(_offset);
      Collider[] colliders = Physics.OverlapBox(centro, _tamanioCaja / 2, transform.rotation, _shootableLayers);

      foreach (var other in colliders)
      {
         if (_impactado.Contains(other.gameObject)) continue;

         if ((_shootableLayers & (1 << other.gameObject.layer)) != 0 && other.TryGetComponent(out IDamageabe<float> damageable))
         {
            damageable.TakeDamag(_damage, transform.position);
            OnImpact?.Invoke(transform.position);

            // if (other.TryGetComponent(out CharacterController component))
            // {

            //    Vector3 direccion = (other.transform.position - transform.position).normalized;
            //    direccion.y = 0.1f;
            //    component.Move(direccion * _fuerzaEscudo);
            // }
            _impactado.Add(other.gameObject);
         }
      }
   }
    void OnDisable()
    {
        _atacando = false;
        _impactado.Clear();
    }

    // void OnTriggerEnter(Collider other)
    // {
    //    if ((_shootableLayers & (1 << other.gameObject.layer)) != 0 && other.TryGetComponent(out IDamageabe<float> damageable))
    //    {
    //       damageable.TakeDamag(_damage, transform.position);

    //       if (other.TryGetComponent(out CharacterController component))
    //       {

    //          Vector3 direccion = (other.transform.position - transform.position).normalized;
    //          direccion.y = 0.1f;
    //          component.Move(direccion * _fuerzaEscudo);
    //       }
    //       OnImpact?.Invoke(transform.position);
    //    }
    // }
}














