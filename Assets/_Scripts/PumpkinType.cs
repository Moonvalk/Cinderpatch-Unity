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
    public int MinItems = 1;
    public int MaxItems = 3;

    public PumpkinStateTexture[] TextureStates;
    public LootDropConfig LootConfig;
    public GameObject ParticleEffect;
}
