using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float stopDistance; // distance from origin that enemies will stop at
    public float speed;
    public float speedRange; // range of random amount of speed to subtract/add to speed
    public LayerMask playerHitboxLayer;
    public ParticleSystem gotHitParticles;
    public AudioClip[] gotHitClips;
    public AudioClip whackClip;

    private int m_health;
    private readonly float thwackForce = 18; // with how much force to fly away with when killed
    private readonly float thwackRange = 2;
    private Rigidbody enemyRb;
    private float swingCooldown; // how long until enemy can swing again
    private AudioSource audioSource;

    public void Setup(int health)
    {
        audioSource = GetComponent<AudioSource>();
        m_health = health;
        enemyRb = GetComponent<Rigidbody>();
        speed += Random.Range(-speedRange, speedRange);
        swingCooldown = 0;
    }

    bool IsDead()
    {
        return m_health <= 0;
    }

    void Update()
    {
        swingCooldown -= Time.deltaTime;
        // If player is within stopDistance + 0.5f (some extra buffer space) and the axeman can swing...
        if (Physics.CheckSphere(transform.position, stopDistance + 0.5f, playerHitboxLayer) && swingCooldown <= 0)
        {
            // ... swing and reset cooldown to 1.5 seconds
            swingCooldown = 1.5f;
            GetComponent<Animator>().SetTrigger("Attack");
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        // Delay a bit to align with whack sound a bit better
        Invoke(nameof(PlayGotHitClip), 0.1f);
        // Play whack at 15% volume
        audioSource.PlayOneShot(whackClip, 0.15f);
        // Commented because it causes lag
        // gotHitParticles.Play();

        if (IsDead())
        {
            GetComponent<Animator>().SetTrigger("Die");
            enemyRb.constraints = RigidbodyConstraints.None;
            GetComponent<BoxCollider>().enabled = false; // prevent double hitting the same enemy
            Quaternion rotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(20, 40), Random.Range(-20, 20));
            enemyRb.AddForce(
                transform.position.normalized * (thwackForce + Random.Range(-thwackRange, thwackRange)),
                ForceMode.Impulse
            );
            enemyRb.AddTorque(
                Random.Range(-30, 30),
                Random.Range(-150, 150),
                Random.Range(-30, 30),
                ForceMode.Impulse
            );
            Destroy(gameObject, 2.0f); // then die after a few seconds

            FindObjectOfType<GameManager>().AddKill();
        }
    }

    void PlayGotHitClip()
    {
        AudioClip clip = gotHitClips[Random.Range(0, gotHitClips.Length)];
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.PlayOneShot(clip, 0.4f); // play at 40% volume
    }

    void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }
        // Where the enemy is trying to get
        Vector3 target = transform.position.normalized * stopDistance;
        float distanceFromTarget = (target - transform.position).magnitude;
        // value from 0-1:
        // 0 indicating enemy is close enough to target and therefore shouldn't move
        // 1 indicating it is far enough away that it can move at full speed
        float speedModifier = Mathf.Clamp(distanceFromTarget, 0f, 2f) / 2f;

        // Move forward (towards target) if not very close to it
        transform.Translate(speedModifier * speed * Time.deltaTime * Vector3.forward, Space.Self);
    }
}
