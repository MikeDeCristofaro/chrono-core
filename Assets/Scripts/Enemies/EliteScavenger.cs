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
    private float _speedMultiplier = 1.0f;

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
        // Adjust cooldowns/timers by speed multiplier if appropriate
        if (Time.time >= _nextSlamTime && !_isTelegraphing)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && Vector2.Distance(transform.position, player.transform.position) < slamRadius)
            {
                StartCoroutine(PerformGroundSlam());
            }
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    private System.Collections.IEnumerator PerformGroundSlam()
    {
        _isTelegraphing = true;
        // Slow down telegraph if speed multiplier is low
        yield return new WaitForSeconds(slamTelegraphTime / _speedMultiplier);

        if (!_isDead && !(RewindManager.Instance != null && RewindManager.Instance.IsRewinding))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<PlayerController>()?.TakeDamage(2);
                }
            }
            _nextSlamTime = Time.time + (groundSlamCooldown / _speedMultiplier);
        }
        _isTelegraphing = false;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;
        _currentHealth -= amount;
        if (JuiceManager.Instance != null) JuiceManager.Instance.HitFlash(GetComponent<SpriteRenderer>());
        if (_currentHealth <= 0) Die();
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
                return;
            }
        }

        TakeDamage(amount);
    }

    private void Die()
    {
        _isDead = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Enemy_Explode");
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
}
