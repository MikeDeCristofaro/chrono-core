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

<<<<<<< HEAD
    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = currentHealth,
            Velocity = rb.linearVelocity,
            IsActive = true
        };
=======
    public void CaptureState(ref RewindSnapshot snapshot)
    {
        snapshot.position = transform.position;
        snapshot.rotation = transform.rotation;
        snapshot.health = currentHealth;
        snapshot.velocity = rb.linearVelocity;
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
<<<<<<< HEAD
        transform.position = snapshot.Position;
        currentHealth = snapshot.Health;
        rb.linearVelocity = snapshot.Velocity;
=======
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;
        currentHealth = snapshot.health;
        rb.linearVelocity = snapshot.velocity;
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20
    }

    public string GetRewindableId() => "PlayerInstance";
}
