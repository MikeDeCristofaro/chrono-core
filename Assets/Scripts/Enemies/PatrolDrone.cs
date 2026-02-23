using UnityEngine;

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
        if (isDead) return;

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
        // Irreversible events like boss deaths are handled by IrreversibleEventManager
        // but normal mobs just stay dead per their snapshot state
    }

    public void CaptureState(ref RewindSnapshot snapshot)
    {
        snapshot.position = transform.position;
        snapshot.rotation = transform.rotation;
        snapshot.health = currentHealth;
        snapshot.isDead = isDead;
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        transform.position = snapshot.position;
        transform.rotation = snapshot.rotation;
        currentHealth = snapshot.health;
        isDead = snapshot.isDead;
        gameObject.SetActive(!isDead);
    }

    public string GetRewindableId() => $"PatrolDrone_{gameObject.GetInstanceID()}";
}
