using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Vector2 coords;
    [SerializeField] NodeType type;
    int currentHP, currentAttack, value1;
    int nearestBonusRange = 5;
    bool visited = false;
    bool active = false;
    bool rangeShown = false;

    [SerializeField] Animator anim;
    [SerializeField] TMP_Text hpText, attackText, nearestBonusRangeText;
    [SerializeField] Image image;

    [SerializeField] List<Node> neighbours = new();

    public void Reinitialize()
    {
        visited = false;
        active = false;
        rangeShown = false;
        type = null;
        ClearNeighbours();
    }

    public Vector2 GetCoords()
    {
        return coords;
    }

    public int GetCurrentHP()
    {
        return currentHP;
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

    public void ClearNeighbours()
    {
        neighbours.RemoveAll(x => true);
    }

    public NodeType GetNodeType()
    {
        return type;
    }

    public void SetNodeType(NodeType nodeType)
    {
        type = nodeType;

        int difficulty = HackWindowController.Instance.GetDifficulty();
        currentHP = type.hpByDifficulty[difficulty - 1];
        currentAttack = type.attackByDifficulty[difficulty - 1];
        value1 = type.value1ByDifficulty[difficulty - 1];
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
        if (visited && !IsCoreNode() && !IsBonus()) return true; //если узел не центральный, не бонус и уже посещен, то true

        bool accessible = false; //изначально считаем, что узел недоступен
        foreach(Node n in neighbours)
        {
            if (n.IsDefensiveNode() && n.IsActive()) return false; // если соседний защитный узел активен, то false
            if (!n.IsDefensiveNode() && n.IsVisited()) accessible = true; // если соседний узел пуст и уже посещен, то клетка доступна
            if (n.IsDefensiveNode() && n.IsVisited() && !n.IsActive()) accessible = true; // если соседний защитный узел был уничтожен, то клетка доступна
        }

        return accessible;
    }

    public bool IsDefensiveNode()
    {
        return GetNodeType() != null && !IsCoreNode() && !IsBonus();
    }

    public bool IsCoreNode()
    {
        return (GetNodeType() != null) && GetNodeType().isCoreNode;
    }

    public bool IsBonus()
    {
        return (GetNodeType() != null) && GetNodeType().isBonus;
    }

    public void StartVisit()
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
                if (IsActive())
                {
                    TakeDamage(VirusController.Instance.GetCurrentAttack());
                    if (IsActive())
                    {
                        Attack();
                    }
                }
                else
                {
                    active = true;
                }
            }
            else if (IsBonus())
            {
                if (IsActive())
                {
                    if (BonusController.Instance.AddBonus(type.bonus))
                    {
                        type = null;
                        active = false;
                        HackWindowController.Instance.GetGridController().RemoveNodeFromLists(this);
                    }
                }
                else
                {
                    active = true;
                }
            }
            else
            {
                active = false;
            }

            if (!IsVisited())
            {
                HackWindowController.Instance.GetGridController().UpdateBonusRanges();
            }
            if (!rangeShown && !active)
            {
                anim.SetTrigger("ShowRange");
                rangeShown = true;
            }
            visited = true;
            UpdateIcon();
            foreach(Node n in neighbours)
            {
                n.UpdateIcon();
            }
        }
    }

    public void UpdateIcon()
    {
        //Debug.Log($"{gameObject.name}: updating icon");
        bool showHP = false;
        if (!IsAccessible())
        {
            //Debug.Log($"{gameObject.name}: not accessible");
            if ((IsCoreNode() || IsBonus()) && IsVisited())
            {
                Color color;
                
                if (IsCoreNode())
                {
                    int difficulty = HackWindowController.Instance.GetDifficulty();
                    showHP = true;
                    image.sprite = type.spriteByDifficulty[difficulty - 1];
                    color = type.colorByDifficulty[difficulty - 1];
                }
                else
                {
                    image.sprite = type.bonus.nodeSprite;
                    color = Color.white;
                }
                
                color.a = 0.5f;
                image.color = color;
            }
            else
            {
                image.sprite = NodeImages.Instance.notAccessibleNode;
                image.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            //Debug.Log($"{gameObject.name}: accessible");
            if (IsVisited())
            {
                //Debug.Log($"{gameObject.name}: visited");
                if (!IsActive())
                {
                    //Debug.Log($"{gameObject.name}: not active");
                    image.sprite = NodeImages.Instance.visitedNode;
                    image.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    //Debug.Log($"{gameObject.name}: active");
                    if (IsBonus())
                    {
                        image.sprite = type.bonus.nodeSprite;
                        image.color = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        showHP = true;
                        int difficulty = HackWindowController.Instance.GetDifficulty();
                        image.sprite = type.spriteByDifficulty[difficulty - 1];
                        image.color = type.colorByDifficulty[difficulty - 1];
                    }
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

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        active = false;
        foreach (Node n in neighbours)
        {
            n.UpdateIcon();
        }
        if (IsCoreNode())
        {
            HackWindowController.Instance.SuccessLock();
        }
        type = null;
        HackWindowController.Instance.GetGridController().RemoveNodeFromLists(this);
    }

    public void Repair(int value)
    {
        currentHP += value;
        UpdateIcon();
    }

    public int GetValue1()
    {
        return value1;
    }

    void Attack()
    {
        VirusController.Instance.TakeDamage(currentAttack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsVisited() || IsAccessible())
            HackWindowController.Instance.SetHoveredNode(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HackWindowController.Instance.ClearHoveredNode();
    }
}
