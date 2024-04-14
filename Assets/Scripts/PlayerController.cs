using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public int damage = 1;
    public ParticleSystem onHitParticles;

    public GameObject branchPrefab;
    public Animator branchAnim;

    public Slider healthBar;
    public Image healthBarFill;
    public Color fullhealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    public Slider policyBar;
    public Image policyBarFill;
    public Color zeroPolicyColor = Color.gray;
    public Color fullPolicyColor = Color.blue;

    private KeyCode? bufferKey = null;
    private readonly int numBufferFrames = 7;
    private int bufferFramesLeft = 3;
    private readonly Dictionary<(KeyCode, KeyCode?), int> cardinalDirectionToSlot = new(){
        {(KeyCode.W, null), 6},
        {(KeyCode.A, null), 4},
        {(KeyCode.S, null), 2},
        {(KeyCode.D, null), 0},

        // Repeat diagonal directions so lookup order doesn't matter

        {(KeyCode.W, KeyCode.D), 7},
        {(KeyCode.D, KeyCode.W), 7},

        {(KeyCode.D, KeyCode.S), 1},
        {(KeyCode.S, KeyCode.D), 1},

        {(KeyCode.S, KeyCode.A), 3},
        {(KeyCode.A, KeyCode.S), 3},

        {(KeyCode.A, KeyCode.W), 5},
        {(KeyCode.W, KeyCode.A), 5},
    };

    private int currentHealth;
    private int policyProgress;
    private int progressNeeded;

    private readonly float shakeDuration = 0.4f; // how long to shake after hit
    private readonly float shakeMagnitude = 0.1f; // how much to shake after hit
    private float? shakeTime = null;

    // map of slot index to corresponding BranchController
    private readonly Dictionary<int, BranchController> slotToBranch = new();

    public void Setup(GameDifficulty difficulty)
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        progressNeeded = difficulty == GameDifficulty.EASY ? 3 : 4;
        policyProgress = 0;
        policyBar.maxValue = progressNeeded;
        UpdatePolicyBar();

        // Set up branches for attacking
        for (int i = 0; i < 8; i++) // TODO maybe don't need all 8 in easy mode
        {
            GameObject branchGO = Instantiate(
                branchPrefab,
                gameObject.transform.position,
                Quaternion.Euler(0, i * 45 + 180, 0) // extra 180 degree offset to align with arrow keys
            );
            BranchController branchController = branchGO.GetComponent<BranchController>();
            slotToBranch.Add(i, branchController);
            branchController.Setup(i);
        }
    }

    void Update()
    {
        UpdateHealthBar();
        UpdatePolicyBar();

        if (shakeTime != null) // if shaking
        {
            if (shakeTime > shakeDuration) // if shaking is over, reset back to 0
            {
                shakeTime = null;
                transform.position = Vector3.zero;
            }
            else // otherwise, move a bit in the horizontal (x-z) plane
            {
                float shakeAmountX = Random.Range(-shakeMagnitude, shakeMagnitude);
                float shakeAmountZ = Random.Range(-shakeMagnitude, shakeMagnitude);
                transform.position = new Vector3(shakeAmountX, 0, shakeAmountZ);
            }
            shakeTime += Time.deltaTime;
        }

        // don't handle attack inputs if dead
        if (IsDead())
        {
            return;
        }

        HandleAttacks();

    }

    void HandleAttacks()
    {
        // Special attack
        if (Input.GetKeyDown(KeyCode.Space) && policyProgress >= progressNeeded)
        {
            policyProgress -= progressNeeded;
            KillAllEnemies();
            foreach (BranchController branch in slotToBranch.Values)
            {
                branch.PlayAnimation();
            }
            return;
        }

        int? attackDirection = GetSlotAttackDirection();
        // If attacking in a diagonal direction...
        if (attackDirection % 2 == 1)
        // ... buffers dont really matter
        {
            bufferKey = null;
            slotToBranch[attackDirection.Value].Attack();
            return;
        }

        KeyCode? keyDown = GetCardinalDirection();
        // If attack is buffered, buffer timer is out, and nothing else has been pressed,
        // attack in the buffered direction and reset buffer.
        if (keyDown == null && bufferFramesLeft == 0 && bufferKey.HasValue)
        {
            slotToBranch[cardinalDirectionToSlot[(bufferKey.Value, null)]].Attack();
            bufferKey = null;
            bufferFramesLeft = numBufferFrames;
        }
        // If attack is buffered, buffer timer hasn't run out, and nothing else is pressed,
        // only subtract from the buffer timer.
        else if (keyDown == null && bufferFramesLeft > 0 && bufferKey.HasValue)
        {
            bufferFramesLeft--;
        }
        // Regardless of buffer timing, if an attack is being pressed and an attack is buffered...
        else if (keyDown.HasValue && bufferKey.HasValue && keyDown != bufferKey)
        {
            // and if the combination of those two is a diagonal direction,
            // attack in that direction
            if (cardinalDirectionToSlot.ContainsKey((bufferKey.Value, keyDown.Value)))
            {
                slotToBranch[cardinalDirectionToSlot[(bufferKey.Value, keyDown.Value)]].Attack();
            }
            else
            {
                // or if pressing opposite directions, attack in both directions since that
                // doesn't correspond to a diagonal direction
                slotToBranch[cardinalDirectionToSlot[(bufferKey.Value, null)]].Attack();
                slotToBranch[cardinalDirectionToSlot[(keyDown.Value, null)]].Attack();
            }
            // Then reset the buffer
            bufferFramesLeft = numBufferFrames;
            bufferKey = null;
        }
        // If nothing is buffered and an attack is being pressed, buffer that attack
        // and reset the buffer timer.
        else if (keyDown.HasValue && !bufferKey.HasValue)
        {
            bufferKey = keyDown.Value;
            bufferFramesLeft = numBufferFrames;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.value = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBarFill.color = Color.Lerp(zeroHealthColor, fullhealthColor, currentHealth / (float)maxHealth);
    }

    void UpdatePolicyBar()
    {
        policyBar.value = Mathf.Clamp(policyProgress, 0, progressNeeded);
        policyBarFill.color = Color.Lerp(zeroPolicyColor, fullPolicyColor, policyProgress / (float)progressNeeded);
    }

    public void TakeDamage(int damage)
    {
        onHitParticles.Play();
        shakeTime = 0f;
        // Don't allow going under 0 health
        currentHealth = Mathf.Max(0, currentHealth - damage);
    }

    public void CollectPolicy()
    {
        // Don't allow going over progressNeeded
        policyProgress = Mathf.Min(policyProgress + 1, progressNeeded);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    void KillAllEnemies()
    {
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.TakeDamage(3);
        }
    }

    KeyCode? GetCardinalDirection()
    { // should only be called after checking for multiple cardinal directions pressed
        KeyCode[] cardinalKeys = { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };
        foreach (KeyCode key in cardinalKeys)
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return null;
    }

    int? GetSlotAttackDirection()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.W))
        {
            return cardinalDirectionToSlot[(KeyCode.D, KeyCode.W)];
        }
        else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.W))
        {
            return cardinalDirectionToSlot[(KeyCode.A, KeyCode.W)];
        }
        else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            return cardinalDirectionToSlot[(KeyCode.S, KeyCode.D)];
        }
        else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S))
        {
            return cardinalDirectionToSlot[(KeyCode.A, KeyCode.S)];
        }

        KeyCode? cardinalDirection = GetCardinalDirection();
        if (cardinalDirection.HasValue)
        {
            return cardinalDirectionToSlot[(cardinalDirection.Value, null)];
        }
        return null;
    }
}
