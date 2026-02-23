using UnityEngine;
using ChronoCore.Rewind;

public class PlayerController : MonoBehaviour, IRewindable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
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

        // Input
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Rewind Toggle (Space or R)
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

        // Movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void CaptureState(ref RewindSnapshot snapshot)
    {
        snapshot.position = transform.position;
        snapshot.rotation = transform.rotation;
        snapshot.health = currentHealth;
        snapshot.velocity = rb.linearVelocity;
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;
        currentHealth = snapshot.health;
        rb.linearVelocity = snapshot.velocity;
    }

    public string GetRewindableId() => "PlayerInstance";
}
