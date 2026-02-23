using UnityEngine;
using System.Collections.Generic;

public class IrreversibleEventManager : MonoBehaviour
{
    public static IrreversibleEventManager Instance { get; private set; }

    private HashSet<string> triggeredEvents = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEvent(string eventId)
    {
        if (!triggeredEvents.Contains(eventId))
        {
            triggeredEvents.Add(eventId);
            Debug.Log($"[Irreversible] Event triggered: {eventId}");
        }
    }

    public bool IsEventTriggered(string eventId) => triggeredEvents.Contains(eventId);

    // This data persists through Rewind as it's not part of the snapshots
    public void ResetAllEvents()
    {
        triggeredEvents.Clear();
    }
}
