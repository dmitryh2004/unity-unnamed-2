using UnityEngine;

public class OpenWebPage : MonoBehaviour
{
    // Вставьте сюда нужный URL
    [SerializeField] string url = "https://example.com";

    // Этот метод нужно назначить на кнопку через инспектор
    public void OpenPage()
    {
        Application.OpenURL(url);
    }
}
