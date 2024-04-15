using TMPro;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    private bool isFadingIn = false;
    private TextMeshProUGUI text;

    void Update()
    {
        if (isFadingIn)
        {
            text.alpha = Mathf.Clamp01(text.alpha + 0.003f);
        }
    }

    public void StartFade()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.alpha = 0f;
        isFadingIn = true;
    }
}
