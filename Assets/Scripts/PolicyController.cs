using UnityEngine;

public class PolicyController : MonoBehaviour
{
    public float speed;

    // Parameters for ellipse
    private float a;
    private float b;
    private float xCenter;
    private float zCenter;
    private float angle;
    private float initialAngle;



    void Start()
    {
        a = Random.Range(5.5f, 7f);
        b = Random.Range(5.5f, 7f);
        xCenter = Random.Range(-1.8f, 1.8f);
        zCenter = Random.Range(-1.8f, 1.8f);
        angle = Random.Range(0f, 2f * Mathf.PI);
        initialAngle = angle;
        UpdatePosition();
    }

    void Update()
    {
        angle += Time.deltaTime * speed;
        if (angle > initialAngle + 2 * Mathf.PI) // If powerup has gone in a full circle, destroy it
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = new Vector3(a * Mathf.Cos(angle) - xCenter, 0, b * Mathf.Sin(angle) - zCenter);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Branch"))
        {
            FindObjectOfType<PlayerController>().CollectPolicy();
            Destroy(gameObject);
        }
    }
}
