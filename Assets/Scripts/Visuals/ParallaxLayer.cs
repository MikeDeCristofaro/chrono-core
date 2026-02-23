using UnityEngine;

namespace ChronoCore.Visuals
{
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Tooltip("0 = moves with camera, 1 = static background. Near layers can use negative values.")]
        [SerializeField] private float parallaxFactor = 0.5f;

        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;

        private void Start()
        {
            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
                _lastCameraPosition = _cameraTransform.position;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            
            // X-axis parallax (common for 2D)
            // Y-axis parallax can be added if verticality is high
            transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * (parallaxFactor * 0.5f), 0);
            
            _lastCameraPosition = _cameraTransform.position;
        }
    }
}
