using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private float blastRadius = 5f;
    private float pushForce = 10f;

    private Animator fieldAnimator;
    private SphereCollider fieldCollider;

    private void Start()
    {
        fieldAnimator = transform.GetChild(0).GetComponent<Animator>();
        fieldCollider = GetComponentInParent<SphereCollider>();
    }

    public void EnableField(float duration)
    {
        fieldAnimator.gameObject.SetActive(true);
        fieldAnimator.SetBool("ForcefieldTimeEnding", false);
        PushObjectsAway();
        fieldCollider.radius = 1f;
        StartCoroutine(DisableField(duration));
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

    IEnumerator DisableField(float duration)
    {
        yield return new WaitForSeconds(duration);
        fieldAnimator.SetBool("ForcefieldTimeEnding", true);
        yield return new WaitForSeconds(fieldAnimator.GetCurrentAnimationLength() - 0.1f);
        fieldCollider.radius = 0.5f;
        fieldAnimator.gameObject.SetActive(false);
    }
}
