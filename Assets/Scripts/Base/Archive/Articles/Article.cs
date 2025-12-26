using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArticlePart
{
    
}

[System.Serializable]
public class ArticlePartText : ArticlePart
{
    [TextArea(8, 10)] public string text;
    public int fontSize = 18;
    public bool bold;
    public bool italic;
    public bool underline;
    public Color color = new Color(0f, 0.5f, 0f, 1f);
    public TextAlignment textAlignment;
}

[System.Serializable]
public class ArticlePartImage : ArticlePart
{
    public Sprite sprite;
    public int width, height;
}

[System.Serializable]
public class ArticlePartLink : ArticlePart
{
    public string text;
    public Article article;
}

[CreateAssetMenu(fileName = "Article", menuName = "Scriptable Objects/Archive Article")]
public class Article : ScriptableObject
{
    public string header;
    [SerializeReference] public List<ArticlePart> articleParts = new();
}