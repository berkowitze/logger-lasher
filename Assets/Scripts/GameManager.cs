using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI gameOverText;

    private GameDifficulty difficulty = GameDifficulty.UNSET;
    private int kills;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(MainMenu());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(GameLoop());
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator MainMenu()
    {
        mainMenu.SetActive(true);
        while (difficulty == GameDifficulty.UNSET)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
            }
            yield return null;
        }
        mainMenu.SetActive(false);
    }

    IEnumerator PlayGame()
    {
        kills = 0;

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
            UpdateKillCount();
            yield return null;
        }
        if (playerController.IsDead())
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.gameObject.GetComponent<GameOverText>().StartFade();
            // only do this if the player is dead, not if they pressed escape
            yield return new WaitForSeconds(4f);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void AddKill()
    {
        kills++;
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
}
