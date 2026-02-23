using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Ability Flags")]
    public bool wallJumpUnlocked = false;
    public bool ledgeClimbUnlocked = false;

    [Header("Stat Boosts")]
    public float maxChronoEnergyMultiplier = 1.0f;
    public float rewindDurationMultiplier = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadUpgrades();
    }

    public void UnlockAbility(string abilityId)
    {
        switch (abilityId)
        {
            case "WallJump":
                wallJumpUnlocked = true;
                break;
            case "LedgeClimb":
                ledgeClimbUnlocked = true;
                break;
        }

        // Save state irreversibly
        if (IrreversibleEventManager.Instance != null)
        {
            IrreversibleEventManager.Instance.TriggerEvent($"Unlock_{abilityId}");
        }
        
        SyncWithPlayer();
    }

    public void LoadUpgrades()
    {
        if (IrreversibleEventManager.Instance == null) return;

        wallJumpUnlocked = IrreversibleEventManager.Instance.IsEventTriggered("Unlock_WallJump");
        ledgeClimbUnlocked = IrreversibleEventManager.Instance.IsEventTriggered("Unlock_LedgeClimb");
        
        SyncWithPlayer();
    }

    public void SyncWithPlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.canWallJump = wallJumpUnlocked;
            // ledgeClimb hook once implemented
        }
    }
}
