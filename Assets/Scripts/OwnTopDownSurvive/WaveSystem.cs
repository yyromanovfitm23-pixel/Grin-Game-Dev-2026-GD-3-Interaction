using UnityEngine;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    private GameObject[] spawners;
    private GameObject enemy;

    private Vector3 min;
    private Vector3 max;
    private Bounds spawnerBounds;
    private Renderer spawnerRenderer;
    private int randIndex;
    private float randX;
    private float randY;

    public float spawnCooldown = 10f;
    public float descendSpawnCooldownSpeed = 0.1f;

    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        enemy = Resources.Load<GameObject>("Objects/Enemy");

        SpawnEnemy();
        StartCoroutine(DelaySpawnEnemy());
        StartCoroutine(DescendDelaySpawnEnemy());
    }

    void SpawnEnemy()
    {
        randIndex = Random.Range(0, spawners.Length);

        spawnerRenderer = spawners[randIndex].GetComponent<Renderer>();
        spawnerBounds = spawnerRenderer.bounds;

        min = spawnerBounds.min;
        max = spawnerBounds.max;

        randX = Random.Range(min.x, max.x);
        randY = Random.Range(min.y, max.y);

        Instantiate(enemy, new Vector3(randX, randY, 0), Quaternion.identity);
    }

    IEnumerator DelaySpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);
            SpawnEnemy();
        }
    }

    IEnumerator DescendDelaySpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            spawnCooldown -= descendSpawnCooldownSpeed;
        }
    }
}
