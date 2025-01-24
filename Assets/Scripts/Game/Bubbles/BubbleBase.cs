using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField]
    private float force = 1000f;

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
}
