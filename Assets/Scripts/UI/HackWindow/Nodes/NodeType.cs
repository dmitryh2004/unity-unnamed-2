using UnityEngine;

[CreateAssetMenu(fileName = "NodeType", menuName = "Scriptable Objects/NodeType")]
public class NodeType : ScriptableObject
{
    [Header("Общая информация")]
    public int id;
    public Sprite[] spriteByDifficulty = new Sprite[10];
    public Color[] colorByDifficulty = new Color[10];
    public string nodeName;

    [TextArea(6, 10)] public string nodeDesc;

    public bool isCoreNode;
    public bool isBonus;
    public BonusType bonus;

    [Header("Элемент защиты - Баланс")]
    public int minDifficulty;
    public int[] hpByDifficulty = new int[10];
    public int[] attackByDifficulty = new int[10];
    public int[] value1ByDifficulty = new int[10];
}
