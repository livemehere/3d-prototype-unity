using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float knockbackDamping = 8f;
    [SerializeField] private float minKnockbackSpeed = 0.05f;

    private NavMeshAgent agent;
    private Vector3 knockbackVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            agent.SetDestination(player.position);
        }

        if (knockbackVelocity.sqrMagnitude > minKnockbackSpeed * minKnockbackSpeed)
        {
            agent.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(
                knockbackVelocity,
                Vector3.zero,
                knockbackDamping * Time.deltaTime
            );
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }
    }

    public void Knockback(Vector3 dir, float force)
    {
        knockbackVelocity = dir.normalized * force;
    }
}
