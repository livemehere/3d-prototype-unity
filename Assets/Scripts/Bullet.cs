using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;
    public float knockbackForce = 1f;
    public GameObject particle;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var em = collision.gameObject.GetComponentInParent<EnemyMovement>();
            if (em)
            {
                Vector3 knockbackDir = transform.forward;
                knockbackDir.y = 0;
                knockbackDir.Normalize();
                em.Knockback(knockbackDir, knockbackForce);
                Heath hp = collision.gameObject.GetComponentInParent<Heath>();
                if (hp)
                {
                    hp.TakeDamage(damage);
                }
            }
        }

        CreateParticle(collision.contacts[0]);
        Destroy(gameObject);
    }

    void CreateParticle(ContactPoint contact)
    {
        var rotation = Quaternion.LookRotation(contact.normal);
        Instantiate(particle, contact.point, rotation);
    }
}