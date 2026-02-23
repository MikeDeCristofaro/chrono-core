using UnityEngine;
using UnityEngine.Audio;
using ChronoCore.Rewind;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private string pitchParam = "BGM_Pitch";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.OnRewindStart += HandleRewindStart;
            RewindManager.Instance.OnRewindStop += HandleRewindStop;
        }
    }

    private void HandleRewindStart()
    {
        if (mainMixer != null)
        {
            // Pitch shift down to simulate time reversal or slowing
            mainMixer.SetFloat(pitchParam, 0.7f);
        }
        PlaySFXInternal("Rewind_Start");
    }

    private void HandleRewindStop()
    {
        if (mainMixer != null)
        {
            // Restore normal pitch
            mainMixer.SetFloat(pitchParam, 1.0f);
        }
        PlaySFXInternal("Rewind_Stop");
    }

    public void PlaySFX(string sfxName)
    {
        PlaySFXInternal(sfxName);
    }

    private void PlaySFXInternal(string sfxName)
    {
        // Placeholder for real audio clip lookup and playback
        Debug.Log($"[AudioManager] Playing SFX: {sfxName}");
    }
}
