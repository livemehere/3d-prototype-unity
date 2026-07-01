using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxAlive = 5;

    private float timer;
    private int aliveCount;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < spawnInterval || aliveCount >= maxAlive)
        {
            return;
        }

        timer = 0f;
        Spawn();
    }

    private void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        aliveCount++;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement)
        {
            movement.player = player;
        }

        Heath health = enemy.GetComponent<Heath>();
        if (health)
        {
            health.OnDied += HandleEnemyDied;
        }
    }

    private void HandleEnemyDied()
    {
        aliveCount--;
    }
}
