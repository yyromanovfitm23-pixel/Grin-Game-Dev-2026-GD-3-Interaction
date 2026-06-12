using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    private GameObject enemy;
    private GameObject healBox;
    private Transform transform;
    private Transform cameraTransform;
    private Transform cameraCursorTransform;
    private Transform groundSpawnerListTransform;
    private Vector3 mousePos;
    private Vector3 playerPos;
    private Vector3 cameraCursorPos;
    private Vector2 rotDirection;
    private Vector2 movDirection;
    private Rigidbody2D playerRb;
    private BoxCollider2D attackCollider;
    private Collider2D hit;
    private Collider2D healerHit;
    private Collider2D hematohyHit;
    private CircleCollider2D cameraZoneCollider;
    private float angle;
    private float playerNextAttackTime;
    private string minutes;
    private string seconds;
    private static string scoreText;
    private static float score;

    public float playerHematomahyLostSpeed = 0.1f;
    public float playerSecondTimer = 0f;
    public float playerMinuteTimer = 0f;

    public float movingSpeed = 5f;
    public float playerHp = 100f;
    public float playerHematomahyAmount = 100f;
    public float playerDamage = 25f;
    public float playerDamageCooldown = 1f;
    public LayerMask enemyLayerMask;
    public LayerMask healBoxLayerMask;
    public LayerMask hematohyBoxLayerMask;
    public GameObject camera;
    public GameObject cameraCursor;
    public GameObject attackZone;
    public GameObject cameraZone;
    public GameObject groundSpawnerList;
    public Text surviveTimeText;
    public Text bestSurviveTimeText;
    public Text playerHpText;
    public Text playerHematohyText;

    void Start()
    {
        transform = GetComponent<Transform>();
        playerRb = GetComponent<Rigidbody2D>();
        cameraTransform = camera.GetComponent<Transform>();
        cameraZoneCollider = cameraZone.GetComponent<CircleCollider2D>();
        cameraCursorTransform = cameraCursor.GetComponent<Transform>();
        groundSpawnerListTransform = groundSpawnerList.GetComponent<Transform>();
        attackCollider = attackZone.GetComponent<BoxCollider2D>();

        StartCoroutine(LostSpeedUpdateSecondTimer());
    }

    void Update()
    {
        Moving();
        WatchOnCursor();
        CameraFollow();
        GroundSpawnerFollow();
        PlayerAttack();
        PlayerHeal();
        PlayerHematohyHeal();
        Death();
        MenuText();
    }

    void MenuText()
    {
        if (playerMinuteTimer < 10)
        {
            minutes = $"0{playerMinuteTimer}";
        }
        else minutes = $"{playerMinuteTimer}";

        if (playerSecondTimer < 10)
        {
            seconds = $"0{playerSecondTimer}";
        }
        else seconds = $"{playerSecondTimer}";

        if (score <= 0f)
        {
            bestSurviveTimeText.text = "00:00";
        }
        else bestSurviveTimeText.text = scoreText;

        surviveTimeText.text = $"{minutes}:{seconds}";
        playerHpText.text = $"{playerHp} HP";
        playerHematohyText.text = $"{Math.Floor(playerHematomahyAmount)} HM";
    }

    void Moving()
    {
        movDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movDirection.y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movDirection.y -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movDirection.x -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movDirection.x += 1;
        }

        movDirection = movDirection.normalized;

        playerRb.MovePosition(playerRb.position + movDirection * movingSpeed * Time.deltaTime);
    }

    IEnumerator LostSpeedUpdateSecondTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            playerHematomahyAmount -= playerHematomahyLostSpeed;
            playerSecondTimer += 1;

            if (playerSecondTimer >= 60)
            {
                playerHematomahyLostSpeed += 0.1f;
                playerMinuteTimer += 1f;
                playerSecondTimer = 0f;
            }
        }
    }

    void WatchOnCursor()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0f;

        Vector2 center = cameraZoneCollider.bounds.center;
        float radius = cameraZoneCollider.radius * cameraZoneCollider.transform.lossyScale.x;

        Vector2 direction = (Vector2)mousePos - center;

        if (direction.magnitude > radius)
        {
            direction = direction.normalized * radius;
        }

        cameraCursorTransform.position = center + direction;
        rotDirection = mousePos - transform.position;

        angle = Mathf.Atan2(rotDirection.y, rotDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void CameraFollow()
    {
        cameraCursorPos = cameraCursorTransform.position;
        cameraCursorPos.z = -10f;

        cameraTransform.position = cameraCursorPos;
    }

    void GroundSpawnerFollow()
    {
        playerPos = transform.position;
        playerPos.x += -5;
        playerPos.y += 1;
        playerPos.z = -10f;

        groundSpawnerListTransform.position = playerPos;
    }

    void PlayerAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.OverlapBox(
                attackCollider.bounds.center,
                attackCollider.size,
                0f,
                enemyLayerMask
            );

            if (hit != null && Time.time >= playerNextAttackTime)
            {
                Enemy enemy_ = hit.GetComponent<Enemy>();

                if (enemy_ != null)
                {
                    enemy_.TakeDamage(playerDamage);
                    enemy_.isPushed = true;

                    playerNextAttackTime = Time.time + playerDamageCooldown;
                }
            }
        }
    }

    void PlayerHeal()
    {
        healerHit = Physics2D.OverlapCircle(
            transform.position,
            0.5f,
            healBoxLayerMask
        );

        if (healerHit != null)
        {
            HealBox healBox_ = healerHit.GetComponent<HealBox>();

            if (healBox_ != null)
            {
                if ((playerHp + healBox_.healAmount) >= 100)
                {
                    playerHp = 100;
                }
                else playerHp += healBox_.healAmount;

                healBox_.Disappear();
            }
        }
    }

    void PlayerHematohyHeal()
    {
        hematohyHit = Physics2D.OverlapCircle(
            transform.position,
            0.5f,
            hematohyBoxLayerMask
        );

        if (hematohyHit != null)
        {
            HealBox hematohyBox_ = hematohyHit.GetComponent<HealBox>();

            if (hematohyBox_ != null)
            {
                if ((playerHematomahyAmount + hematohyBox_.healAmount) >= 100)
                {
                    playerHematomahyAmount = 100;
                }
                else playerHematomahyAmount += hematohyBox_.healAmount;

                hematohyBox_.Disappear();
            }
        }
    }

    public void TakeDamage(float enemyDamage)
    {
        playerHp -= enemyDamage;
    }

    void Death()
    {
        if (playerHp <= 0 || playerHematomahyAmount <= 0)
        {
            if (score < ((60 * playerMinuteTimer) + playerSecondTimer))
            {
                score = (60 * playerMinuteTimer) + playerSecondTimer;
                scoreText = surviveTimeText.text;
            }

            SceneManager.LoadScene(0);
        }
    }
}