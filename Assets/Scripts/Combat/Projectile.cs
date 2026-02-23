using UnityEngine;

public class Projectile : MonoBehaviour, IRewindable
{
    private Vector3 position;
    private Vector3 velocity;
    private bool isActive;

    public void OnSnapshot(RewindFrame frame)
    {
        frame.Position = transform.position;
        if (GetComponent<Rigidbody>() != null)
        {
            frame.Velocity = GetComponent<Rigidbody>().velocity;
        }
        frame.IsActive = gameObject.activeSelf;
    }

    public void OnRewind(RewindFrame frame)
    {
        transform.position = frame.Position;
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().velocity = frame.Velocity;
        }
        gameObject.SetActive(frame.IsActive);
    }
}
