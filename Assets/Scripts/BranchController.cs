using UnityEngine;

public class BranchController : MonoBehaviour
{
    public LayerMask enemyLayerMask;

    private int m_branchSlot; // integer from 0-7
    private readonly float maxHitDistance = 10f;

    public void Setup(int branchSlot)
    {
        m_branchSlot = branchSlot;
    }

    public void Attack()
    {
        PlayAnimation();
        // Cast out a ray in the branch direction to determine the first enemy hit, if any.
        bool hasHit = Physics.Raycast(
            transform.position,
            Quaternion.Euler(0, 45 * m_branchSlot, 0) * Vector3.right,
            out RaycastHit hit,
            maxHitDistance,
            enemyLayerMask
        );
        if (hasHit)
        {
            hit.collider.GetComponent<EnemyController>().TakeDamage(1);
        }
    }

    public void PlayAnimation()
    {
        GetComponent<Animator>().SetTrigger("Attack");
    }
}
