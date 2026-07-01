using UnityEngine;
using System;

public class Heath : MonoBehaviour
{
    public int hp = 3;
    public event Action OnDied;

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
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}
