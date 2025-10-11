using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 50f;
    public float deceleration = 50f;
    public float airAcceleration = 35f;
    public float airDeceleration = 35f;

    [Header("Jump Settings")]
    public float jumpForce = 15f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int maxJumps = 2;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Animation Settings")]
    public Animator animator;
    public string idleAnimParam = "Idle";
    public string walkAnimParam = "Walk";
    public string jumpAnimParam = "Jump";
    public string fallAnimParam = "Fall";

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;
    private int jumpsRemaining;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        CheckGroundStatus();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleJump();
        HandleFlip();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleBetterJump();
    }

    #region Movement and Jump Logic

    void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
            jumpsRemaining = maxJumps;
    }

    void HandleCoyoteTime()
    {
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;
    }

    void HandleJumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f && jumpsRemaining > 0)
            {
                Jump();
            }
            else if (!isGrounded && jumpsRemaining > 0 && jumpsRemaining < maxJumps)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsRemaining--;
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    void HandleBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void HandleMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        float accel = isGrounded ? acceleration : airAcceleration;
        float decel = isGrounded ? deceleration : airDeceleration;

        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel : decel;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right);
    }

    void HandleFlip()
    {
        if (horizontalInput > 0 && !isFacingRight)
            Flip();
        else if (horizontalInput < 0 && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    #endregion

    #region Animation Logic

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Reset all to false first (simple way to control state manually)
        animator.SetBool(idleAnimParam, false);
        animator.SetBool(walkAnimParam, false);
        animator.SetBool(jumpAnimParam, false);

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                // Going up
                animator.SetBool(jumpAnimParam, true);

            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                // Going down
                animator.SetBool(fallAnimParam, true);
                animator.SetBool(jumpAnimParam, false);
            }
            else
            {
                // Mid-air but near the peak (neither rising nor falling strongly)
                animator.SetBool(jumpAnimParam, true);
            }
        }

        
        else
        {
            animator.SetBool(fallAnimParam, false);
            animator.SetBool(jumpAnimParam, false);
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                animator.SetBool(walkAnimParam, true);
                animator.SetBool(idleAnimParam, false);
            }
            else
            {
                animator.SetBool(jumpAnimParam, false);
                animator.SetBool(idleAnimParam, true);
                animator.SetBool(walkAnimParam, false);
                animator.SetBool(fallAnimParam, false);
            }
        }
        
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
