using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] Transform gridParent;
    List<Node> nodes = new();
    List<Node> emptyNodes = new();
    List<Node> positiveNodes = new(); // центральный узел и бонусы
    List<Node> negativeNodes = new(); // защита
    List<Node> repairNodes = new(); // ремонтные узлы
    List<Node> pacifierNodes = new(); // блокировщики
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

        ReinitializeNodes();
    }

    void ReinitializeNodes()
    {
        nodes.RemoveAll(x => true);
        emptyNodes.RemoveAll(x => true);
        positiveNodes.RemoveAll(x => true);
        negativeNodes.RemoveAll(x => true);
        repairNodes.RemoveAll(x => true);
        pacifierNodes.RemoveAll(x => true);

        int childCount = gridParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = gridParent.GetChild(i);
            if (child.CompareTag("HackGrid"))
            {
                Node node = child.GetComponent<Node>();
                nodes.Add(node);
                node.Reinitialize();
                node.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Возвращает количество узлов между начальной и конечной точкой (без учета активности узлов).
    /// </summary>
    /// <param name="start">Начало</param>
    /// <param name="end">Конец</param>
    /// <returns>Расстояние в узлах</returns>
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

    /// <summary>
    /// Возвращает количество узлов между начальным и конечным узлом (с учетом активности узлов).
    /// </summary>
    /// <param name="start">Начало</param>
    /// <param name="end">Конец</param>
    /// <returns>Расстояние в узлах</returns>
    int CalculateDistance(Node start, Node end)
    {
        if (start == end) return 0;

        var visited = new HashSet<Node>();
        var queue = new Queue<(Node node, int dist)>();
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            foreach (Node n in current.GetNeighbours())
            {
                if (n == end) return dist + 1;
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue((n, dist + 1));
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

        // stage 1: place central core
        int centralCoreIndex = random.Next(0, emptyNodes.Count);
        Node centralCore = emptyNodes[centralCoreIndex]; 
        centralCore.SetNodeType(NodeTypes.Instance.CentralCore);
        positiveNodes.Add(centralCore);
        emptyNodes.Remove(centralCore);

        // stage 2: place defense
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
            Node newWall = emptyNodes[wallIndex];
            newWall.SetNodeType(NodeTypes.Instance.WallNode);
            negativeNodes.Add(newWall);
            emptyNodes.Remove(newWall);
            wallsCount++;
        }

        //place antiviruses
        int antivirusCount = 0;

        while (antivirusCount < antivirusRequired)
        {
            int antivirusIndex = random.Next(0, emptyNodes.Count);
            Node newAntivirus = emptyNodes[antivirusIndex];
            newAntivirus.SetNodeType(NodeTypes.Instance.AntivirusNode);
            negativeNodes.Add(newAntivirus);
            emptyNodes.Remove(newAntivirus);
            antivirusCount++;
        }

        //place repair nodes
        int repairCount = 0;

        while (repairCount < repairRequired)
        {
            int repairIndex = random.Next(0, emptyNodes.Count);
            Node newRepair = emptyNodes[repairIndex];
            newRepair.SetNodeType(NodeTypes.Instance.RepairNode);
            negativeNodes.Add(newRepair);
            repairNodes.Add(newRepair);
            emptyNodes.Remove(newRepair);
            repairCount++;
        }

        //place pacifiers
        int pacifierCount = 0;

        while (pacifierCount < pacifierRequired)
        {
            int pacifierIndex = random.Next(0, emptyNodes.Count);
            Node newPacifier = emptyNodes[pacifierIndex];
            newPacifier.SetNodeType(NodeTypes.Instance.PacifierNode);
            negativeNodes.Add(newPacifier);
            pacifierNodes.Add(newPacifier);
            emptyNodes.Remove(newPacifier);
            pacifierCount++;
        }

        //stage 3: place bonuses
        //define amount of bonuses
        int reinforcementsRequired = random.Next(
                reinforcementMinCountByDifficulty[difficulty - 1],
                reinforcementMaxCountByDifficulty[difficulty - 1] + 1
            );
        int dividersRequired = random.Next(
                dividerMinCountByDifficulty[difficulty - 1],
                dividerMaxCountByDifficulty[difficulty - 1] + 1
            );
        int additionalEncryptionsRequired = random.Next(
                additionalEncryptionMinCountByDifficulty[difficulty - 1],
                additionalEncryptionMaxCountByDifficulty[difficulty - 1] + 1
            );
        int additionalAttacksRequired = random.Next(
                additionalAttackMinCountByDifficulty[difficulty - 1],
                additionalAttackMaxCountByDifficulty[difficulty - 1] + 1
            );

        //place reinforcements
        int reinforcementsCount = 0;

        while (reinforcementsCount < reinforcementsRequired)
        {
            int reinforcementIndex = random.Next(0, emptyNodes.Count);
            Node newReinforcement = emptyNodes[reinforcementIndex];
            newReinforcement.SetNodeType(NodeTypes.Instance.BonusNode(BonusTypes.Instance.Reinforcement));
            positiveNodes.Add(newReinforcement);
            emptyNodes.Remove(newReinforcement);
            reinforcementsCount++;
        }

        //place dividers
        int dividersCount = 0;

        while (dividersCount < dividersRequired)
        {
            int dividerIndex = random.Next(0, emptyNodes.Count);
            Node newDivider = emptyNodes[dividerIndex];
            newDivider.SetNodeType(NodeTypes.Instance.BonusNode(BonusTypes.Instance.Divider));
            positiveNodes.Add(newDivider);
            emptyNodes.Remove(newDivider);
            dividersCount++;
        }

        Debug.Log($"Placed {dividersCount} dividers");
    }

    public void UpdateBonusRanges()
    {
        foreach(Node n in nodes)
        {
            int minDist = 5;
            foreach(Node p in positiveNodes)
            {
                if (!p.IsVisited())
                    minDist = Mathf.Min(minDist, CalculateDistance(n, p));
            }
            n.SetNearestBonusRange(minDist);
        }
    }

    public void RemoveNodeFromLists(Node node)
    {
        negativeNodes.Remove(node);
        positiveNodes.Remove(node);
    }

    void SelectStartNode()
    {
        Node startNode = positiveNodes[0]; //центральный узел

        List<Node> visited = new();
        Dictionary<Node, int> nodes = new();
        Queue<Node> queue = new();

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            foreach (Node n in current.GetNeighbours())
            {
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                    if (emptyNodes.Contains(n))
                        nodes.Add(n, CalculateDistance(startNode, n));
                }
            }
        }

        var sortedNodes = nodes.OrderByDescending(pair => pair.Value).ToList();

        int sum = 0;
        foreach (var n in sortedNodes)
        {
            sum += n.Value;
        }

        int choice = random.Next(0, sum);
        sum = 0;
        foreach (var n in sortedNodes)
        {
            sum += n.Value;
            if (sum > choice)
            {
                n.Key.StartVisit();
                break;
            }
        }
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

        ReinitializeNodes();

        RemoveRedundantNodes(size);

        SetNeighbours();

        RemoveRandomNodes(size);

        PlaceNodes(difficulty);

        UpdateBonusRanges();

        SelectStartNode();

        UpdateNodes();
    }

    public void MakeStepPre()
    {
        List<Node> repairTargets = negativeNodes.FindAll(x => x.IsActive());
        foreach (Node repair in repairNodes)
        {
            if (!repair.IsActive()) continue;
            if (repairTargets.Count == 1 && repair == repairTargets[0]) continue;
            List<Node> repairTargetsExceptRepair = repairTargets.FindAll(x => x != repair);
            Node target = repairTargetsExceptRepair[random.Next(0, repairTargetsExceptRepair.Count)];

            target.Repair(repair.GetValue1());
        }
    }
    public void MakeStepPost()
    {
        VirusController.Instance.RecalculateAttack();
    }

    public int GetPacifierDebuff()
    {
        List<Node> activePacifiers = pacifierNodes.FindAll(x => x.IsActive());
        int sum = 0;
        foreach(Node p in activePacifiers)
        {
            sum += p.GetValue1();
        }
        return sum;
    }
}
