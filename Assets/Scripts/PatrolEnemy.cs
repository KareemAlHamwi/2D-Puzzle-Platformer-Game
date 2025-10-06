using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform target;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        // Move towards the current target
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // If close to the target, switch direction
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
            Flip();
        }
    }

    void Flip()
    {
        // Flip the enemy's local scale (for 2D sprite direction)
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        // Draw patrol path in the editor for visualization
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
