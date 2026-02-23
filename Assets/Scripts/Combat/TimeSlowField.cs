using UnityEngine;

namespace ChronoCore.Combat
{
    public class TimeSlowField : MonoBehaviour
    {
        [SerializeField] private float slowFactor = 0.3f;
        
        private List<GameObject> _affectedEnemies = new List<GameObject>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                // In a real project, we'd have an IEnemy interface or a base AI class
                // For now, we'll check the specific types we have.
                ApplySlow(other.gameObject);
                _affectedEnemies.Add(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                RemoveSlow(other.gameObject);
                _affectedEnemies.Remove(other.gameObject);
            }
        }

        private void ApplySlow(GameObject enemy)
        {
            // We can use a simple message system or direct variable access
            // For now, let's assume enemies have a speed multiplier
            enemy.SendMessage("SetSpeedMultiplier", slowFactor, SendMessageOptions.DontRequireReceiver);
        }

        private void RemoveSlow(GameObject enemy)
        {
            enemy.SendMessage("SetSpeedMultiplier", 1.0f, SendMessageOptions.DontRequireReceiver);
        }

        private void OnDestroy()
        {
            foreach (var enemy in _affectedEnemies)
            {
                if (enemy != null) RemoveSlow(enemy);
            }
        }
    }
}
