using UnityEngine;

public class GameMusic : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().time = 4.3f;
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<GameMusic>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
