using UnityEngine;

public class Axe : MonoBehaviour
{
    public LayerMask playerHitboxLayer;

    private readonly int damage = 5;

    private readonly float pitchMin = 0.5f;
    private readonly float pitchMax = 0.9f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.Play();

            FindObjectOfType<PlayerController>().TakeDamage(damage);
        }
    }
}
