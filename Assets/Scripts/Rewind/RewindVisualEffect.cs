using UnityEngine;
using ChronoCore.Rewind;

public class RewindVisualEffect : MonoBehaviour
{
    private void OnGUI()
    {
        if (RewindManager.Instance == null || !RewindManager.Instance.IsRewinding) return;

        // Draw a full-screen semi-transparent blue tint to indicate rewind
        GUI.color = new Color(0, 0.5f, 1f, 0.2f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        
        // Add some "scanline" text
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width / 2 - 50, 40, 200, 30), "<< REWINDING >>");
    }
}
