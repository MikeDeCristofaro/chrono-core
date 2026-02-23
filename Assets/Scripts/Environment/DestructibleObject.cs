using UnityEngine;
using ChronoCore.Rewind;

namespace ChronoCore.Environment
{
    public class DestructibleObject : MonoBehaviour, IRewindable
    {
        [SerializeField] private int health = 1;
        [SerializeField] private bool isIrreversible = false;
        [SerializeField] private GameObject debrisPrefab;

        private int _currentHealth;
        private bool _isDestroyed;

        private void Start()
        {
            _currentHealth = health;
            if (RewindManager.Instance != null)
                RewindManager.Instance.RegisterRewindable(this);
        }

        public void TakeDamage(int amount)
        {
            if (_isDestroyed) return;

            _currentHealth -= amount;
            if (_currentHealth <= 0) DestroyObject();
        }

        private void DestroyObject()
        {
            _isDestroyed = true;
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Object_Destruction");
            if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.2f, 0.1f);
            
            // Deactivate visual components
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (isIrreversible && IrreversibleEventManager.Instance != null)
            {
                IrreversibleEventManager.Instance.TriggerEvent($"Destroy_{gameObject.name}_{transform.position}");
            }
        }

        public RewindSnapshot CaptureState()
        {
            return new RewindSnapshot
            {
                Position = transform.position,
                Health = _currentHealth,
                IsActive = !_isDestroyed
            };
        }

        public void RestoreState(RewindSnapshot snapshot)
        {
            if (isIrreversible && _isDestroyed) return;

            _currentHealth = snapshot.Health;
            _isDestroyed = !snapshot.IsActive;

            GetComponent<Renderer>().enabled = snapshot.IsActive;
            GetComponent<Collider2D>().enabled = snapshot.IsActive;
        }

        public string GetRewindableId() => $"Destructible_{gameObject.name}_{transform.position}";
    }
}
