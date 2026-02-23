using UnityEngine;

namespace ChronoCore.AI
{
    public class SeekerMissile : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 7f;
        [SerializeField] private float rotateSpeed = 200f;
        [SerializeField] private float lifespan = 5f;
        [SerializeField] private int damage = 1;

        private Transform _target;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, lifespan);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void FixedUpdate()
        {
            if (RewindManager.Instance != null && RewindManager.Instance.IsRewinding)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_target == null)
            {
                _rb.linearVelocity = transform.right * speed;
                return;
            }

            // Homing logic
            Vector2 direction = (Vector2)_target.position - _rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.right).z;
            _rb.angularVelocity = -rotateAmount * rotateSpeed;
            _rb.linearVelocity = transform.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                if (pc != null) pc.TakeDamage(damage);
                Explode();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Default")) // Wall/Floor
            {
                Explode();
            }
        }

        private void Explode()
        {
            // Add particle effect here later
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Missile_Impact");
            Destroy(gameObject);
        }
    }
}
