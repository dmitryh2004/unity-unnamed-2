using UnityEngine;

[CreateAssetMenu(fileName = "BonusType", menuName = "Scriptable Objects/BonusType")]
public class BonusType : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public Sprite nodeSprite;
    public string bonusName;

    [TextArea(6, 10)] public string bonusDesc;

    public int[] value1ByDifficulty = new int[10];
    public int[] value2ByDifficulty = new int[10];
    public int[] value3ByDifficulty = new int[10];
    public int[] value4ByDifficulty = new int[10];
}
