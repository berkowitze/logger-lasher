using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameDifficulty
{
    UNSET,
    EASY,
    HARD
}

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playerPrefab;
    public GameObject spawnerPrefab;
    public TextMeshProUGUI scoreText;

    private GameDifficulty difficulty = GameDifficulty.UNSET;
    private int kills;

    // Start is called before the first frame update
    void Start()
    {
        kills = 0;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(MainMenu());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(GameLoop());
    }

    IEnumerator MainMenu()
    {
        mainMenu.SetActive(true);
        while (difficulty == GameDifficulty.UNSET)
        {
            yield return null;
        }
        mainMenu.SetActive(false);
    }

    IEnumerator PlayGame()
    {
        GameObject player = Instantiate(playerPrefab);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Setup(difficulty);

        GameObject spawnManager = Instantiate(spawnerPrefab);
        spawnManager.GetComponent<SpawnManager>().Setup(difficulty);

        while (!playerController.IsDead())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Update is called once per frame
    void Update()
    {
        UpdateKillCount();
    }

    void UpdateKillCount()
    {
        scoreText.text = "Loggers lashed: " + kills;
    }

    private void SetDifficulty(GameDifficulty difficultyToSet)
    {
        difficulty = difficultyToSet;
    }

    public void SetEasyDifficulty()
    {
        SetDifficulty(GameDifficulty.EASY);
    }

    public void SetHardDifficulty()
    {
        SetDifficulty(GameDifficulty.HARD);
    }

    public void AddKill()
    {
        kills++;
    }
}
