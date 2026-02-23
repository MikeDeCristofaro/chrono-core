using UnityEngine;
using ChronoCore.Rewind;

public class EliteScavenger : MonoBehaviour, IRewindable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    private int _currentHealth;
    private bool _isDead;

    [Header("Shield")]
    [SerializeField] private bool shieldActive = true;
    [SerializeField] private float shieldAngle = 90f; // Frontal coverage

    [Header("Abilities")]
    [SerializeField] private float groundSlamCooldown = 5f;
    [SerializeField] private float slamRadius = 4f;
    [SerializeField] private float slamTelegraphTime = 0.8f;
    private float _nextSlamTime;
    private bool _isTelegraphing;

    private Rigidbody2D _rb;

    private void Start()
    {
        _currentHealth = maxHealth;
        _rb = GetComponent<Rigidbody2D>();
        
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.RegisterRewindable(this);
        }
    }

    private void Update()
    {
        if (_isDead || (RewindManager.Instance != null && RewindManager.Instance.IsRewinding)) return;

        UpdateAI();
    }

    private void UpdateAI()
    {
        if (Time.time >= _nextSlamTime && !_isTelegraphing)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && Vector2.Distance(transform.position, player.transform.position) < slamRadius)
            {
                StartCoroutine(PerformGroundSlam());
            }
        }
    }

    private System.Collections.IEnumerator PerformGroundSlam()
    {
        _isTelegraphing = true;
        Debug.Log("[EliteScavenger] Telegraphing Ground Slam...");
        yield return new WaitForSeconds(slamTelegraphTime);

        if (!_isDead && !(RewindManager.Instance != null && RewindManager.Instance.IsRewinding))
        {
            Debug.Log("[EliteScavenger] GROUND SLAM!");
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    // Logic to damage player
                }
            }
            _nextSlamTime = Time.time + groundSlamCooldown;
        }
        _isTelegraphing = false;
    }

    public void TakeDamage(int amount, Vector2 attackPosition)
    {
        if (_isDead) return;

        if (shieldActive)
        {
            Vector2 dirToAttack = (attackPosition - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(transform.right, dirToAttack);
            if (angle < shieldAngle / 2f)
            {
                Debug.Log("[EliteScavenger] Attack Blocked by Shield!");
                return;
            }
        }

        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        _isDead = true;
        gameObject.SetActive(false);
    }

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = _currentHealth,
            IsActive = !_isDead,
            CustomBoolA = _isTelegraphing,
            CustomFloatA = _nextSlamTime
        };
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        transform.position = snapshot.Position;
        _currentHealth = snapshot.Health;
        _isDead = !snapshot.IsActive;
        _isTelegraphing = snapshot.CustomBoolA;
        _nextSlamTime = snapshot.CustomFloatA;
        
        gameObject.SetActive(!_isDead);
        
        if (!_isTelegraphing)
        {
            StopAllCoroutines();
        }
    }

    public string GetRewindableId() => $"EliteScavenger_{gameObject.GetInstanceID()}";

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRadius);
        
        if (shieldActive)
        {
            Gizmos.color = Color.blue;
            Vector3 right = transform.right;
            Vector3 leftLimit = Quaternion.AngleAxis(-shieldAngle / 2f, Vector3.forward) * right;
            Vector3 rightLimit = Quaternion.AngleAxis(shieldAngle / 2f, Vector3.forward) * right;
            Gizmos.DrawLine(transform.position, transform.position + leftLimit * 2f);
            Gizmos.DrawLine(transform.position, transform.position + rightLimit * 2f);
        }
    }
}
