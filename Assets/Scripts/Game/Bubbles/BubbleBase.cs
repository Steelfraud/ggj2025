using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleBase : MonoBehaviour
{
    [SerializeField]
    private float force = 1000f;

    private float destroyAfterSeconds = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(100 * transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 direction = other.transform.position - transform.position;

        other.collider.GetComponent<Rigidbody>().AddForce(force * direction);
    }

    private void OnBecameInvisible()
    {
        if (enabled && gameObject.activeInHierarchy)
        {
            StartCoroutine(PendingDestroy());
        }
    }

    IEnumerator PendingDestroy()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);

        if (!GetComponent<MeshRenderer>().isVisible)
        {
            Destroy(this);
            Debug.Log("bubble destroyed");
        }
    }
}
