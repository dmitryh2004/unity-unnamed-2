using UnityEngine;

public class Order
{
    ClientType clientType;
    int required;
    float multiplier;

    public ClientType GetClientType() => clientType;
    public int GetRequired() => required;
    public void SetRequired(int required) => this.required = required;
    public float GetMultiplier() => multiplier;
    public void SetMultiplier(float multiplier) => this.multiplier = multiplier;
    public void SetClientType(ClientType clientType) => this.clientType = clientType;
}
