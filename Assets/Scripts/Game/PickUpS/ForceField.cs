using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private float blastRadius = 5f;
    private float pushForce = 10f;
    private float duration = 7f;

    private Animator fieldAnimator;

    private void OnEnable()
    {
        if (fieldAnimator == null) fieldAnimator = transform.GetChild(0).GetComponent<Animator>();

        fieldAnimator.SetBool("ForcefieldTimeEnding", false);
        PushObjectsAway();
    }

    void PushObjectsAway()
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
            var newForce = (bubble.transform.position - transform.position) * pushForce;
            bubble.GetComponent<Rigidbody>().AddForce(newForce, ForceMode.VelocityChange);
        }
    }

    IEnumerator DisableField()
    {
        yield return new WaitForSeconds(duration);
        fieldAnimator.SetBool("ForcefieldTimeEnding", true);
        yield return new WaitForSeconds(fieldAnimator.GetCurrentAnimationLength() - 0.1f);
        gameObject.SetActive(false);
    }
}
