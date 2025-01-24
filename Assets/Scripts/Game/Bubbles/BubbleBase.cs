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
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("collision");
            Vector3 direction = other.transform.position - transform.position;
            float newForce = other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude + force;
            other.collider.GetComponent<Rigidbody>().AddForce(newForce * direction, ForceMode.VelocityChange);
            Destroy(gameObject);
        }
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
            Destroy(gameObject);
        }
    }
}
