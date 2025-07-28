using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance = null;
    int money = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetMoney(int money) => this.money = money;
    public int GetMoney() => money;
    public bool CanAfford(int cost) => money >= cost;
    public void AddMoney(int diff)
    {
        if (diff > 0) money += diff;
        else throw new ArgumentException($"Error: diff must be > 0 (given {diff})");
    }
    public void SubtractMoney(int diff)
    {
        if (diff > 0)
        {
            if (CanAfford(diff))
                money -= diff;
            else
                throw new ArgumentException($"Error: not enought funds in the wallet (has {money}, {money} - {diff} = {money - diff} < 0)");
        }
        else throw new ArgumentException($"Error: diff must be > 0 (given {diff})");
    }
}
