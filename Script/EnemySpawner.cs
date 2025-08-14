using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public float spawnInterval = 2f; 
    public float spawnRadius = 5f;   

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
    
        Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
