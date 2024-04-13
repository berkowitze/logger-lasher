using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float stopDistance = 1.5f; // distance from origin that enemies will stop at
    public float speed = 10.0f;
    public LayerMask playerHitboxLayer;

    private Animator animator;
    private int m_health;
    private Rigidbody enemyRb;
    private float swingCooldown;

    public void Setup(int health)
    {
        animator = GetComponentInChildren<Animator>();
        m_health = health;
        enemyRb = GetComponent<Rigidbody>();
        speed += Random.Range(-1f, 1f);
        swingCooldown = 0;
    }

    bool IsDead()
    {
        return m_health <= 0;
    }

    void Update()
    {
        swingCooldown -= Time.deltaTime;
        if (Physics.CheckSphere(transform.position, 3f, playerHitboxLayer) && swingCooldown <= 0)
        {
            swingCooldown = 1.5f;
            animator.SetTrigger("Swing");
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (IsDead())
        {
            FindObjectOfType<GameManager>().AddKill();
            enemyRb.constraints = RigidbodyConstraints.None;
            Quaternion rotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0, 20), Random.Range(-20, 20));
            enemyRb.AddForce(rotation * transform.position.normalized * 15, ForceMode.Impulse);
            enemyRb.AddTorque(Random.Range(-10, 10), Random.Range(-50, 50), Random.Range(-10, 10), ForceMode.Impulse);
            Destroy(gameObject, 2.0f);
        }
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

        // Move forward (towards target) if not very close to it
        transform.Translate(Vector3.forward * Time.deltaTime * speed * (Mathf.Clamp(distanceFromTarget, 0f, 2f) / 2f), Space.Self);
    }
}
