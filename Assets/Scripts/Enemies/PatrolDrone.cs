using UnityEngine;
using ChronoCore.Rewind;

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
        if (isDead || (RewindManager.Instance != null && RewindManager.Instance.IsRewinding)) return;

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
    }

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = currentHealth,
            IsActive = !isDead
        };
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        transform.position = snapshot.Position;
        currentHealth = snapshot.Health;
        isDead = !snapshot.IsActive;
        gameObject.SetActive(!isDead);
    }

    public string GetRewindableId() => $"PatrolDrone_{gameObject.GetInstanceID()}";
}
