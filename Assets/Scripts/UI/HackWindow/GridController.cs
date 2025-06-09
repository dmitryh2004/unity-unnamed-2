using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] Transform gridParent;
    List<Node> nodes = new();
    List<Node> emptyNodes = new();
    List<Node> positiveNodes = new();
    System.Random random = new();

    List<Vector2> movements = new();

    [Header("Баланс - количество защиты")]
    [SerializeField] int[] wallMinCountByDifficulty = new int[10];
    [SerializeField] int[] wallMaxCountByDifficulty = new int[10];

    [SerializeField] int[] antivirusMinCountByDifficulty = new int[10];
    [SerializeField] int[] antivirusMaxCountByDifficulty = new int[10];

    [SerializeField] int[] repairMinCountByDifficulty = new int[10];
    [SerializeField] int[] repairMaxCountByDifficulty = new int[10];

    [SerializeField] int[] pacifierMinCountByDifficulty = new int[10];
    [SerializeField] int[] pacifierMaxCountByDifficulty = new int[10];

    [Header("Баланс - количество бонусов")]
    [SerializeField] int[] reinforcementMinCountByDifficulty = new int[10];
    [SerializeField] int[] reinforcementMaxCountByDifficulty = new int[10];

    [SerializeField] int[] dividerMinCountByDifficulty = new int[10];
    [SerializeField] int[] dividerMaxCountByDifficulty = new int[10];

    [SerializeField] int[] additionalEncryptionMinCountByDifficulty = new int[10];
    [SerializeField] int[] additionalEncryptionMaxCountByDifficulty = new int[10];

    [SerializeField] int[] additionalAttackMinCountByDifficulty = new int[10];
    [SerializeField] int[] additionalAttackMaxCountByDifficulty = new int[10];

    private void InitializeMovements()
    {
        movements.Add(new Vector2(1, 0));
        movements.Add(new Vector2(-1, 0));
        movements.Add(new Vector2(0, 1));
        movements.Add(new Vector2(0, -1));
        movements.Add(new Vector2(-1, 1));
        movements.Add(new Vector2(1, -1));
    }

    private void Start()
    {
        InitializeMovements();

        int childCount = gridParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = gridParent.GetChild(i);
            if (child.CompareTag("HackGrid"))
            {
                nodes.Add(child.GetComponent<Node>());
            }
        }

        Generate(10);
    }

    int CalculateDistance(Vector2 start, Vector2 end)
    {
        if (start == end) return 0;

        var visited = new HashSet<Vector2>();
        var queue = new Queue<(Vector2 point, int dist)>();
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            foreach (var dir in movements)
            {
                Vector2 next = current + dir;
                if (next == end) return dist + 1;
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue((next, dist + 1));
                }
            }
        }

        // Если путь не найден (на этой сетке такого быть не должно)
        return -1;
    }

    private bool CheckRemovalCondition(int size, Node n)
    {
        int x = (int)n.GetCoords().x, y = (int)n.GetCoords().y;
        int absx = Mathf.Abs(x), absy = Mathf.Abs(y);
        int pathLength = CalculateDistance(n.GetCoords(), Vector2.zero);
        switch (size)
        {
            case 1:
                return absx > 2 || absy > 2 || pathLength > 2;
            case 2:
                return absx > 3 || absy > 3 || pathLength > 3;
            case 3:
                return absx > 4 || absy > 3 || pathLength > 4;
            case 4:
                return absx > 4 || absy > 3;
            case 5:
                return false;
            default:
                return false;
        }
    }

    void RemoveNode(Node n)
    {
        foreach (Node n2 in n.GetNeighbours())
        {
            n2.RemoveNeighbour(n);
        }

        n.gameObject.SetActive(false);
        nodes.Remove(n);
    }

    private void RemoveRedundantNodes(int size)
    {
        List<Node> removeList = new();
        foreach (Node n in nodes)
        {
            if (CheckRemovalCondition(size, n))
            {
                removeList.Add(n);
            }
        }

        foreach (Node n in removeList)
        {
            RemoveNode(n);
        }
    }

    private void SetNeighbours()
    {
        foreach(Node n in nodes)
        {
            Vector2 coords = n.GetCoords();
            foreach(Vector2 dir in movements)
            {
                foreach(Node n2 in nodes)
                {
                    if (n == n2) continue;
                    if (n2.GetCoords() - dir == coords)
                    {
                        n.AddNeighbour(n2);
                        n2.AddNeighbour(n);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Проверяет, будет ли граф связным, если удалить узел.
    /// </summary>
    /// <param name="n">Проверяемый узел</param>
    /// <returns>Связен ли граф после удаления узла</returns>
    bool CheckIfRemove(Node n)
    {
        if (!nodes.Contains(n)) return true;

        Node start = nodes.FirstOrDefault(node => node != n);
        Debug.Log($"start: {start.gameObject.name}");
        if (start == null) return false;

        HashSet<Node> visited = new();
        Queue<Node> queue = new();

        visited.Add(start);
        queue.Enqueue(start);
        
        while (queue.Count > 0)
        {
            Node cur = queue.Dequeue();
            //Debug.Log($"current: {cur.gameObject.name} (queue size={queue.Count})");
            foreach (Node n2 in cur.GetNeighbours())
            {
                if (n == n2) continue;
                if (!visited.Contains(n2))
                {
                    visited.Add(n2);
                    queue.Enqueue(n2);
                }
            }
        }

        //Debug.Log($"visited: {visited.Count}, nodes: {nodes.Count - 1}");
        return visited.Count == nodes.Count - 1;
    }

    void RemoveRandomNodes(int size)
    {
        int minRemove = (int) Mathf.Clamp(size - 2, 0f, 2 * size), maxRemove = 2 * size;
        int count = random.Next(minRemove, maxRemove + 1);
        int removed = 0;

        while (removed < count)
        {
            Node candidate = nodes[random.Next(0, nodes.Count)];

            if (candidate.GetNeighbours().Count < 6)
            {
                int r = random.Next(0, 3);
                if (r < 2) continue;
            }

            //Debug.Log($"candidate: {candidate.gameObject.name}");

            if (CheckIfRemove(candidate))
            {
                //Debug.Log($"{candidate.gameObject.name} can be deleted");
                RemoveNode(candidate);
                removed++;
            }
        }
    }

    void PlaceNodes(int difficulty)
    {
        foreach (Node n in nodes)
        {
            emptyNodes.Add(n);
        }

        // place central core
        int centralCoreIndex = random.Next(0, emptyNodes.Count);
        emptyNodes[centralCoreIndex].SetNodeType(NodeTypes.Instance.CentralCore);
        positiveNodes.Add(emptyNodes[centralCoreIndex]);
        emptyNodes.Remove(emptyNodes[centralCoreIndex]);

        // define amount of defensive nodes
        int wallsRequired = random.Next(
                wallMinCountByDifficulty[difficulty - 1],
                wallMaxCountByDifficulty[difficulty - 1] + 1
            );
        int antivirusRequired = random.Next(
                antivirusMinCountByDifficulty[difficulty - 1],
                antivirusMaxCountByDifficulty[difficulty - 1] + 1
            );
        int repairRequired = random.Next(
                repairMinCountByDifficulty[difficulty - 1],
                repairMaxCountByDifficulty[difficulty - 1] + 1
            );
        int pacifierRequired = random.Next(
                pacifierMinCountByDifficulty[difficulty - 1],
                pacifierMaxCountByDifficulty[difficulty - 1] + 1
            );

        //place walls
        int wallsCount = 0;

        while (wallsCount < wallsRequired)
        {
            int wallIndex = random.Next(0, emptyNodes.Count);
            emptyNodes[wallIndex].SetNodeType(NodeTypes.Instance.WallNode);
            emptyNodes.Remove(emptyNodes[wallIndex]);
            wallsCount++;
        }

        //place antiviruses
        int antivirusCount = 0;

        while (antivirusCount < antivirusRequired)
        {
            int antivirusIndex = random.Next(0, emptyNodes.Count);
            emptyNodes[antivirusIndex].SetNodeType(NodeTypes.Instance.AntivirusNode);
            emptyNodes.Remove(emptyNodes[antivirusIndex]);
            antivirusCount++;
        }

        //place repair nodes
        int repairCount = 0;

        while (repairCount < repairRequired)
        {
            int repairIndex = random.Next(0, emptyNodes.Count);
            emptyNodes[repairIndex].SetNodeType(NodeTypes.Instance.RepairNode);
            emptyNodes.Remove(emptyNodes[repairIndex]);
            repairCount++;
        }

        //place pacifiers
        int pacifierCount = 0;

        while (pacifierCount < pacifierRequired)
        {
            int pacifierIndex = random.Next(0, emptyNodes.Count);
            emptyNodes[pacifierIndex].SetNodeType(NodeTypes.Instance.PacifierNode);
            emptyNodes.Remove(emptyNodes[pacifierIndex]);
            pacifierCount++;
        }
    }

    void UpdateBonusRanges()
    {
        foreach(Node n in nodes)
        {
            int minDist = 5;
            foreach(Node p in positiveNodes)
            {
                minDist = Mathf.Min(minDist, CalculateDistance(n.GetCoords(), p.GetCoords()));
            }
            n.SetNearestBonusRange(minDist);
        }
    }

    void SelectStartNode()
    {
        int startIndex = random.Next(0, emptyNodes.Count);

        emptyNodes[startIndex].Visit();
    }

    void UpdateNodes()
    {
        foreach(Node n in nodes)
        {
            n.UpdateIcon();
        }
    }

    public void Generate(int difficulty)
    {
        if (difficulty < 1 || difficulty > 10)
        {
            Debug.LogError($"{gameObject.name}: difficulty out of bounds ({difficulty}). It must be between 1 and 10.");
            return;
        }

        int size = (difficulty - 1) / 2 + 1;
        

        RemoveRedundantNodes(size);

        SetNeighbours();

        RemoveRandomNodes(size);

        PlaceNodes(difficulty);

        UpdateBonusRanges();

        SelectStartNode();

        UpdateNodes();
    }
}
