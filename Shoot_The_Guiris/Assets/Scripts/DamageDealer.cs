using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    int damage;


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
        {
            other.gameObject.TryGetComponent<Healt>(out Healt _healt);

            _healt.Damage(damage);
        }
    }
}  

