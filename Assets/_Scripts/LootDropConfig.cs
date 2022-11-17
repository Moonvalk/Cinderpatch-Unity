using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootDropConfig", menuName = "ScriptableObjects/Collectables/LootDropConfig", order = 1)]
public class LootDropConfig : ScriptableObject
{
    public GameObject[] Items;
}
