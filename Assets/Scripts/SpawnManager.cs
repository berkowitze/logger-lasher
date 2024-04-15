using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject weakEnemyPrefab;
    public GameObject strongEnemyPrefab;
    public GameObject policyPrefab;

    public float secondsBetweenWaves;
    public float spawnDistance; // distance from origin to spawn enemies
    public float secondsBetweenEnemiesAvg;
    public float secondsBetweenEnemiesRange;

    public float chanceOfPolicySpawnPerWave;

    // every wave, waveNumber enemies will be spawned.
    private int waveNumber;
    // first wave to spawn strong enemies in
    private readonly int firstStrongEnemyWave = 6;
    // This holds the number of enemies to spawn in each direction (index i = 45 * i degrees from x axis)
    private readonly int[] weakEnemiesInSlot = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private readonly int[] strongEnemiesInSlot = { 0, 0, 0, 0, 0, 0, 0, 0 };

    private GameDifficulty difficulty;

    public void Setup(GameDifficulty difficultyToSet)
    {
        // it's important to set difficulty before filling slots,
        // otherwise might spawn diagonal opponents on easy mode in wave 1
        difficulty = difficultyToSet;
        waveNumber = 1;

        FillSlots(1, 0);
        Invoke(nameof(SpawnWave), 2f);
    }


    private void SpawnWave()
    {
        // If all slots are empty, start the next wave after secondsBetweenWaves seconds
        int leftToSpawn = weakEnemiesInSlot.Sum() + strongEnemiesInSlot.Sum();
        if (leftToSpawn == 0)
        {
            if (Random.value < chanceOfPolicySpawnPerWave)
            {
                // Spawn more policies in later rounds
                for (int i = 0; i < Random.Range(2, waveNumber / 5 + 2); i++)
                {
                    Instantiate(policyPrefab);
                }
            }
            waveNumber++;
            if (waveNumber > 10)
            {
                chanceOfPolicySpawnPerWave = 1.0f; // in later rounds, guarantee policy spawn each round
            }
            int numStrongEnemies = waveNumber < firstStrongEnemyWave ? 0 : waveNumber == firstStrongEnemyWave ? 2 : waveNumber / 4;
            int numWeakEnemies = waveNumber < firstStrongEnemyWave ? waveNumber : waveNumber == firstStrongEnemyWave ? 0 : waveNumber - (numStrongEnemies * 2);
            FillSlots(numWeakEnemies, numStrongEnemies);
            Invoke(nameof(SpawnWave), secondsBetweenWaves);
            return;
        }

        // Otherwise, spawn up to 2 (or 3, if on hard mode) enemies in random unique slots
        HashSet<int> slotsWithEnemies = GetSlotsWithEnemies();
        int maxSimultaneousEnemiesToSpawn = Mathf.Min(leftToSpawn, difficulty == GameDifficulty.EASY ? 2 : 3, slotsWithEnemies.Count);
        int numToSpawn = Random.Range(1, maxSimultaneousEnemiesToSpawn + 1);

        for (int i = 0; i < numToSpawn; i++)
        {
            int slot = RandomSetElement(slotsWithEnemies);
            bool spawnStrongEnemy = false;
            if (weakEnemiesInSlot[slot] > 0 && strongEnemiesInSlot[slot] > 0 && Random.value < 0.5)
            {
                spawnStrongEnemy = true;
            }
            else if (strongEnemiesInSlot[slot] > 0)
            {
                spawnStrongEnemy = true;
            }

            GameObject enemyPrefab = spawnStrongEnemy ? strongEnemyPrefab : weakEnemyPrefab;
            int spawnAngle = slot * 45;
            Quaternion rotation = Quaternion.Euler(0, spawnAngle, 0);
            Vector3 xzPosition = rotation * Vector3.forward * spawnDistance;
            Vector3 position = new Vector3(xzPosition.x, enemyPrefab.transform.position.y, xzPosition.z);
            GameObject enemyGO = Instantiate(enemyPrefab, position, Quaternion.Euler(0, spawnAngle + 180, 0));
            enemyGO.GetComponent<EnemyController>().Setup();
            slotsWithEnemies.Remove(slot); // we don't want to send multiple enemies from the same direction
            if (spawnStrongEnemy)
            {
                strongEnemiesInSlot[slot]--;
            }
            else
            {
                weakEnemiesInSlot[slot]--;
            }
        }

        Invoke(
            nameof(SpawnWave),
            Random.Range(secondsBetweenEnemiesAvg - secondsBetweenEnemiesRange, secondsBetweenEnemiesAvg + secondsBetweenEnemiesRange)
        );
    }

    private HashSet<int> GetSlotsWithEnemies()
    {
        HashSet<int> slotsWithEnemies = new();
        for (int i = 0; i < weakEnemiesInSlot.Length; i++)
        {
            if (weakEnemiesInSlot[i] > 0 || strongEnemiesInSlot[i] > 0)
            {
                slotsWithEnemies.Add(i);
            }
        }

        return slotsWithEnemies;
    }

    private int RandomSetElement(HashSet<int> set)
    {
        List<int> setAsList = set.ToList();
        int index = Random.Range(0, setAsList.Count);
        return setAsList[index];
    }

    private void FillSlots(int numWeakEnemies, int numStrongEnemies)
    {
        int numSlots = difficulty == GameDifficulty.EASY ? 4 : 8;
        int slotMultiplier = difficulty == GameDifficulty.EASY ? 2 : 1;

        for (int i = 0; i < numWeakEnemies; i++)
        {
            int slot = Random.Range(0, difficulty == GameDifficulty.EASY ? 4 : 8);
            weakEnemiesInSlot[slot * slotMultiplier]++;
        }

        for (int i = 0; i < numStrongEnemies; i++)
        {
            int slot = Random.Range(0, numSlots);
            strongEnemiesInSlot[slot * slotMultiplier]++;
        }
    }
}
