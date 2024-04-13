using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public int damage = 1;

    public GameObject branchPrefab;
    public Animator branchAnim;

    public Slider healthBar;
    public Image healthBarFill;
    public Color FullHealthColor = Color.green;
    public Color ZeroHealthColor = Color.red;

    public Slider policyBar;
    public Image policyBarFill;
    public Color ZeroPolicyColor = Color.gray;
    public Color FullPolicyColor = Color.blue;

    private int currentHealth;
    private int policyProgress;
    private int progressNeeded;

    private Dictionary<int, BranchController> slotToBranch = new Dictionary<int, BranchController>();

    public void Setup(GameDifficulty difficulty) // eventually use to only spawn 4 branches in easy mode
    {
        currentHealth = maxHealth;
        policyProgress = 0;
        progressNeeded = difficulty == GameDifficulty.EASY ? 3 : 4;
        policyBar.maxValue = progressNeeded;
        UpdateHealthBar();
        UpdatePolicyBar();
        for (int i = 0; i < 8; i++)
        {
            GameObject branchGO = Instantiate(branchPrefab, gameObject.transform.position, Quaternion.Euler(0, i * 45, 0));
            BranchController branchController = branchGO.GetComponent<BranchController>();
            slotToBranch.Add(i, branchController);
            branchController.Setup(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdatePolicyBar();
        if (Input.GetKeyDown(KeyCode.Space) && policyProgress >= progressNeeded)
        {
            policyProgress -= progressNeeded;
            KillAllEnemies();
        }
        int? attackDirection = GetSlotAttackDirection();
        if (!IsDead() && attackDirection.HasValue && slotToBranch.ContainsKey(attackDirection.Value))
        {
            slotToBranch[attackDirection.Value].Attack();
        }
    }

    void UpdateHealthBar()
    {
        healthBar.value = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBarFill.color = Color.Lerp(ZeroHealthColor, FullHealthColor, currentHealth / (float)maxHealth);
    }

    void UpdatePolicyBar()
    {
        policyBar.value = Mathf.Clamp(policyProgress, 0, progressNeeded);
        policyBarFill.color = Color.Lerp(ZeroPolicyColor, FullPolicyColor, policyProgress / (float)progressNeeded);
    }

    int? GetSlotAttackDirection()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.W))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.W))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            return 0;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            return 2;
        }
        else
        {
            return null;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Math.Max(0, currentHealth - damage);
    }

    public void CollectPolicy()
    {
        policyProgress = Math.Min(policyProgress + 1, progressNeeded);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    void KillAllEnemies()
    {
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.TakeDamage(1);
        }
    }
}
