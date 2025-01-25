using PlayerController;
using System.Collections;
using UnityEngine;

public class UltimatePickUp : PlayerPickUpObjectBase
{
    [SerializeField]
    private float ultimateForce = 10f;
    [SerializeField]
    private float durationSeconds = 10f;

    protected override void ApplyEffect(Player player)
    {
        if (player.ultimateFormEnabled) return;

        player.TriggerUltimateForm(durationSeconds, ultimateForce);
        base.ApplyEffect(player);
    }
}
