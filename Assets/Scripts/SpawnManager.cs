using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject policyPrefab;

    public float secondsBetweenWaves;
    public float spawnDistance; // distance from origin to spawn enemies
    public float secondsBetweenEnemiesAvg;
    public float secondsBetweenEnemiesRange;

    public float chanceOfPolicySpawnPerWave;

    private int waveNumber;
    // every wave, waveNumber enemies will be spawned one at a time
    // number of enemies to spawn in each direction (index i = 45 * i degrees from x axis)
    private int[] enemiesInSlot = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private GameDifficulty difficulty;

    public void Setup(GameDifficulty difficultyToSet)
    {
        // it's important to set difficulty before filling slots,
        // otherwise might spawn diagonal opponents on easy mode in wave 1
        difficulty = difficultyToSet;
        waveNumber = 1;
        FillSlots(waveNumber);
        Invoke(nameof(SpawnWave), 2f);
    }


    private void SpawnWave()
    {
        // If all slots are empty, start the next wave after secondsBetweenWaves seconds
        int leftToSpawn = enemiesInSlot.Sum();
        if (leftToSpawn == 0)
        {
            if (Random.value < chanceOfPolicySpawnPerWave)
            {
                // Spawn more policies in later rounds
                for (int i = 0; i < Random.Range(1, waveNumber / 5 + 1); i++)
                {
                    Instantiate(policyPrefab);
                }
            }
            waveNumber++;
            if (waveNumber > 10)
            {
                chanceOfPolicySpawnPerWave = 1.0f; // in later rounds, guarantee policy spawn each round
            }
            FillSlots(waveNumber);
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
            int spawnAngle = slot * 45;
            Quaternion rotation = Quaternion.Euler(0, spawnAngle, 0);
            Vector3 xzPosition = rotation * Vector3.forward * spawnDistance;
            Vector3 position = new Vector3(xzPosition.x, enemyPrefab.transform.position.y, xzPosition.z);
            GameObject enemyGO = Instantiate(enemyPrefab, position, Quaternion.Euler(0, spawnAngle + 180, 0));
            enemyGO.GetComponent<EnemyController>().Setup(1);
            slotsWithEnemies.Remove(slot); // we don't want to send multiple enemies from the same direction
            enemiesInSlot[slot]--;
        }

        Invoke(nameof(SpawnWave), Random.Range(secondsBetweenEnemiesAvg - secondsBetweenEnemiesRange, secondsBetweenEnemiesAvg + secondsBetweenEnemiesRange));
    }

    private HashSet<int> GetSlotsWithEnemies()
    {
        HashSet<int> slotsWithEnemies = new HashSet<int>();
        for (int i = 0; i < enemiesInSlot.Length; i++)
        {
            if (enemiesInSlot[i] > 0)
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

    private void FillSlots(int numToAdd)
    {
        for (int i = 0; i < numToAdd; i++)
        {
            int slot = Random.Range(0, difficulty == GameDifficulty.EASY ? 4 : 8);
            enemiesInSlot[slot * (difficulty == GameDifficulty.EASY ? 2 : 1)]++;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        int numIntervals = difficulty == GameDifficulty.EASY ? 4 : 8;
        int spawnAngle = 360 / numIntervals * Random.Range(0, numIntervals - 1);
        Quaternion rotation = Quaternion.Euler(0, spawnAngle, 0);
        Vector3 xzPosition = rotation * Vector3.forward * spawnDistance;
        return new Vector3(xzPosition.x, enemyPrefab.transform.position.y, xzPosition.z);
    }
}
