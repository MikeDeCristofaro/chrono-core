using UnityEngine;
using System.Collections;
using ChronoCore.Rewind;

public class ThresherBoss : MonoBehaviour, IRewindable
{
    [Header("Health & Phases")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int phase2Threshold = 25;
    private int _currentHealth;
    private bool _isDead;
    private int _currentPhase = 1;

    [Header("Phase 1: Tail Sweep")]
    [SerializeField] private float sweepCooldown = 4f;
    [SerializeField] private float sweepRadius = 6f;
    private float _nextSweepTime;

    [Header("Phase 2: Spore Barrage")]
    [SerializeField] private float barrageCooldown = 6f;
    [SerializeField] private int sporeCount = 12;
    private float _nextBarrageTime;

    private void Start()
    {
        _currentHealth = maxHealth;
        
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.RegisterRewindable(this);
        }

        // Check if boss was already killed in an irreversible way
        if (IrreversibleEventManager.Instance != null && IrreversibleEventManager.Instance.IsEventTriggered("ThresherBossDead"))
        {
            _isDead = true;
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isDead || (RewindManager.Instance != null && RewindManager.Instance.IsRewinding)) return;

        UpdateBossAI();
    }

    private void UpdateBossAI()
    {
        if (_currentPhase == 1)
        {
            if (Time.time >= _nextSweepTime)
            {
                PerformTailSweep();
            }

            if (_currentHealth <= phase2Threshold)
            {
                TransitionToPhase2();
            }
        }
        else if (_currentPhase == 2)
        {
            if (Time.time >= _nextBarrageTime)
            {
                PerformSporeBarrage();
            }
        }
    }

    private void PerformTailSweep()
    {
        Debug.Log("[ThresherBoss] Performing TAIL SWEEP!");
        // Sweep logic (collision check in arc)
        _nextSweepTime = Time.time + sweepCooldown;
        if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.3f, 0.2f);
    }

    private void TransitionToPhase2()
    {
        _currentPhase = 2;
        Debug.Log("[ThresherBoss] PHASING... Spore mode active!");
        if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.5f, 0.3f);
    }

    private void PerformSporeBarrage()
    {
        Debug.Log("[ThresherBoss] Performing SPORE BARRAGE!");
        // Barrage logic (spawning projectiles)
        if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.4f, 0.25f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Boss_Barrage");
        _nextBarrageTime = Time.time + barrageCooldown;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        if (JuiceManager.Instance != null)
        {
            JuiceManager.Instance.HitFlash(GetComponent<SpriteRenderer>());
            JuiceManager.Instance.ShakeCamera(0.1f, 0.05f);
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Boss_Hurt");

        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log("[ThresherBoss] DEFEATED.");
        
        // Mark boss death as irreversible
        if (IrreversibleEventManager.Instance != null)
        {
            IrreversibleEventManager.Instance.TriggerEvent("ThresherBossDead");
        }

        gameObject.SetActive(false);
    }

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position = transform.position,
            Health = _currentHealth,
            IsActive = !_isDead,
            CustomIntA = _currentPhase,
            CustomFloatA = _nextSweepTime // We could track more if needed
        };
    }

    public void RestoreState(RewindSnapshot snapshot)
    {
        // If boss is dead irreversibly, don't restore position/health
        if (IrreversibleEventManager.Instance != null && IrreversibleEventManager.Instance.IsEventTriggered("ThresherBossDead"))
        {
            _isDead = true;
            gameObject.SetActive(false);
            return;
        }

        transform.position = snapshot.Position;
        _currentHealth = snapshot.Health;
        _currentPhase = snapshot.CustomIntA;
        _isDead = !snapshot.IsActive;
        _nextSweepTime = snapshot.CustomFloatA;

        gameObject.SetActive(!_isDead);
    }

    public string GetRewindableId() => "ThresherBoss_Final";
}
