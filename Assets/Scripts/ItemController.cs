using UnityEngine;

public class ItemController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame 
    private void Update()
    {
        transform.Rotate(new Vector3(45, 45, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Destroy(gameObject);
    }
}