using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TextureFrame
{
    public Texture Image;
    public float Duration;
}

[CreateAssetMenu(fileName = "TextureAnimation", menuName = "ScriptableObjects/Textures/TextureAnimation", order = 1)]
public class TextureAnimation : ScriptableObject
{
    public string Name = "Default";
    public float DefaultDuration = 1f;
    public float Speed = 1f;
    public List<TextureFrame> Frames;
}
