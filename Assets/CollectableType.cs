using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableType", menuName = "ScriptableObjects/Collectables/CollectableType", order = 1)]
public class CollectableType : ScriptableObject
{
    public string Name = "???";
    public GameObject Object;
}
