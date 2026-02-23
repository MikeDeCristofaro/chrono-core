using UnityEngine;

public class ChronoEnergyHUD : MonoBehaviour
{
    private void OnGUI()
    {
        if (ChronoEnergyManager.Instance == null) return;

        float energy = ChronoEnergyManager.Instance.CurrentEnergyNormalized;
        int segments = ChronoEnergyManager.Instance.FullSegments;

        // Simple Debug HUD
        GUI.Box(new Rect(10, 10, 200, 60), "CHRONO ENERGY");
        GUI.HorizontalSlider(new Rect(20, 35, 180, 20), energy, 0f, 1f);
        GUI.Label(new Rect(20, 50, 180, 20), $"Segments: {segments} / 3");
    }
}
