using UnityEngine;

public class RoomLinkTrigger : MonoBehaviour
{
    [Header("Room Management")]
    [SerializeField] private string roomName = "New Area";
    [SerializeField] private GameObject[] roomsToEnable;
    [SerializeField] private GameObject[] roomsToDisable;
    
    [Header("Transition Settings")]
    [SerializeField] private Vector3 playerSpawnTarget;
    [SerializeField] private bool setAsCheckpoint = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PerformTransition();
        }
    }

    private void PerformTransition()
    {
        // 1. Manage GameObject visibility
        foreach (var room in roomsToEnable)
        {
            if (room != null) room.SetActive(true);
        }

        foreach (var room in roomsToDisable)
        {
            if (room != null) room.SetActive(false);
        }

        // 2. Set new checkpoint/hard-stop position
        if (setAsCheckpoint && RoomTransitionController.Instance != null)
        {
            RoomTransitionController.Instance.SetRoomEntryPoint(playerSpawnTarget);
        }

        Debug.Log($"[RoomLink] Transitioned to new zone: {roomName}. Spawn Target: {playerSpawnTarget}");

        // Juice
        if (JuiceManager.Instance != null)
        {
            JuiceManager.Instance.HitFlash(null, 0.05f); // Screen flash fallback
            JuiceManager.Instance.ShowRoomTitle(roomName);
        }
    }
}
