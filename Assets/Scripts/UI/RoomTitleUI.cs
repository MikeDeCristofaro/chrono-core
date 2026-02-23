using UnityEngine;
using TMPro;
using System.Collections;

namespace ChronoCore.UI
{
    public class RoomTitleUI : MonoBehaviour
    {
        public static RoomTitleUI Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private float displayDuration = 2.0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (canvasGroup != null) canvasGroup.alpha = 0;
        }

        public void ShowTitle(string roomName)
        {
            if (titleText == null || canvasGroup == null) return;
            StopAllCoroutines();
            StartCoroutine(DoShowTitle(roomName));
        }

        private IEnumerator DoShowTitle(string roomName)
        {
            titleText.text = roomName;
            
            // Fade In
            float elapsed = 0;
            while (elapsed < fadeDuration)
            {
                canvasGroup.alpha = elapsed / fadeDuration;
                elapsed += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1;

            yield return new WaitForSeconds(displayDuration);

            // Fade Out
            elapsed = 0;
            while (elapsed < fadeDuration)
            {
                canvasGroup.alpha = 1 - (elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0;
        }
    }
}
