using UnityEngine;

public class Order
{
    ClientType clientType;
    int required;

    public ClientType GetClientType() => clientType;
    public int GetRequired() => required;
    public void SetRequired(int required) => this.required = required;
    public void SetClientType(ClientType clientType) => this.clientType = clientType;
}
