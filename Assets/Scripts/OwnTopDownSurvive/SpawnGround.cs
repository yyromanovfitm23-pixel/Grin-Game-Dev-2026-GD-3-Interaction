using UnityEngine;

public class SpawnGround : MonoBehaviour
{
    private GameObject[] grounds;
    private GameObject[] spawners;
    private Transform groundSpawnerTransform;
    private Collider2D hit;
    private BoxCollider2D groundSpawnerCollider;
    private int randIndex;

    public LayerMask groundLayerMask;

    void Start()
    {

        spawners = GameObject.FindGameObjectsWithTag("GroundSpawner");
        grounds = new GameObject[] { 
            Resources.Load<GameObject>("Objects/backyard_00"),
            Resources.Load<GameObject>("Objects/backyard_01"),
            Resources.Load<GameObject>("Objects/backyard_02"),
            Resources.Load<GameObject>("Objects/backyard_03")
        };
    }

    void Update()
    {
        SpawnGround_();
    }

    void SpawnGround_()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            groundSpawnerCollider = spawners[i].GetComponent<BoxCollider2D>();

            hit = Physics2D.OverlapBox(
                groundSpawnerCollider.bounds.center,
                groundSpawnerCollider.size,
                0f,
                groundLayerMask
            );

            if (hit == null)
            {
                groundSpawnerTransform = spawners[i].GetComponent<Transform>();

                randIndex = Random.Range(0, grounds.Length);

                Instantiate(grounds[randIndex], new Vector3(groundSpawnerTransform.position.x, groundSpawnerTransform.position.y, 0), Quaternion.identity);
            }
        }
    }
}
