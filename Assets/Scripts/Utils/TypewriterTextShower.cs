using UnityEngine;
using System.Collections;
using TMPro;
using System;

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
    public Coroutine ShowText(TMP_Text textElement, string text, TypewiterAudioPlayer tap, Action onComplete = null, bool playAudio = true)
    {
        Coroutine c = StartCoroutine(ShowTextCoroutine(textElement, text, tap, onComplete, playAudio));
        return c;
    }

    IEnumerator ShowTextCoroutine(TMP_Text textElement, string text, TypewiterAudioPlayer tap, Action onComplete, bool playAudio = true)
    {
        for (int i = 0; i < text.Length; i++)
        {
            textElement.text += text[i];
            if (playAudio && i % 3 == 0)
            {
                tap.PlayTypewriterSound();
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);

        onComplete?.Invoke();
    }
}
