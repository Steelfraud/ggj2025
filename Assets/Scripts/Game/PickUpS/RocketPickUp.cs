using PlayerController;
using UnityEngine;

public class RocketPickUp : PlayerPickUpObjectBase
{
    [SerializeField]
    private Rocket rocketPrefab;

    protected override void ApplyEffect(Player player)
    {
        var direction = player.transform.forward;

        var rocket = Instantiate(rocketPrefab, player.transform.position + 2 * direction, Quaternion.FromToRotation(transform.position, direction));
        base.ApplyEffect(player);
    }
}
