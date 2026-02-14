using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Adaptive Difficulty Values", menuName = "Scriptable Objects/Adaptive Difficulty Value List")]
public class AdaptiveDifficultyValues : ScriptableObject
{
    public List<UpgradableValueData> parameters;

    public UpgradableValueData GetParameterList(string name)
    {
        return parameters.FirstOrDefault((x) => x.name == name);
    }

    public float? GetParameterValue(string name, int level)
    {
        return parameters.FirstOrDefault((x) => x.name == name)?.upgradableValueList.FirstOrDefault((x) => x.level == level)?.value;
    }
}
