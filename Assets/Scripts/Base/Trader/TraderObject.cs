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
    [SerializeField] float multiplierRange = 0.25f;
    [SerializeField] TraderUIWindowController windowController;
    [SerializeField] List<ClientTypeOption> clientTypes;
    float multiplier;
    Order order1, order2, order3;
    System.Random rand;

    private void Awake()
    {
        rand = new();
    }
    public void Init()
    {
        multiplier = QuotaSystem.Instance.GetMultiplier();
        GenerateOrders();
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
        if (max < 1.0f) max = 1.0f;
    }

    public ClientType SelectRandomClientType()
    {
        int sum = 0;
        foreach(var temp in clientTypes)
        {
            sum += temp.weight;
        }

        int choice = rand.Next(0, sum);
        sum = 0;

        for (int i = 0; i < clientTypes.Count; i++)
        {
            sum += clientTypes[i].weight;
            if (choice < sum) return ClientTypeManager.Instance.GetClientType(clientTypes[i].clientTypeID);
        }
        return null;
    }

    public void IncreaseMultiplier() 
    { 
        multiplier += multiplierStep;
        QuotaSystem.Instance.SetMultiplier(multiplier);
    }

    public void GenerateOrders()
    {
        float minMultiplier = multiplier, maxMultiplier = multiplier;
        CalculateMinMaxMultipliers(multiplier, multiplierRange, out minMultiplier, out maxMultiplier);

        float[] muls = new float[3];

        for (int i = 0; i < 3; i++)
        {
            muls[i] = minMultiplier + (float)rand.NextDouble() * (maxMultiplier - minMultiplier);
        }

        order1 = new Order();
        order1.SetRequired((int)(baseOrderSum * muls[0]));
        order1.SetMultiplier(muls[0]);
        order1.SetClientType(SelectRandomClientType());

        order2 = new Order();
        order2.SetRequired((int)(baseOrderSum * muls[1]));
        order2.SetMultiplier(muls[1]);
        order2.SetClientType(SelectRandomClientType());

        order3 = new Order();
        order3.SetRequired((int)(baseOrderSum * muls[2]));
        order3.SetMultiplier(muls[2]);
        order3.SetClientType(SelectRandomClientType());
    }

    public Order GetOrder1() => order1;
    public Order GetOrder2() => order2;
    public Order GetOrder3() => order3;
}
