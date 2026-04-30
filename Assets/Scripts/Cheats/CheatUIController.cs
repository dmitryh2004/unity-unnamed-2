#if ALLOW_CHEATS
using UnityEngine;
using UnityEngine.UI;

public class CheatUIController : MonoBehaviour {
    [SerializeField] GameObject AD_DisabledMessage;

    private void Awake() 
    {
        DontDestroyOnLoad(this);
    }
    
    public void SetADDisabled(bool disabled) 
    {
        AD_DisabledMessage.SetActive(disabled);
    }
}
#endif