using UnityEngine;
using UnityEngine.UI;
using ChronoCore.Rewind;

public class ChronoEnergyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image energyFillBar;
    [SerializeField] private Image[] energySegments; // Optional: for discrete segment display
    [SerializeField] private Animator panelAnimator;

    [Header("Warning Settings")]
    [SerializeField] private float lowEnergyThreshold = 0.2f;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color warningColor = Color.red;

    private void Update()
    {
        if (ChronoEnergyManager.Instance == null) return;

        float currentEnergy = ChronoEnergyManager.Instance.CurrentEnergyNormalized;
        
        // Update bar fill
        if (energyFillBar != null)
        {
            energyFillBar.fillAmount = currentEnergy;
            
            // Pulse or change color when low
            if (currentEnergy < lowEnergyThreshold)
            {
                energyFillBar.color = Color.Lerp(warningColor, Color.white, Mathf.PingPong(Time.time * 5f, 1f));
                if (panelAnimator != null) panelAnimator.SetBool("IsLowEnergy", true);
            }
            else
            {
                energyFillBar.color = normalColor;
                if (panelAnimator != null) panelAnimator.SetBool("IsLowEnergy", false);
            }
        }
    }
}
