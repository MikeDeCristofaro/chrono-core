using UnityEngine;
using System.Collections;
using ChronoCore.Rewind;

public class PlayerController : MonoBehaviour, IRewindable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Jump")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForceX = 10f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    public bool canWallJump = false; 

    [Header("Ledge Climb")]
    [SerializeField] private float ledgeClimbDuration = 0.5f;
    [SerializeField] private Vector2 ledgeClimbOffset = new Vector2(0.3f, 1.2f);
    [SerializeField] private Transform ledgeCheck;
    public bool canLedgeClimb = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isHanging;
    private bool isClimbing;
    private float moveInput;
    private int currentHealth = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.RegisterRewindable(this);
        }
    }

    private void Update()
    {
        if (RewindManager.Instance != null && RewindManager.Instance.IsRewinding) return;

        if (isClimbing) return;

        if (isHanging)
        {
            if (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            {
                StartCoroutine(ClimbLedge());
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                isHanging = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            return;
        }

        // Input
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Jump");
            }
            else if (isWallSliding && canWallJump)
            {
                float jumpDir = (transform.localScale.x > 0) ? -1 : 1;
                rb.linearVelocity = new Vector2(jumpDir * wallJumpForceX, wallJumpForceY);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("WallJump");
                
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }

        // Rewind Toggle
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (ChronoEnergyManager.Instance != null && ChronoEnergyManager.Instance.HasEnergy())
            {
                RewindManager.Instance.StartRewind();
                ChronoEnergyManager.Instance.SetRewinding(true);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.R))
        {
            RewindManager.Instance.StopRewind();
            ChronoEnergyManager.Instance.SetRewinding(false);
        }
    }

    private void FixedUpdate()
    {
        if (RewindManager.Instance != null && RewindManager.Instance.IsRewinding) return;

        if (isClimbing) return;

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Wall & Ledge Check
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        bool isTouchingLedge = Physics2D.OverlapCircle(ledgeCheck.position, 0.2f, wallLayer);

        // Ledge Hang Logic
        if (canLedgeClimb && isTouchingWall && !isTouchingLedge && !isGrounded && rb.linearVelocity.y < 0)
        {
            if (!isHanging)
            {
                isHanging = true;
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Ledge_Grab");
            }
        }

        // Wall Slide Logic
        if (!isHanging && canWallJump && isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }

        // Movement
        if (!isWallSliding && !isHanging)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            
            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private IEnumerator ClimbLedge()
    {
        isClimbing = true;
        isHanging = false;
        
        Vector3 startPos = transform.position;
        Vector2 targetPos = new Vector2(startPos.x + (transform.localScale.x * ledgeClimbOffset.x), startPos.y + ledgeClimbOffset.y);
        
        float elapsed = 0;
        while (elapsed < ledgeClimbDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / ledgeClimbDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
        rb.bodyType = RigidbodyType2D.Dynamic;
        isClimbing = false;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Ledge_Climb");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (JuiceManager.Instance != null)
        {
            JuiceManager.Instance.ShakeCamera(0.2f, 0.15f);
            JuiceManager.Instance.HitFlash(GetComponent<SpriteRenderer>());
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Player_Hurt");
    }

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = currentHealth,
            Velocity = rb.linearVelocity,
            IsActive = true,
            CustomBoolA = isHanging,
            CustomBoolB = isClimbing
        };
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        transform.position = snapshot.Position;
        currentHealth = snapshot.Health;
        rb.linearVelocity = snapshot.Velocity;
        isHanging = snapshot.CustomBoolA;
        isClimbing = snapshot.CustomBoolB;

        if (isHanging || isClimbing) rb.bodyType = RigidbodyType2D.Kinematic;
        else rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public string GetRewindableId() => "PlayerInstance";
}
