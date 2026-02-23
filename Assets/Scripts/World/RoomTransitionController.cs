using UnityEngine;

public class RoomTransitionController : MonoBehaviour
{
    public static RoomTransitionController Instance { get; private set; }

    [SerializeField] private Vector3 currentRoomEntryPoint;
    [SerializeField] private bool hasCheckpointSet;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetRoomEntryPoint(Vector3 position)
    {
        currentRoomEntryPoint = position;
        hasCheckpointSet = true;
        
        // Notify RewindManager to lock the buffer tail to this position
        if (RewindManager.Instance != null)
        {
            RewindManager.Instance.SetHardStopPosition(position);
        }
    }

    public Vector3 GetRoomEntryPoint() => currentRoomEntryPoint;
    public bool HasCheckpoint() => hasCheckpointSet;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Simple trigger for demo purposes
            SetRoomEntryPoint(other.transform.position);
        }
    }
}
