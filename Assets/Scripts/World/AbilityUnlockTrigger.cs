using UnityEngine;

public class AbilityUnlockTrigger : MonoBehaviour
{
    [SerializeField] private string abilityId = "WallJump";
    [SerializeField] private GameObject pickupVisual;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.UnlockAbility(abilityId);
        }

        Debug.Log($"[AbilityUnlock] Unlocked: {abilityId}!");
        
        // Visuals/JUICE
        if (JuiceManager.Instance != null)
        {
            JuiceManager.Instance.ShakeCamera(0.4f, 0.2f);
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Ability_Unlock");
        }

        // Deactivate to prevent multiple triggers
        gameObject.SetActive(false);
    }
}
