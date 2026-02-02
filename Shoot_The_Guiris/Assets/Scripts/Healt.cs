using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Healt : MonoBehaviour
{
    public int enemyHealth;
    public int health;
    public int maxHealt;
    public bool noDamage = false;


    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial Platform")
        {
            
            health = DataManager.Instance.tutorialScore;
        }
        else
        {
            health = DataManager.Instance.actualGameScore;
        }
        enemyHealth = maxHealt;
    }


    public void Damage(int damage)
    {
        if (noDamage) return;
        
        if (health >= 0)
        {
            health -= damage;

        }

        if (enemyHealth >= 0)
        {
            enemyHealth -= damage;

        }
        //Sonido de espada
    }
}

