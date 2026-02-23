using UnityEngine;

public class GatedDoor : MonoBehaviour
{
    [SerializeField] private string eventId = "gate_01_unlocked";
    [SerializeField] private GameObject doorVisual;
    [SerializeField] private Collider2D doorCollider;

    private void Start()
    {
        // Check if this event was already triggered in a previous "life" (rewind persistence)
        if (IrreversibleEventManager.Instance != null && IrreversibleEventManager.Instance.IsEventTriggered(eventId))
        {
            OpenDoor(true);
        }
    }

    public void Unlock()
    {
        if (IrreversibleEventManager.Instance != null)
        {
            IrreversibleEventManager.Instance.RegisterEvent(eventId);
        }
        OpenDoor(false);
    }

    private void OpenDoor(bool immediate)
    {
        if (doorVisual != null) doorVisual.SetActive(false);
        if (doorCollider != null) doorCollider.enabled = false;
        
        if (!immediate)
        {
            Debug.Log($"[GatedDoor] {eventId} unlocked! This will persist through rewinds.");
        }
    }
}
