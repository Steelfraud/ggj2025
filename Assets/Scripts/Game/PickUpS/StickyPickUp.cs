using NUnit.Framework;
using PlayerController;
using System.Collections.Generic;
using UnityEngine;

public class StickyPickUp : PlayerPickUpObjectBase
{
    [SerializeField]
    ModifierData modifierDataEnemies;

    protected override void ApplyEffect(PlayerAvatar player)
    {
        List<PlayerAvatar> players = GameManager.Instance.activePlayers;
        foreach (PlayerAvatar otherPlayer in players)
        {
            if (otherPlayer != player)
            {
                otherPlayer.PlayerModifierHandler.AddModifier(new BasicModifierSource(modifierDataEnemies));
            }
        }
        base.ApplyEffect(player);
    }
}
