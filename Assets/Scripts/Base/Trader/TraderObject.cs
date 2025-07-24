using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClientTypeOption
{
    public int clientTypeID;
    public int weight;
}

public class TraderObject : Interactable
{
    [SerializeField] int baseOrderSum = 100_000;
    [SerializeField] float multiplierStep = 0.8f;
    [SerializeField] TraderUIWindowController windowController;
    [SerializeField] List<ClientTypeOption> clientTypes;
    Order order1, order2, order3;
    System.Random rand;

    private void Awake()
    {
        rand = new();
    }
    public override void Interact()
    {
        windowController.ShowWindow();
    }

    void CalculateMinMaxMultipliers(float multiplier, float range, out float min, out float max)
    {
        min = multiplier - range * (float)rand.NextDouble();
        if (min < 1.0f) min = 1.0f;
        max = multiplier + range * (float)rand.NextDouble();
    }

    public ClientType SelectRandomClientType()
    {
        int sum = 0;
        foreach(var temp in clientTypes)
        {
            sum += temp.weight;
        }
        return null;
    }

    public void GenerateOrders()
    {
        float multiplier = QuotaSystem.Instance.GetMultiplier();
        float minMultiplier = multiplier, maxMultiplier = multiplier;
        CalculateMinMaxMultipliers(multiplier, multiplierStep, out minMultiplier, out maxMultiplier);

        float[] muls = new float[3];

        for (int i = 0; i < 3; i++)
        {
            muls[i] = minMultiplier + (float)rand.NextDouble() * (maxMultiplier - minMultiplier);
        }
    }
}
