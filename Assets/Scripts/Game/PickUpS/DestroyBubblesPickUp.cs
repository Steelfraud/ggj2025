using PlayerController;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyBubblesPickUp : PlayerPickUpObjectBase
{
    [SerializeField]
    private float blastRadius = 5f;
    [SerializeField]
    private GameObject particles;

    protected override void ApplyEffect(PlayerAvatar player)
    {
        Instantiate(particles, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        Vector3 p1 = transform.position;
        RaycastHit[] hits = Physics.SphereCastAll(p1, blastRadius, transform.forward, 10f);

        List<BubbleBase> bubbles = new();

        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<BubbleBase>() != null)
            {
                bubbles.Add(hit.collider.GetComponent<BubbleBase>());
            }
            Debug.Log("PickUp");
        }

        foreach (BubbleBase bubble in bubbles)
        {
            bubble.StartCoroutine(bubble.DestroyBubble());
        }
        
        base.ApplyEffect(player);
    }
}
