using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    private GameObject enemyDropSpawn;
    private Transform transform;
    private Transform playerTransform;
    private LayerMask playerLayerMask;
    private CircleCollider2D collider;
    private Rigidbody2D enemyRb;
    private float enemyNextAttackTime;
    private float enemyNextReverseTime;
    private float angle;
    private Vector2 enemyDirection;
    private Vector2 enemyRotDirection;
    private Collider2D hit;

    public float movingSpeed = 3f;
    public float enemyDamage = 5f;
    public float enemyDamageCooldown = 1f;
    public float enemyReverseCooldown = 1f;
    public float enemyHp = 100f;
    public bool isPushed = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        transform = GetComponent<Transform>();
        playerTransform = player.GetComponent<Transform>();
        playerLayerMask = 1 << player.layer;
        collider = GetComponent<CircleCollider2D>();
        enemyRb = GetComponent<Rigidbody2D>();
        enemyDropSpawn = Resources.Load<GameObject>("Objects/DropSpawner");
    }

    void Update()
    {
        EnemyFollowing();
        EnemyAttack();

        if (isPushed && Time.time >= enemyNextReverseTime)
        {
            StartCoroutine(CallCooldown(enemyReverseCooldown));
        }
    }

    IEnumerator CallCooldown(float enemyReverseCooldown)
    {
        yield return new WaitForSeconds(enemyReverseCooldown);
        isPushed = false;
    }

    void EnemyFollowing()
    {
        enemyDirection = (player.transform.position - transform.position).normalized;

        switch (isPushed)
        {
            case false:
                enemyRb.MovePosition(enemyRb.position + enemyDirection * movingSpeed * Time.deltaTime);
                movingSpeed = 6f;
                break;
            case true:
                enemyRb.MovePosition(enemyRb.position + (enemyDirection * (-1)) * movingSpeed * Time.deltaTime);
                movingSpeed = 2f;
                break;
        }

        enemyRotDirection = playerTransform.position - transform.position;

        angle = Mathf.Atan2(enemyRotDirection.y, enemyRotDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void EnemyAttack()
    {
        if (collider.isTrigger)
        {
            hit = Physics2D.OverlapCircle(
                transform.position,
                collider.radius,
                playerLayerMask
            );
        }
        

        if (hit != null && Time.time >= enemyNextAttackTime)
        {
            Player player_ = player.GetComponent<Player>();

            if (player_ != null)
            {
                player_.TakeDamage(enemyDamage);

                enemyNextAttackTime = Time.time + enemyDamageCooldown;
            }
        }
    }

    public void TakeDamage(float playerDamage)
    {
        enemyHp -= playerDamage;

        if (enemyHp <= 0)
        {
            Instantiate(enemyDropSpawn, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
