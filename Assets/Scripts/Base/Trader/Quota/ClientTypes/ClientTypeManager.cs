using System.Collections.Generic;
using UnityEngine;

public class ClientTypeManager : MonoBehaviour
{
    public static ClientTypeManager Instance = null;
    [SerializeField] List<ClientType> clientTypes;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public ClientType GetClientType(int id) => (0 <= id && id < clientTypes.Count) ? clientTypes[id] : null;
    public int GetID(ClientType clientType)
    {
        int id = -1;

        for (int i = 0; i < clientTypes.Count; i++)
        {
            if (clientTypes[i] == clientType)
            {
                id = i;
                break;
            }
        }

        return id;
    }
}
