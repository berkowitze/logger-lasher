using System.Collections;
using System.Collections.Generic;
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


    void Start()
    {
        a = Random.Range(5.5f, 7f);
        b = Random.Range(5.5f, 7f);
        xCenter = Random.Range(-2f, 2f);
        zCenter = Random.Range(-2f, 2f);
        angle = Random.Range(0f, 2f * Mathf.PI);
        UpdatePosition();
        Destroy(gameObject, 4.0f);
    }

    void Update()
    {
        angle += Time.deltaTime * speed;
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
