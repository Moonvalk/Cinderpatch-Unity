using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureAnimationSet", menuName = "ScriptableObjects/Textures/TextureAnimationSet", order = 1)]
public class TextureAnimationSet : ScriptableObject
{
    public string Name = "Default";
    public TextureAnimation[] Animations;
}
