using UnityEngine;
using System.Collections;

public class JuiceManager : MonoBehaviour
{
    public static JuiceManager Instance { get; private set; }

    [Header("Screenshake")]
    private Vector3 _originalCameraPos;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Triggers a screen shake effect.
    /// </summary>
    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.1f)
    {
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        _originalCameraPos = mainCam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.localPosition = new Vector3(_originalCameraPos.x + x, _originalCameraPos.y + y, _originalCameraPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = _originalCameraPos;
    }

    /// <summary>
    /// Makes a sprite flash white briefly.
    /// </summary>
    public void HitFlash(SpriteRenderer sr, float duration = 0.1f)
    {
        StartCoroutine(DoHitFlash(sr, duration));
    }

    private IEnumerator DoHitFlash(SpriteRenderer sr, float duration)
    {
        if (sr == null) yield break;
        
        Color originalColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(duration);
        if (sr != null) sr.color = originalColor;
    }

    /// <summary>
    /// Briefly slows down time for impact.
    /// </summary>
    public void TimeScaleDip(float scale = 0.2f, float duration = 0.5f)
    {
        StartCoroutine(DoTimeScaleDip(scale, duration));
    }

    private IEnumerator DoTimeScaleDip(float scale, float duration)
    {
        float originalScale = Time.timeScale;
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalScale;
    }

    public void ShowRoomTitle(string roomName)
    {
        if (ChronoCore.UI.RoomTitleUI.Instance != null)
        {
            ChronoCore.UI.RoomTitleUI.Instance.ShowTitle(roomName);
        }
    }
}
