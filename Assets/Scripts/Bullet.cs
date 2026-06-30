using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float knockbackForce = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var em = other.gameObject.GetComponentInParent<EnemyMovement>();
            if (em)
            {
                Vector3 knockbackDir = transform.forward;
                knockbackDir.y = 0;
                knockbackDir.Normalize();
                em.Knockback(knockbackDir, knockbackForce);
            }
        }

        Destroy(gameObject);
    }
}