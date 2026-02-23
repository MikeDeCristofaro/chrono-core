using UnityEngine;
using System.Collections;

namespace ChronoCore.Environment
{
    public class LaserGridTrap : MonoBehaviour
    {
        [SerializeField] private float onDuration = 2f;
        [SerializeField] private float offDuration = 2f;
        [SerializeField] private float startDelay = 0f;
        [SerializeField] private int damage = 1;
        [SerializeField] private LineRenderer laserLine;
        [SerializeField] private BoxCollider2D laserCollider;

        private bool _isActive;

        private void Start()
        {
            StartCoroutine(TrapCycle());
        }

        private IEnumerator TrapCycle()
        {
            yield return new WaitForSeconds(startDelay);

            while (true)
            {
                SetTrapActive(true);
                yield return new WaitForSeconds(onDuration);
                
                SetTrapActive(false);
                yield return new WaitForSeconds(offDuration);
            }
        }

        private void SetTrapActive(bool active)
        {
            _isActive = active;
            if (laserLine != null) laserLine.enabled = active;
            if (laserCollider != null) laserCollider.enabled = active;
            
            if (active && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("Laser_On");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isActive && other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>()?.TakeDamage(damage);
            }
        }
    }
}
