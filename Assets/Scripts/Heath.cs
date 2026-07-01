using UnityEngine;

public class Heath : MonoBehaviour
{
    public int hp = 3;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}