using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchController : MonoBehaviour
{
    private int m_branchSlot; // 0-7
    public LayerMask enemyLayerMask;

    public void Setup(int branchSlot)
    {
        m_branchSlot = branchSlot;
    }

    public void Attack()
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(transform.position, Quaternion.Euler(0, 45 * m_branchSlot, 0) * Vector3.right, out hit, 10.0f, enemyLayerMask);
        if (hasHit)
        {
            hit.collider.gameObject.GetComponent<EnemyController>().TakeDamage(1);
        }
        GetComponent<Animator>().SetTrigger("Attack");
    }
}
