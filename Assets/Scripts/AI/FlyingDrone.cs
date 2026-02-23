using UnityEngine;
using ChronoCore.Rewind;

namespace ChronoCore.AI
{
    public class FlyingDrone : MonoBehaviour, IRewindable
    {
        [Header("Flight Settings")]
        [SerializeField] private float flySpeed = 5f;
        [SerializeField] private float sineAmplitude = 1.5f;
        [SerializeField] private float sineFrequency = 2f;
        [SerializeField] private float searchRadius = 15f;
        [SerializeField] private float hoverAltitude = 6f;

        [Header("Combat Settings")]
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private int maxHealth = 3;

        private Transform _target;
        private float _fireTimer;
        private int _currentHealth;
        private Vector3 _startPos;
        private float _sineTimer;
        private float _speedMultiplier = 1.0f;

        private void Start()
        {
            _currentHealth = maxHealth;
            _startPos = transform.position;
            _target = GameObject.FindWithTag("Player")?.transform;
            
            if (RewindManager.Instance != null)
                RewindManager.Instance.RegisterRewindable(this);
        }

        private void Update()
        {
            if (RewindManager.Instance != null && RewindManager.Instance.IsRewinding) return;

            HandleMovement();
            HandleCombat();
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        private void HandleMovement()
        {
            if (_target == null) return;

            // Move towards player but stay above them
            Vector3 targetPos = _target.position + Vector3.up * hoverAltitude;
            
            // Apply sinusoidal hover
            _sineTimer += Time.deltaTime * sineFrequency * _speedMultiplier;
            targetPos.y += Mathf.Sin(_sineTimer) * sineAmplitude;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * _speedMultiplier * Time.deltaTime);

            // Look at player
            if (_target.position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }

        private void HandleCombat()
        {
            if (_target == null) return;

            _fireTimer += Time.deltaTime * _speedMultiplier;
            if (_fireTimer >= fireRate)
            {
                float dist = Vector2.Distance(transform.position, _target.position);
                if (dist <= searchRadius)
                {
                    FireMissile();
                    _fireTimer = 0;
                }
            }
        }

        private void FireMissile()
        {
            if (missilePrefab == null || firePoint == null) return;
            
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            SeekerMissile sm = missile.GetComponent<SeekerMissile>();
            if (sm != null) sm.SetTarget(_target);

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Drone_Fire");
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            if (JuiceManager.Instance != null) JuiceManager.Instance.HitFlash(GetComponent<SpriteRenderer>());
            
            if (_currentHealth <= 0) Die();
        }

        private void Die()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Drone_Explode");
            if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.3f, 0.2f);
            gameObject.SetActive(false);
        }

        // IRewindable
        public RewindSnapshot CaptureState()
        {
            return new RewindSnapshot
            {
                Position = transform.position,
                Health = _currentHealth,
                IsActive = gameObject.activeSelf,
                CustomFloatA = _sineTimer
            };
        }

        public void RestoreState(RewindSnapshot snapshot)
        {
            transform.position = snapshot.Position;
            _currentHealth = snapshot.Health;
            _sineTimer = snapshot.CustomFloatA;
            gameObject.SetActive(snapshot.IsActive);
        }

        public string GetRewindableId() => $"Drone_{gameObject.GetInstanceID()}";
    }
}
