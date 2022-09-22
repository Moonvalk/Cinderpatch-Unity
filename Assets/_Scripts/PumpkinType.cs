using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PumpkinStateTexture
{
    public Texture Image;
    public PumpkinState State;
}

[CreateAssetMenu(fileName = "PumpkinType", menuName = "ScriptableObjects/Farming/PumpkinType", order = 1)]
public class PumpkinType : ScriptableObject
{
    public string Name = "???";
    public GameObject PumpkinObject;
    public int MinSeeds = 1;
    public int MaxSeeds = 3;

    public PumpkinStateTexture[] TextureStates;
}
