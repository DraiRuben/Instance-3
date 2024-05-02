using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MedalRequirements : SerializedScriptableObject
{
    public Dictionary<MedalType, int> MinRequiredForMedal = new();
}
