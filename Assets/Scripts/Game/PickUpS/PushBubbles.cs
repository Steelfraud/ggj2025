using PlayerController;
using System.Collections.Generic;
using UnityEngine;

public class PushBubbles : PlayerPickUpObjectBase
{
    [SerializeField]
    private float blastRadius = 5f;

    [SerializeField]
    private float pushForce = 5f;

    protected override void ApplyEffect(Player player)
    {
        Vector3 p1 = transform.position;
        RaycastHit[] hits = Physics.SphereCastAll(p1, blastRadius, transform.forward, 10f);

        List<BubbleBase> bubbles = new();

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<BubbleBase>() != null)
            {
                bubbles.Add(hit.collider.GetComponent<BubbleBase>());
            }
            Debug.Log("PickUp");
        }

        foreach (BubbleBase bubble in bubbles)
        {
            var newForce = bubble.transform.position - player.transform.position * pushForce;
            bubble.GetComponent<Rigidbody>().AddForce(newForce, ForceMode.VelocityChange);
        }

        base.ApplyEffect(player);
    }
}
