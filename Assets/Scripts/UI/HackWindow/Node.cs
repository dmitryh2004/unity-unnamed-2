using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField] Vector2 coords;
    [SerializeField] NodeType type;
    int currentHP, currentAttack;
    int nearestBonusRange = 5;
    bool visited = false;
    bool active = false;

    [SerializeField] TMP_Text hpText, attackText, nearestBonusRangeText;
    [SerializeField] Image image;

    [SerializeField] List<Node> neighbours = new();

    public Vector2 GetCoords()
    {
        return coords;
    }

    public List<Node> GetNeighbours()
    {
        return neighbours;
    }

    public void SetNearestBonusRange(int range)
    {
        nearestBonusRange = range;
        nearestBonusRangeText.text = $"{nearestBonusRange}";
    }

    public void AddNeighbour(Node another)
    {
        if (!neighbours.Contains(another))
            neighbours.Add(another);
    }

    public void RemoveNeighbour(Node another)
    {
        neighbours.Remove(another);
    }

    public NodeType GetNodeType()
    {
        return type;
    }

    public void SetNodeType(NodeType nodeType)
    {
        type = nodeType;
    }

    public bool IsVisited()
    {
        return visited;
    }

    public bool IsActive()
    {
        return active;
    }

    bool IsAccessible()
    {
        if (visited) return true; //если узел уже посещен, то true

        bool accessible = false; //изначально считаем, что узел недоступен
        foreach(Node n in neighbours)
        {
            if (n.IsDefensiveNode() && n.IsActive()) return false; // если соседний защитный узел активен, то false
            if (!n.IsDefensiveNode() && n.IsVisited()) accessible = true; // если соседний узел пуст и уже посещен, то клетка доступна
        }

        return accessible;
    }

    public bool IsDefensiveNode()
    {
        return GetNodeType() != null && !GetNodeType().isCoreNode && !GetNodeType().isBonus;
    }

    public bool IsCoreNode()
    {
        return GetNodeType().isCoreNode;
    }

    public bool IsBonus()
    {
        return GetNodeType().isBonus;
    }

    public void Visit()
    {
        visited = true;
        active = false;

        UpdateIcon();
        foreach (Node n in neighbours)
        {
            n.UpdateIcon();
        }
    }

    public void Interact()
    {
        if (IsAccessible())
        {
            if (IsDefensiveNode() || IsCoreNode())
            {
                TakeDamage(Virus.Instance.GetCurrentAttack());

                if (IsActive())
                {
                    Attack();
                }
            }
            else if (IsBonus())
            {

            }
            else
            {
                visited = true;
                active = false;
            }

            UpdateIcon();
            foreach(Node n in neighbours)
            {
                n.UpdateIcon();
            }
        }
    }

    public void UpdateIcon()
    {
        Debug.Log($"{gameObject.name}: updating icon");
        bool showHP = false;
        if (!IsAccessible())
        {
            Debug.Log($"{gameObject.name}: not accessible");
            image.sprite = NodeImages.Instance.notAccessibleNode;
            image.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            Debug.Log($"{gameObject.name}: accessible");
            if (IsVisited())
            {
                Debug.Log($"{gameObject.name}: visited");
                if (!IsActive())
                {
                    Debug.Log($"{gameObject.name}: not active");
                    image.sprite = NodeImages.Instance.visitedNode;
                    image.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    Debug.Log($"{gameObject.name}: active");
                    showHP = true;
                    int difficulty = HackWindowController.Instance.GetDifficulty();
                    image.sprite = type.spriteByDifficulty[difficulty - 1];
                    image.color = type.colorByDifficulty[difficulty - 1];
                }
            }
            else
            {
                image.sprite = NodeImages.Instance.accessibleNode;
                image.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        hpText.gameObject.SetActive(showHP);
        attackText.gameObject.SetActive(showHP);

        if (showHP)
        {
            hpText.text = $"{currentHP}";
            attackText.text = $"{currentAttack}";
        }
    }

    void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            active = false;
        }
    }

    void Attack()
    {
        Virus.Instance.TakeDamage(currentAttack);
    }
}
