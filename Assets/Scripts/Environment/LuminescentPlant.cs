using UnityEngine;
using System.Collections;

namespace ChronoCore.Environment
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LuminescentPlant : MonoBehaviour
    {
        [Header("Glow Settings")]
        [SerializeField] private Color normalColor = new Color(0, 0.8f, 0.4f, 1f);
        [SerializeField] private Color hoverColor = new Color(0.2f, 1f, 0.6f, 1f);
        [SerializeField] private float reactDuration = 0.3f;
        [SerializeField] private float scaleMultiplier = 1.15f;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _originalScale;
        private Coroutine _reactCoroutine;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalScale = transform.localScale;
            _spriteRenderer.color = normalColor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (_reactCoroutine != null) StopCoroutine(_reactCoroutine);
                _reactCoroutine = StartCoroutine(DoReact());
            }
        }

        private IEnumerator DoReact()
        {
            // Rapidly swell and brighten
            float elapsed = 0;
            while (elapsed < reactDuration * 0.5f)
            {
                float t = elapsed / (reactDuration * 0.5f);
                transform.localScale = Vector3.Lerp(_originalScale, _originalScale * scaleMultiplier, t);
                _spriteRenderer.color = Color.Lerp(normalColor, hoverColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Return to normal
            elapsed = 0;
            while (elapsed < reactDuration * 0.5f)
            {
                float t = elapsed / (reactDuration * 0.5f);
                transform.localScale = Vector3.Lerp(_originalScale * scaleMultiplier, _originalScale, t);
                _spriteRenderer.color = Color.Lerp(hoverColor, normalColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _reactCoroutine = null;
        }
    }
}
