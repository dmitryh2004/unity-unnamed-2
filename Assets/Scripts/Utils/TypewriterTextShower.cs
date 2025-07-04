using UnityEngine;
using System.Collections;
using TMPro;

public class TypewriterTextShower : MonoBehaviour
{
    public static TypewriterTextShower Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public Coroutine ShowText(TMP_Text textElement, string text, TypewiterAudioPlayer tap, bool playAudio = true)
    {
        Coroutine c = StartCoroutine(ShowTextCoroutine(textElement, text, tap, playAudio));
        return c;
    }
    IEnumerator ShowTextCoroutine(TMP_Text textElement, string text, TypewiterAudioPlayer tap, bool playAudio = true)
    {
        for (int i = 0; i < text.Length; i++)
        {
            textElement.text += text[i];
            if (playAudio)
            {
                if (i % 3 == 0)
                {
                    tap.PlayTypewriterSound();
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
    }
}
