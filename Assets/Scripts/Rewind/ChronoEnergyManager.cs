using UnityEngine;

public class ChronoEnergyManager : MonoBehaviour
{
    public static ChronoEnergyManager Instance { get; private set; }

    [SerializeField] private int maxSegments = 3;
    [SerializeField] private float secondsPerSegment = 3f;
    [SerializeField] private float regenDelay = 8f;
    [SerializeField] private float regenRate = 0.25f; // 4 seconds for full segment

    private float currentEnergy;
    private float lastRewindTime;
    private bool isRewinding;

    public float CurrentEnergyNormalized => currentEnergy / (maxSegments * secondsPerSegment);
    public int FullSegments => Mathf.FloorToInt(currentEnergy / secondsPerSegment);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentEnergy = maxSegments * secondsPerSegment;
    }

    private void Update()
    {
        if (isRewinding)
        {
            ConsumeEnergy(Time.deltaTime);
            lastRewindTime = Time.time;
        }
        else if (Time.time - lastRewindTime > regenDelay)
        {
            RegenerateEnergy(Time.deltaTime * regenRate * secondsPerSegment);
        }
    }

    public void SetRewinding(bool rewinding)
    {
        isRewinding = rewinding;
    }

    public bool HasEnergy() => currentEnergy > 0;

    private void ConsumeEnergy(float amount)
    {
        float previousEnergy = currentEnergy;
        currentEnergy = Mathf.Max(0, currentEnergy - amount);
        
        if (previousEnergy > 0 && currentEnergy <= 0)
        {
            if (JuiceManager.Instance != null)
            {
                JuiceManager.Instance.TimeScaleDip(0.1f, 0.4f);
            }
        }
    }

    private void RegenerateEnergy(float amount)
    {
        currentEnergy = Mathf.Min(maxSegments * secondsPerSegment, currentEnergy + amount);
    }
}
