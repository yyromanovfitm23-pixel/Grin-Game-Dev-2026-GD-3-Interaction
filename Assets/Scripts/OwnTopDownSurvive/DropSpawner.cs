using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    private GameObject healerBox;
    private GameObject hematohyBox;
    private Vector3 min;
    private Vector3 max;
    private Bounds dropSpawnerBounds;
    private Renderer dropSpawnerRenderer;
    private float randX;
    private float randY;
    private float randChance;

    public float chanceOfDropHealBox = 20f;

    void Start()
    {
        dropSpawnerRenderer = GetComponent<Renderer>();
        dropSpawnerBounds = dropSpawnerRenderer.bounds;

        healerBox = Resources.Load<GameObject>("Objects/HealBox");
        hematohyBox = Resources.Load<GameObject>("Objects/HematohyBox");

        min = dropSpawnerBounds.min;
        max = dropSpawnerBounds.max;

        randChance = Random.Range(1, 100);

        if (randChance <= chanceOfDropHealBox)
        {
            DropSpawn(healerBox);
        }
        DropSpawn(hematohyBox);

        Destroy(gameObject);
    }

    void DropSpawn(GameObject spawnObject)
    {
        randX = Random.Range(min.x, max.x);
        randY = Random.Range(min.y, max.y);

        Instantiate(spawnObject, new Vector3(randX, randY, 0), Quaternion.identity);
    }
}
