using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ChronoCore.Combat
{
    public class ChronoGrenade : MonoBehaviour
    {
        [Header("Explosion Settings")]
        [SerializeField] private float explosionDelay = 2f;
        [SerializeField] private float explosionRadius = 4f;
        [SerializeField] private int damage = 2;
        [SerializeField] private GameObject slowFieldPrefab;

        [Header("Time Slow Settings")]
        [SerializeField] private float slowDuration = 4f;
        [SerializeField] private float slowFactor = 0.3f;

        private bool _exploded = false;

        private void Start()
        {
            StartCoroutine(ExplosionTimer());
        }

        private IEnumerator ExplosionTimer()
        {
            yield return new WaitForSeconds(explosionDelay);
            Explode();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Optional: Explode on contact with enemies
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Explode();
            }
        }

        private void Explode()
        {
            if (_exploded) return;
            _exploded = true;

            // 1. Visual/Audio
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Grenade_Explosion");
            if (JuiceManager.Instance != null) JuiceManager.Instance.ShakeCamera(0.4f, 0.3f);

            // 2. Damage
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    // Basic damage logic
                    var drone = hit.GetComponent<ChronoCore.AI.FlyingDrone>();
                    if (drone != null) drone.TakeDamage(damage);
                    
                    var scavenger = hit.GetComponent<EliteScavenger>();
                    if (scavenger != null) scavenger.TakeDamage(damage);
                }
            }

            // 3. Create Time Slow Field
            if (slowFieldPrefab != null)
            {
                GameObject field = Instantiate(slowFieldPrefab, transform.position, Quaternion.identity);
                Destroy(field, slowDuration);
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
