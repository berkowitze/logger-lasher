using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    public ParticleSystem onCollectParticles;
    private readonly float timeToCollect = 2f;

    public void Start()
    {
        Invoke(nameof(Despawn), timeToCollect);
    }

    public void Update()
    {
        // Don't allow negative y values
        transform.position = new Vector3(transform.position.x, Mathf.Max(0, transform.position.y), transform.position.z);
    }

    void Despawn()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Branch"))
        {
            ParticleSystem particles = Instantiate(
                onCollectParticles,
                gameObject.transform.position,
                onCollectParticles.gameObject.transform.rotation
            );

            particles.gameObject.transform.LookAt(particles.gameObject.transform.position * 1.1f + Vector3.up * 1f);
            particles.Play();
            Destroy(particles.gameObject, 1.0f);

            FindObjectOfType<PlayerController>().Heal(5);
            Destroy(gameObject);
        }
    }
}
