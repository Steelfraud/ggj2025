using PlayerController;
using UnityEngine;

public class RocketPickUp : PlayerPickUpObjectBase
{
    [SerializeField]
    private Rocket rocketPrefab;

    protected override void ApplyEffect(PlayerAvatar player)
    {
        var direction = player.transform.forward;

        if (GameManager.Instance.activePlayers.Count > 1)
        {
            direction = GameManager.Instance.activePlayers.GetRandomElementFromList().transform.position;
        }

        var rocket = Instantiate(rocketPrefab, player.transform.position + 2 * direction, Quaternion.FromToRotation(transform.position, direction));
        base.ApplyEffect(player);
    }
}
