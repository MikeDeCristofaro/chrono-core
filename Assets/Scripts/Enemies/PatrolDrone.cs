using UnityEngine;
<<<<<<< HEAD
using ChronoCore.Rewind;
=======
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20

public class PatrolDrone : MonoBehaviour, IRewindable
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private Vector2 startPos;
    private int direction = 1;
    private bool isDead;

    private void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
        
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.RegisterRewindable(this);
        }
    }

    private void Update()
    {
<<<<<<< HEAD
        if (isDead || (RewindManager.Instance != null && RewindManager.Instance.IsRewinding)) return;
=======
        if (isDead) return;
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            direction *= -1;
            // Snap to boundary to avoid drift
            float targetX = startPos.x + (patrolDistance * direction * -1);
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        gameObject.SetActive(false);
<<<<<<< HEAD
    }

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = currentHealth,
            IsActive = !isDead
        };
=======
        // Irreversible events like boss deaths are handled by IrreversibleEventManager
        // but normal mobs just stay dead per their snapshot state
    }

    public void CaptureState(ref RewindSnapshot snapshot)
    {
        snapshot.position = transform.position;
        snapshot.rotation = transform.rotation;
        snapshot.health = currentHealth;
        snapshot.isDead = isDead;
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
<<<<<<< HEAD
        transform.position = snapshot.Position;
        currentHealth = snapshot.Health;
        isDead = !snapshot.IsActive;
=======
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;
        currentHealth = snapshot.health;
        isDead = snapshot.isDead;
>>>>>>> 1a55cd53b60e3dda2ad47fa9cf2d258426432c20
        gameObject.SetActive(!isDead);
    }

    public string GetRewindableId() => $"PatrolDrone_{gameObject.GetInstanceID()}";
}
