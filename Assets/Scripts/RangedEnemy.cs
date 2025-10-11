using UnityEngine;
using System.Collections;

public class EnemyMortarShooter : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public string playerTag = "Player";

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float launchAngle = 45f; // degrees
    public float gravity = 9.81f;
    public float shootInterval = 1.5f; 

    [Header("Animation")]
    public Animator animator;
    public string throwAnimParam = "Throw";

    private Transform player;
    private bool isShooting = false;
    private bool isFacingRight = true;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("⚠️ No GameObject with tag '" + playerTag + "' found!");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Always face player if in range
        if (distance <= detectionRadius)
            FacePlayer();

        // Start/stop shooting based on range
        if (distance <= detectionRadius && !isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
        else if (distance > detectionRadius && isShooting)
        {
            StopAllCoroutines();
            isShooting = false;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        
        if (player.position.x < transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && isFacingRight)
        {
            Flip();
        }
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator ShootRoutine()
    {
        isShooting = true;

        while (true)
        {
            if (animator != null && !string.IsNullOrEmpty(throwAnimParam))
                animator.SetTrigger(throwAnimParam);

            yield return new WaitForSeconds(shootInterval);
        }
    }

    
    public void LaunchMortar()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        Vector2 targetPos = player.position;
        Vector2 startPos = firePoint.position;
        Vector2 direction = targetPos - startPos;
        float distance = direction.magnitude;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        float v2 = (gravity * distance * distance) /
                   (2f * (distance * Mathf.Tan(angleRad) - (targetPos.y - startPos.y)) * Mathf.Pow(Mathf.Cos(angleRad), 2));

        if (v2 <= 0) return;

        float velocity = Mathf.Sqrt(v2);
        float vx = velocity * Mathf.Cos(angleRad) * Mathf.Sign(direction.x);
        float vy = velocity * Mathf.Sin(angleRad);

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(vx, vy);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
