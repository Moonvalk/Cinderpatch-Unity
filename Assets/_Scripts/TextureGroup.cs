using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TextureGroup", menuName = "ScriptableObjects/Textures/TextureGroup", order = 1)]
public class TextureGroup : ScriptableObject
{
    public string Name = "Default";
    public List<Texture> Textures;
}
