using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FarmConfiguration", menuName = "ScriptableObjects/Farming/FarmConfiguration", order = 1)]
public class FarmConfiguration : ScriptableObject
{
    public int MinPumpkinSpawns = 3;
    public int MaxPumpkinSpawns = 5;
    public PumpkinType[] PumpkinTypes;
}
