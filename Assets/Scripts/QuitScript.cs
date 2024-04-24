using System.Collections;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayLittle());
    }

    IEnumerator DelayLittle()
    {
        yield return new WaitForSeconds(5 * 60);
        Application.Quit();
    }
}