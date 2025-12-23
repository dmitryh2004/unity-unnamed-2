using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArchiveUIController : UIWindowCameraTransitioning
{
    [Header("Input")]
    [SerializeField] PlayerInput playerInput;
    [Header("Article render")]
    [SerializeField] Transform contentParent;
    [SerializeField] TMP_Text articleHeader;
    [SerializeField] TMP_FontAsset fontAsset;
    Article currentArticle = null;
    [SerializeField] Article mainArticle;

    public void SetArticle(Article article)
    {
        currentArticle = article;
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (currentArticle == null)
            if (mainArticle != null)
                SetArticle(mainArticle);
            else return;

        // Скрыть все дочерние объекты
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentParent.GetChild(i).gameObject);
        }

        articleHeader.text = currentArticle.header;

        // Создать UI элементы для каждой части статьи
        foreach (var part in currentArticle.articleParts)
        {
            GameObject uiElement = CreateUIElement(part);
            if (uiElement != null)
            {
                uiElement.transform.SetParent(contentParent, false);
            }
        }
    }

    private GameObject CreateUIElement(ArticlePart part)
    {
        if (part is ArticlePartText textPart)
        {
            return CreateTextElement(textPart);
        }
        else if (part is ArticlePartImage imagePart)
        {
            return CreateImageElement(imagePart);
        }
        else if (part is ArticlePartLink linkPart)
        {
            return CreateLinkElement(linkPart);
        }

        return null;
    }

    private GameObject CreateTextElement(ArticlePartText textPart)
    {
        GameObject textObj = new GameObject("TextPart");

        // TextMeshPro
        var textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = textPart.text;
        textMesh.fontSize = textPart.fontSize;
        textMesh.color = Color.green * 0.5f;
        textMesh.font = fontAsset;

        // Стилизация текста
        if (textPart.bold) textMesh.fontStyle |= FontStyles.Bold;
        if (textPart.italic) textMesh.fontStyle |= FontStyles.Italic;
        if (textPart.underline) textMesh.fontStyle |= FontStyles.Underline;

        // Выравнивание (нужен enum TextAlignment)
        switch (textPart.textAlignment)
        {
            case TextAlignment.Left: textMesh.alignment = TextAlignmentOptions.Left; break;
            case TextAlignment.Center: textMesh.alignment = TextAlignmentOptions.Center; break;
            case TextAlignment.Right: textMesh.alignment = TextAlignmentOptions.Right; break;
        }

        // RectTransform настройки
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, textMesh.preferredHeight);

        return textObj;
    }

    private GameObject CreateImageElement(ArticlePartImage imagePart)
    {
        GameObject imageObj = new GameObject("ImagePart");

        var image = imageObj.AddComponent<Image>();
        image.sprite = imagePart.sprite;

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(imagePart.width, imagePart.height);

        return imageObj;
    }

    private GameObject CreateLinkElement(ArticlePartLink linkPart)
    {
        GameObject linkObj = new GameObject("LinkPart");

        var textMesh = linkObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = linkPart.text;
        textMesh.fontSize = 18;
        textMesh.font = fontAsset;
        textMesh.color = Color.green;
        textMesh.fontStyle |= FontStyles.Underline;

        // Добавить кнопку для перехода
        var button = linkObj.AddComponent<Button>();
        button.onClick.AddListener(() => SetArticle(linkPart.article));

        RectTransform rect = linkObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, textMesh.preferredHeight);

        return linkObj;
    }

    protected override void ChangeToMainMenu()
    {
        SetArticle(mainArticle);
    }

    protected override void OnClosed()
    {
        SetArticle(mainArticle);
    }

    protected override void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("ArchiveUI");
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }
}
